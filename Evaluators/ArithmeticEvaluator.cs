using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw
{
    class ArithmeticEvaluator
    {
        public static dynamic Eval(string s)
        {
            return Eval(ArithmeticGrammar.Expression.Parse(s)[0]);
        }

        public static dynamic Eval(Node n)
        {
            switch (n.Label)
            {
                case "Number":      return Eval(n[0]);
                case "Integer":     return Int64.Parse(n.Text);
                case "Float":       return Double.Parse(n.Text);
                case "NegatedExpr": return -Eval(n[0]);
                case "ParanExpr":   return Eval(n[0]);
                case "Expression":
                    PrecedenceResolution.SplitLongExpression(n);
                    switch (n.Count)
                    {
                        case 1: 
                            return Eval(n[0]);
                        case 3: 
                            switch (n[1].Text)
                            {
                                case "+": return Eval(n[0]) + Eval(n[2]);
                                case "-": return Eval(n[0]) - Eval(n[2]);
                                case "*": return Eval(n[0]) * Eval(n[2]);
                                case "/": return Eval(n[0]) / Eval(n[2]);
                                case "%": return Eval(n[0]) % Eval(n[2]);
                                case "<<": return Eval(n[0]) << Eval(n[2]);
                                case ">>": return Eval(n[0]) >> Eval(n[2]);
                                case "==": return Eval(n[0]) == Eval(n[2]);
                                case "!=": return Eval(n[0]) != Eval(n[2]);
                                case "<=": return Eval(n[0]) <= Eval(n[2]);
                                case ">=": return Eval(n[0]) >= Eval(n[2]);
                                case "<": return Eval(n[0]) < Eval(n[2]);
                                case ">": return Eval(n[0]) > Eval(n[2]);
                                case "&&": return Eval(n[0]) && Eval(n[2]);
                                case "||": return Eval(n[0]) || Eval(n[2]);
                                case "&": return Eval(n[0]) & Eval(n[2]);
                                case "|": return Eval(n[0]) | Eval(n[2]);
                                default: throw new Exception("Unreocognized operator " + n[1].Text);
                            }
                        default: 
                            throw new Exception(String.Format("Unexpected number of nodes {0} in expression", n.Count));
                    }
                default:
                    throw new Exception("Unexpected type of node " + n.Label);
            }
        }
    }
}
