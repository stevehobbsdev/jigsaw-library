using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Diggins.Jigsaw
{
    /// <summary>
    /// Simplifies the parse tree for evaluation.
    /// </summary>
    public class JavaScriptTransformer : TreeTransformer
    {
        public static Node Transform(Node n)
        {
            return new JavaScriptTransformer().TransformAlNodes(n);
        }

        Node ToAssignment(Node left, string op, Node right)
        {
            var assignOp = new Node("AssignOp", "=");
            var binOp = new Node("Binaryop", op);
            var binExpr = new Node("BinaryExpr", left, binOp, right);
            return new Node("AssignExpr", left, assignOp, binExpr);
        }

        protected override Node InternalTransform(Node n)
        {
            switch (n.Label)
            {
                case "Literal":
                    return n[0];
                case "LeafExpr":
                    return n[0];
                case "ParenExpr":
                    return n[0];
                case "Expr":
                    return n[0];
                case "Statement":
                    return n[0];
                case "ExprStatement":
                    return n[0];
                case "While":
                    // While loops are a special case of for loops.
                    return new Node("For", new Node("Empty"), n[0], new Node("Empty"), n[1]);
                case "PostfixExpr":
                    {
                        Debug.Assert(n.Count != 0);
                        // Is it really a postfix expression? If not return the sub-expression
                        if (n.Count == 1)
                            return n.Nodes[0];
                        var last = n.Nodes.Last();
                        switch (last.Label)
                        {
                            case "Field":
                                return LeftGroup(n, "FieldExpr");
                            case "Index":
                                return LeftGroup(n, "IndexExpr");
                            case "ArgList":
                                {
                                    var call = LeftGroup(n, "CallExpr");

                                    // Method calls have special semantics.
                                    // You know it is a method if the left side of a call is a field express 
                                    if (call[0].Label == "FieldExpr")
                                    {
                                        var obj = call[0][0];
                                        var field = call[0][1];
                                        var args = call[1];
                                        return new Node("MethodCallExpr", obj, field, args);
                                    }
                                    else
                                    {
                                        return call;
                                    }
                                }
                            default: throw new Exception("Unexpected node in postfix-expression " + last.Label);
                        }
                    }
                case "NamedFunc":
                    return new Node("VarDecl", n[0], new Node("AnonFunc", n[1], n[2]));
                case "TertiaryExpr":
                    // Is not really a tertiary expression return the sub-expression
                    return (n.Count == 1) ? n[0] : n;
                case "BinaryExpr":
                    // Is not really a binary expression return the sub-expression
                    return (n.Count == 1) 
                        ? n[0] 
                        : SplitBinaryExpression(n);
                case "AssignExpr":
                    {
                        // Is it really an assignment expression? If not return the sub-expressions
                        if (n.Count == 1)
                            return n[0];
                        // Transform special assignement operators into plain assignment and binary operation
                        switch (n[1].Text)
                        {
                            case "=": return n;
                            case "+=": return ToAssignment(n[0], "+", n[2]);
                            case "-=": return ToAssignment(n[0], "-", n[2]);
                            case "*=": return ToAssignment(n[0], "*", n[2]);
                            case "/=": return ToAssignment(n[0], "/", n[2]);
                            case "%=": return ToAssignment(n[0], "%", n[2]);
                            case "|=": return ToAssignment(n[0], "|", n[2]);
                            case "&=": return ToAssignment(n[0], "&", n[2]);
                            case "^=": return ToAssignment(n[0], "^", n[2]);
                            case "||=": return ToAssignment(n[0], "||", n[2]);
                            case "&&=": return ToAssignment(n[0], "&&", n[2]);
                            case ">>=": return ToAssignment(n[0], ">>", n[2]);
                            case "<<=": return ToAssignment(n[0], "<<", n[2]);
                            default:
                                throw new Exception("Unexpected assignment operator " + n[1].Text);
                        }
                    }
            }
            return n;
        }

        public static int Precendence(string op)
        {
            switch (op)
            {
                case "=":
                case "+=":
                case "-=":
                case "*=":
                case "/=":
                case "%=":
                case ">>=":
                case "<<=":
                case "|=":
                case "&=":
                case "^=":
                case "||=":
                case "&&=":
                    return 10;
                case "||":
                    return 20;
                case "&&":
                    return 30;
                case "|":
                    return 40;
                case "^":
                    return 50;
                case "&":
                    return 60;
                case "==":
                case "!=":
                    return 70;
                case ">=":
                case "<=":
                case ">":
                case "<":
                    return 80;
                case ">>":
                case "<<":
                    return 90;
                case "+":
                case "-":
                    return 100;
                case "*":
                case "/":
                case "%":
                    return 110;
                default:
                    throw new Exception("Not a recognized operator");
            }
        }

        public static Node SplitBinaryExpression(Node node)
        {
            if (node.Count <= 3) return node;

            // Long expressions should always have an odd number of nodes.
            Debug.Assert(node.Count % 2 == 1);

            // Find the lowest priority operation 
            int pivot = 1;
            for (int i = 3; i < node.Count; i += 2)
                if (Precendence(node[i].Text) < Precendence(node[pivot].Text))
                    pivot = i;

            Node left = new Node("BinaryExpr", node.Nodes.Take(pivot).ToArray());
            Node right = new Node("BinaryExpr", node.Nodes.Skip(pivot + 1).ToArray());
            return new Node("BinaryExpr", SplitBinaryExpression(left), node[pivot], SplitBinaryExpression(right));
        }
    }
}
