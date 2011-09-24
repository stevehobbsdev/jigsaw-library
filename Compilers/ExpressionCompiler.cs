using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Diggins.Jigsaw
{
    public class ExpressionCompiler
    {
        public Context Context = new Context();

        public static Expression Noop
        {
            get { return Expression.Call(null, typeof(Primitives).GetMethod("noop")); }
        }

        public static void CheckNode(Node node, string name)
        {
            if (node.Label != name)
                throw new Exception(String.Format("Expected node of type {0} not {1}", name, node.Label));
        }

    }
}
