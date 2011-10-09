using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw
{
    public class TreeTransformer
    {
        protected virtual Node Transform(Node n) { return n; }

        public Node TransformNodes(Node n)
        {
            n.Nodes = n.Nodes.Select(TransformNodes).ToList();
            return Transform(n);
        }

        public Node LeftGroup(Node n, string leftLabel)
        {
            var leftChild = Transform(new Node(n.Label, n.Nodes.Take(n.Count - 1)));
            return new Node(leftLabel, leftChild, n.Nodes.Last());
        }

        public static bool IsNthChild(Node node, int n, string label)
        {
            if (node.Count <= n) return false;
            return node[n].Label == label;
        }

        public static bool IsLastChild(Node n, string label)
        {
            if (n.Count == 0) return false;
            return IsNthChild(n, n.Count - 1, label);
        }

        public static bool IsFirstChild(Node n, string label)
        {
            return IsNthChild(n, 0, label);
        }

        public static bool HasChild(Node n, string label)
        {
            return n.Nodes.Any(x => x.Label == label);
        }
    }
}
