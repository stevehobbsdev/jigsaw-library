using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Dynamic;
using System.Reflection;

namespace Diggins.Jigsaw
{
    public class SchemeExpressionCompiler : ExpressionCompiler
    {
        public static Delegate CompileLambda(string s)
        {
            var nodes = SchemeGrammar.Lambda.Parse(s);
            return CompileLambda(nodes[0]);
        }

        public static Delegate CompileLambda(Node n)
        {
            var compiler = new SchemeExpressionCompiler();
            var expr = (LambdaExpression)compiler.ToExpr(n);
            if (expr == null) return null;
            return expr.Compile();
        }

        public Expression SExprToExpr(Node node)
        {
            var terms = node["Terms"];
            var head = terms[0];
            var mi = typeof(Primitives).GetMethod(head.Text);
            if (mi == null) throw new Exception("Could not find primitive named " + head.Text);
            var args = terms.Nodes.Skip(1).Select(n => ToExpr(n));
            return Expression.Call(mi, args);
        }

        public Expression LetToExpr(Node node)
        {
            var exprs = new List<Expression>();
            foreach (var binding in node[0].Nodes) {
                var name = binding[0].Text;
                var param = AddBinding(Expression.Parameter(typeof(Object), name));
                exprs.Add(param);
                exprs.Add(Expression.Assign(Lookup(name), ToExpr(binding[1])));
            }
            exprs.Add(ToExpr(node["Term"]));
            return Expression.Block(exprs);
        }

        public Expression ToExpr(Node node)
        {
            switch (node.Label)
            {
                case "Integer":
                    return Expression.Constant(int.Parse(node.Text));
                case "Float":
                    return Expression.Constant(double.Parse(node.Text));
                case "String":
                    return Expression.Constant(node.Text.Substring(1, node.Text.Length - 2));
                case "SExpr":
                    return SExprToExpr(node);
                case "Symbol":
                    return Lookup(node.Text);
                case "Param":
                    return Expression.Parameter(typeof(Object), node[0].Text);
                case "Term":
                    return ToExpr(node[0]);
                case "Atom":
                    return ToExpr(node[0]);
                case "Lambda":
                    return CreateExpressionLambda(node["ParamList"].Nodes.Select(ToExpr)
                        .OfType<ParameterExpression>().ToArray(), () => ToExpr(node["Term"]));
                case "Begin":
                    return Expression.Block(node.Nodes.Select(n => ToExpr(n)));
                case "Let":
                    return LetToExpr(node);
                case "If":
                    return node.Count == 3 
                        ? Expression.IfThenElse(ToExpr(node[0]), ToExpr(node[1]), ToExpr(node[2]))
                        : Expression.IfThen(ToExpr(node[0]), ToExpr(node[1]));
                default:
                    throw new Exception("Unrecognized node type " + node.Label);
            }
        }
   }
}
