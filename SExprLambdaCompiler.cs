using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Dynamic;
using System.Reflection;

namespace Diggins.Jigsaw
{
    public class SExprLambdaCompiler
    {
        public class Context
        {
            public string Name;
            public Expression Expression;
            public Context Tail;
            public Context AddContext(string name, Expression expr)
            {
                return new Context { Name = name, Expression = expr, Tail = this };
            }
            public IEnumerable<Context> Contexts
            {
                get
                {
                    for (Context c = this; c != null; c = c.Tail)
                        yield return c;
                }
            }
            public Expression Find(string name)
            {
                var r = Contexts.FirstOrDefault(c => c.Name == name);
                if (r == null) throw new Exception("Name does not exist in context: " + name);
                return r.Expression;
            }
        }

        public static void CheckNode(Node node, string name)
        {
            if (node.Label != name)
                throw new Exception(String.Format("Expected node of type {0} not {1}", name, node.Label));
        }

        public static Expression Noop
        {
            get { return Expression.Call(null, typeof(Primitives).GetMethod("Noop")); }
        }

        public static ParameterExpression NodeToParam(Node node)
        {
            CheckNode(node, "Symbol");
            return Expression.Parameter(typeof(Object), node.Text);
        }

        public static ParameterExpression[] NodeToParams(Node node)
        {
            CheckNode(node, "ParamList");
            return node.Nodes.Select(n => NodeToParam(n)).ToArray();            
        }

        public static Object NodeToValue(Node node)
        {
            switch (node.Label)
            {
                case "Integer":
                    return int.Parse(node.Text);
                case "Float":
                    return double.Parse(node.Text);
                case "String":
                    return node.Text.Substring(1, node.Text.Length - 2);
                default:
                    throw new Exception(String.Format("Node {0} of type {1} is not a recognized value", node.Text, node.Label));
            }
        }

        public static Expression SExprToExpression(Node node, Context context)
        {
            var terms = node[0];
            CheckNode(terms, "Terms");
            var head = terms[0];
            var mi = typeof(Primitives).GetMethod(head.Text);
            if (mi == null) throw new Exception("Could not find primitive named " + head.Text);
            var args = terms.Nodes.Skip(1).Select(n => NodeToExpression(n, context));
            return Expression.Call(mi, args);
        }

        public static Expression NodeToExpression(Node node, Context context)
        {
            switch (node.Label)
            {
                case "SExpr":
                    return SExprToExpression(node, context);
                case "Symbol":
                    return context.Find(node.Text);
                case "Term":
                    return NodeToExpression(node[0], context);
                case "Atom":
                    return NodeToExpression(node[0], context);
                default:
                    return Expression.Constant(NodeToValue(node));
            }
        }

        public static Expression NodeToBody(Node node, Context context)
        {
            CheckNode(node, "Terms");
            if (node.Count == 0)
                return Noop;
            return Expression.Block(node.Nodes.Select(n => NodeToExpression(n, context)).ToArray());
        }

        public static LambdaExpression NodeToLambda(Node node)
        {
            return NodeToLambda(node, new Context());
        }

        public static LambdaExpression NodeToLambda(Node node, Context context)
        {
            CheckNode(node, "Lambda");
            var ps = NodeToParams(node["ParamList"]);
            foreach (var p in ps)
                context = context.AddContext(p.Name, p);
            return Expression.Lambda(NodeToBody(node["Terms"], context), ps);                
        }
    }
}
