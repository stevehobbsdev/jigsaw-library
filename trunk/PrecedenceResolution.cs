using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Diggins.Jigsaw
{
    class PrecedenceResolution
    {
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

        public static void SplitLongExpression(Node node, int pivot)
        {
            Node left = new Node("Expression", node.Nodes.Take(pivot).ToArray());
            Node right = new Node("Expression", node.Nodes.Skip(pivot + 1).ToArray());
            SplitLongExpression(left);
            SplitLongExpression(right);
            node.Nodes = new List<Node>() { left, node[pivot], right };
        }

        public static void SplitLongExpression(Node node)
        {
            if (node.Label != "Expression" || node.Count <= 3)
                return;
            
            // Long expressions should always have an odd number of nodes.
            Debug.Assert(node.Count % 2 == 1);

            // Find the lowest priority operation 
            int pivot = 1;
            for (int i = 3; i < node.Count; i += 2)
                if (Precendence(node[i].Text) < Precendence(node[pivot].Text))
                    pivot = i;

            SplitLongExpression(node, pivot);
            Debug.Assert(node.Count == 3);
        }
    }
}
