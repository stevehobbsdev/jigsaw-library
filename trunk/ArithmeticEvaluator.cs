using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw
{
    class ArithmeticEvaluator
    {
        public static dynamic Eval(Node node)
        {
            switch (node.Label)
            {
                case "Number":      return Eval(node[0]);
                case "Integer":     return Int64.Parse(node.Text);
                case "Float":       return Double.Parse(node.Text);
                case "NegatedExpr": return -Eval(node[0]);
                case "ParanExpr":   return Eval(node[0]);
                case "Expression":
                    PrecedenceResolution.SplitLongExpression(node);
                    switch (node.Count)
                    {
                        case 1: 
                            return Eval(node[0]);
                        case 3: 
                            switch (node[1].Text)
                            {
                                case "+": return Eval(node[0]) + Eval(node[2]);
                                case "-": return Eval(node[0]) - Eval(node[2]);
                                case "*": return Eval(node[0]) * Eval(node[2]);
                                case "/": return Eval(node[0]) / Eval(node[2]);
                                case "%": return Eval(node[0]) % Eval(node[2]);
                                default: throw new Exception("Unreocognized operator " + node[1].Text);
                            }
                        default: 
                            throw new Exception(String.Format("Unexpected number of nodes {0} in expression", node.Count));
                    }
                default:
                    throw new Exception("Unexpected type of node " + node.Label);
            }
        }

        public static dynamic Eval(string s)
        {
            return Eval(Parser.Parse(s, Grammars.ArithmeticGrammar.Expression).First());
        }
    }
}
