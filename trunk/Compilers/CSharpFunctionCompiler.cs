using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace Diggins.Jigsaw
{
    public class CSharpFunctionCompiler : ExpressionCompiler
    {
        public static Delegate CompileLambda(string s)
        {
            var nodes = Parser.Parse(s, Grammars.CSharpExprGrammar.LambdaExpr);
            return CompileLambda(nodes[0]);
        }

        public static Delegate CompileLambda(Node n)
        {
            var compiler = new CSharpFunctionCompiler();
            var expr = compiler.ToExpr(n) as LambdaExpression;
            if (expr == null) return null;
            return expr.Compile();
        }

        public Expression ToExpr(Node n)
        {
            switch (n.Label)
            {
                case "Expression":
                    return ToExpr(n[0]);
                case "TertiaryExpr":
                    if (n.Count > 1)
                        return Expression.Condition(ToExpr(n[0]), ToExpr(n[1]), ToExpr(n[2]));
                    else
                        return ToExpr(n[0]);    
                case "BinaryExpr":
                    // TODO: deal with binary operations
                    return ToExpr(n[0]);
                case "UnaryExpr":
                    {
                        var x = ToExpr(n[0]);
                        for (int i = 1; i < n.Count; ++i)
                        {
                            switch (n[i].Label)
                            {
                                case "Inc":         
                                    x = Expression.Increment(x); 
                                    break;
                                case "Dec":         
                                    x = Expression.Decrement(x); 
                                    break;
                                case "Indexer":     
                                    x = Expression.ArrayIndex(x, ToExpr(n[i][0])); 
                                    break;
                                case "ArgList":
                                    x = Expression.Invoke(x, n[1].Nodes.Select(ToExpr)); 
                                    break;
                                case "Selector":    
                                    x = Expression.Field(x, n[i][0].Text); 
                                    break;
                                default: 
                                    throw new Exception("Unrecognized postfix operator " + n[i].Label);
                            }
                        }
                        return x;
                    }
                case "PrefixExpr":
                    if (n[0].Label == "LeafExpr") 
                        return ToExpr(n[0]);
                    switch (n[0].Text)
                    {
                        case "++":  
                            return Expression.PreIncrementAssign(ToExpr(n[1]));                                
                        case "--":  
                            return Expression.PreDecrementAssign(ToExpr(n[1]));
                        case "!":   
                            return Expression.Not(ToExpr(n[1]));
                        case "-":   
                            return Expression.Negate(ToExpr(n[1]));
                        case "~":   
                            return Expression.OnesComplement(ToExpr(n[1]));
                        default:    
                            throw new Exception("Unrecognized prefix operator " + n[0].Text);
                    }
                case "LeafExpr":
                    return ToExpr(n[0]);
                case "ParenthesizedExpr":
                    return ToExpr(n[0]);
                case "NewExpr":
                    throw new NotImplementedException();
                case "Identifier":
                    return (Expression)Context[n.Text];
                case "Integer":
                    return Expression.Constant(Int32.Parse(n.Text));
                case "Float":
                    return Expression.Constant(Double.Parse(n.Text));
                case "String":
                    return Expression.Constant(n.Text.StripQuotes());
                case "LambdaParam":
                    if (n.Count > 1)
                        return Expression.Parameter(Type.GetType(n[0].Text), n[1].Text);
                    else
                        return Expression.Parameter(typeof(Object), n[0].Text);
                case "LambdaExpr":
                    return Expression.Lambda(ToExpr(n[1]), n[0].Nodes.Select(ToExpr).OfType<ParameterExpression>());
                default:
                    throw new Exception("Node type not handled " + n.Label);
            }
        }
    }
}
