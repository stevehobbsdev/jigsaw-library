using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw
{
    public class JavaScriptSourcePrinter : Printer
    {
        public static string ToString(Node n)
        {
            return new JavaScriptSourcePrinter().Print(n).GetStringBuilder().ToString();
        }

        public override Printer Print(Node n)
        {
            switch (n.Label)
            {
                case "Script":
                    return Print(n.Nodes);
                case "Statement":
                    return Print(n[0]);
                case "Empty":
                    return Print(";");
                case "Return":
                    return (n.Count > 0) 
                        ? Print("return ").Print(n[0]).Print(";")
                        : Print("return;");
                case "ExprStatement":
                    return Print(n[0]).Print(";");
                case "If":
                    Print("if (").Print(n[0]).Print(")").Indent().Print(n[1]).Unindent();
                    return n.Count > 2 
                        ? Print(n[2]) 
                        : this;
                case "Else":
                    return Print("else").Indent().Print(n[0]).Unindent();
                case "For":
                    return Print("for (").Print(n[0]).Print(";").Print(n[1]).Print(";").Print(n[2]).Print(") ").Indent().Print(n[3]).Unindent();
                case "While":
                    return Print("while (").Print(n[0]).Print(") ") .Indent().Print(n[1]).Unindent();
                case "VarDecl":
                    return (n.Count > 1)
                        ? Print("var ").Print(n[0]).Print(" = ").Print(n[1]).Print(";")
                        : Print("var ").Print(n[0]).Print(";");
                case "Block":
                    if (n.Count > 0)
                    {
                        Print("{").Indent();
                        foreach (var n2 in n.Nodes.Take(n.Count - 1))
                            PrintLine(n2);
                        return Print(n[n.Count - 1]).Unindent().Print("}");
                    }
                    else
                    {
                        return Print("{}");
                    }
                case "Expr":
                    return Print(n[0]);
                case "TertiaryExpr":
                    return (n.Count == 3)
                        ? Print(n[0]).Print(" ? ").Print(n[1]).Print(" : ").Print(n[2])
                        : Print(n[0]);
                case "AssignOp":
                case "BinaryOp":
                    return Print(" ").Print(n.Text).Print(" ");
                case "PostfixOp":
                case "PrefixOp":
                    return Print(n.Text);
                case "BinaryExpr":
                case "AssignExpr":
                case "PostfixExpr":
                    return Print(n.Nodes);
                case "NewExpr":
                    return Print("new ").Print(n.Nodes);
                case "ParenExpr":
                    return Print("(").Print(n[0]).Print(")");
                case "Field":
                    return Print(".").Print(n.Nodes);
                case "Index":
                    return Print("[").Print(n.Nodes).Print("]");
                case "ArgList":
                    return Print("(").Print(n.Nodes, ", ").Print(")");
                case "NamedFunc":
                    return Print("function ").Print(n[0]).Print(n[1]).Print(" ").Indent().Print(n[2]).Unindent();
                case "AnonFunc":
                    return Print("function ").Print(n[0]).Print(" ").Indent().Print(n[1]).Unindent();
                case "ParamList":
                    return Print("(").Print(n.Nodes, ", ").Print(")");
                case "Object":
                    return Print("{").Indent().Print(n.Nodes, ", ").Unindent().Print("}");
                case "Array":
                    return Print("[").Indent().Print(n.Nodes, ", ").Unindent().Print("]");
                case "Pair":
                    return Print(n[0]).Print(" : ").Print(n[1]);
                case "PairName":
                    return Print(n[0]);
                case "Literal":
                    return Print(n[0]);
                case "Identifier":
                case "String":
                case "Integer":
                case "Float":
                case "True":
                case "False":
                case "Null":
                    return Print(n.Text);
                default:
                    throw new Exception("Unrecognized node type " + n.Label);
            }
        }
    }
}
