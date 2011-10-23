using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Diggins.Jigsaw
{
    public class JavaScriptExpressionCompiler : ExpressionCompiler
    {
        public static Delegate CompileLambda(string s)
        {
            var nodes = JavaScriptGrammar.AnonFunc.Parse(s);
            return CompileLambda(nodes[0]);
        }

        public static Delegate CompileLambda(Node n)
        {
            n = JavaScriptTransformer.Transform(n);
            var compiler = new JavaScriptExpressionCompiler();
            var expr = (LambdaExpression)compiler.ToExpr(n);
            if (expr == null) return null;
            return expr.Compile();
        }

        /// <summary>
        /// The node has to be transformed before being passed
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public Expression ToExpr(Node n)
        {
            switch (n.Label)
            {
                case "Empty":
                    return Noop;
                case "Return":
                    return (n.Count > 0)
                        ? Expression.Return(GetReturnTarget(), ToExpr(n[0]), typeof(Object))
                        : Expression.Return(GetReturnTarget(), Expression.Constant(typeof(Object), null));
                case "If":
                    return n.Count > 2
                        ? Expression.IfThenElse(Expression.Convert(ToExpr(n[0]), typeof(Boolean)), ToExpr(n[1]), ToExpr(n[2]))
                        : Expression.IfThen(Expression.Convert(ToExpr(n[0]), typeof(Boolean)), ToExpr(n[1]));
                case "Else":
                    return ToExpr(n[0]);
                case "For":                    
                    return CompileForLoop(ToExpr(n[0]), ToExpr(n[1]), ToExpr(n[2]), ToExpr(n[3]));
                case "VarDecl":
                    {
                        var r = AddBinding(Expression.Variable(typeof(object), n[0].Text));
                        if (n.Count > 1)
                            r = Expression.Assign(r, ToExpr(n[1]));
                        return r;
                    }
                case "Block":
                    return ScopedExpr(() => Expression.Block(n.Nodes.Select(ToExpr)));
                case "TertiaryExpr":
                    return Expression.Condition(ToExpr(n[0]), ToExpr(n[1]), ToExpr(n[2]));
                case "BinaryExpr":
                    return Expression.Call(null, Primitives.GetMethodFromBinaryOperator(n[1].Text), ToExpr(n[0]), ToExpr(n[2]));
                case "AssignExpr":
                    return Expression.Assign(ToExpr(n[0]), ToExpr(n[2]));
                case "FieldExpr":
                    return Expression.Field(ToExpr(n[0]), n[1].Text);
                case "IndexExpr":
                    return CompileIndexExpr(ToExpr(n[0]), ToExpr(n[2]));
                case "CallExpr":
                    return CompileCallExpr(ToExpr(n[0]), n[1].Nodes.Select(ToExpr));
                case "MethodCallExpr":
                    {
                        var self = ToExpr(n[0]);
                        var func = Expression.Field(self, n[1].Text);
                        var args = new List<Expression>() { self };
                        args.AddRange(n[2].Nodes.Select(ToExpr));
                        return CompileCallExpr(func, args);
                    }
                case "PrefixExpr":
                    switch (n[0].Text)
                    {
                        case "!": return Expression.Not(ToExpr(n[1]));
                        case "~": return Expression.OnesComplement(ToExpr(n[1]));
                        case "-": return Expression.Negate(ToExpr(n[1]));
                        default: throw new Exception("Unrecognized prefix operator " + n[0].Text);
                    }
                case "NewExpr":
                    return ScopedExpr(() => 
                        {
                            AddBinding("this", Expression.Constant(new JsonObject()));
                            return ToExpr(n[0]);
                        });                    
                case "ParenExpr":
                    return ToExpr(n[0]);
                case "AnonFunc":
                    {
                        var ps = n[0].Nodes.Select(x => Expression.Parameter(typeof(Object), x.Text));
                        return CreateStatementLambda(ps, () => ToExpr(n[1]));
                    };
                case "Object":
                    throw new NotImplementedException();
                case "Array":
                    return Expression.NewArrayInit(typeof(object), n.Nodes.Select(ToExpr));
                case "Identifier":
                    return Lookup(n.Text);
                case "String":
                    return Expression.Constant(Utilities.Unquote(n.Text));
                case "Integer":
                    return Expression.Convert(Expression.Constant(int.Parse(n.Text)), typeof(Object));
                case "Float":
                    return Expression.Convert(Expression.Constant(float.Parse(n.Text)), typeof(Object));
                case "True":
                    return Expression.Convert(Expression.Constant(true), typeof(Object));
                case "False":
                    return Expression.Convert(Expression.Constant(false), typeof(Object));
                case "Null":
                    return Expression.Constant(null);
                default:
                    throw new Exception("Unrecognized node type " + n.Label);
            }
        }
    }
}
