using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw.Parsing
{
    public class TreeVisitor
    {
        protected virtual bool Visit(Node n)
        {
            return true;
        }

        public bool VisitNodes(Node n)
        {
            if (!Visit(n)) return false;
            return n.Nodes.All(VisitNodes);
        }
    }
}
