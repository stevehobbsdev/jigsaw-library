using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Dynamic;
using System.Reflection;

namespace Diggins.Jigsaw
{
    public class SchemeCompiler : ExpressionCompiler
    {
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

        public static Expression SExprToExpr(Node node, Context context)
        {
            var terms = node[0];
            CheckNode(terms, "Terms");
            var head = terms[0];
            var mi = typeof(Primitives).GetMethod(head.Text);
            if (mi == null) throw new Exception("Could not find primitive named " + head.Text);
            var args = terms.Nodes.Skip(1).Select(n => ToExpr(n, context));
            return Expression.Call(mi, args);
        }

        public static Expression LetToExpr(Node node, Context context)
        {
            var exprs = new List<Expression>();
            foreach (var binding in node[0].Nodes) {
                var name = binding[0].Text;
                var param = Expression.Parameter(typeof(Object), name);
                exprs.Add(param);
                context.AddContext(name, param);                   
            }
            foreach (var binding in node[0].Nodes) {
                var name = binding[0].Text;
                var val = ToExpr(binding[1], context);
                exprs.Add(Expression.Assign((Expression)context.Find(name), val));
            }
            exprs.Add(ToExpr(node[1], context));
            return Expression.Block(exprs);
        }

        public static Expression ToExpr(Node node, Context context)
        {
            switch (node.Label)
            {
                case "SExpr":
                    return SExprToExpr(node, context);
                case "Symbol":
                    return (Expression)context.Find(node.Text);
                case "Term":
                    return ToExpr(node[0], context);
                case "Atom":
                    return ToExpr(node[0], context);
                case "Lambda":
                    return NodeToLambda(node, context);
                case "Begin":
                    return Expression.Block(node.Nodes.Select(n => ToExpr(n, context)));
                case "Let":
                    return LetToExpr(node, context);
                case "If":
                    return node.Count == 3 
                        ? Expression.IfThenElse(ToExpr(node[0], context), ToExpr(node[1], context), ToExpr(node[2], context))
                        : Expression.IfThen(ToExpr(node[0], context), ToExpr(node[1], context));
                default:
                    return Expression.Constant(NodeToValue(node));
            }
        }

        public static Expression NodeToBody(Node node, Context context)
        {
            CheckNode(node, "Terms");
            if (node.Count == 0)
                return Noop;
            return Expression.Block(node.Nodes.Select(n => ToExpr(n, context)).ToArray());
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
