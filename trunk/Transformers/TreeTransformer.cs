using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw
{
    public class TreeTransformer
    {
        protected virtual Node InternalTransform(Node n) { return n; }

        protected Node TransformAlNodes(Node n)
        {
            n.Nodes = n.Nodes.Select(TransformAlNodes).ToList();
            return InternalTransform(n);
        }

        protected Node LeftGroup(Node n, string leftLabel)
        {
            var leftChild = InternalTransform(new Node(n.Label, n.Nodes.Take(n.Count - 1)));
            return new Node(leftLabel, leftChild, n.Nodes.Last());
        }

        protected static bool IsNthChild(Node node, int n, string label)
        {
            if (node.Count <= n) return false;
            return node[n].Label == label;
        }

        protected static bool IsLastChild(Node n, string label)
        {
            if (n.Count == 0) return false;
            return IsNthChild(n, n.Count - 1, label);
        }

        protected static bool IsFirstChild(Node n, string label)
        {
            return IsNthChild(n, 0, label);
        }

        protected static bool HasChild(Node n, string label)
        {
            return n.Nodes.Any(x => x.Label == label);
        }
    }
}
