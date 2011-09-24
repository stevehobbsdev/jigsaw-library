using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw
{
    public class List
    {
        public static List Empty = new List(null, null);

        public dynamic Head { get; private set; }
            
        public dynamic Tail { get; private set; }

        public bool IsEmpty { get { return this == Empty; } }

        private List(dynamic head, dynamic tail) 
        {
            Head = head;
            Tail = tail;
        }

        public IEnumerable<dynamic> Elements
        {
            get 
            {
                for (var iter = this; !iter.IsEmpty; iter = iter.Tail)
                    yield return iter.Head;
            }
        }

        public static object Parse(Node node)
        {
            switch (node.Label)
            {
                case "Integer": return Int32.Parse(node.Text);
                case "String":  return node.Text;
                case "Atom":    return Parse(node[0]);
                case "Term":    return Parse(node[0]);
                case "SExpr":   return Build(node["Terms"].Nodes.Select(Parse));
                case "Symbol":  return node.Text;
                default: throw new Exception("Unrecognized node type " + node.Label);
            }
        }

        public static List Build(IEnumerable<Object> xs)
        {
            var result = Empty;
            foreach (var x in xs)
                result = Cons(x, result);
            return result;
        }

        public static List Unit(Object hd)
        {
            return Cons(hd, Empty);
        }
            
        public static List Cons(Object hd, Object tl)
        {
            if (tl == null) throw new ArgumentNullException();
            return new List(hd, tl);
        }
            
        public static List Parse(string s)
        {
            var nodes = Parser.Parse(s, Grammars.SExpressionGrammar.SExpr);
            if (nodes == null) 
                throw new Exception("Invalid string format");
            var node = nodes[0];
            if (node.Label != "SExpr") 
                throw new Exception("Expected a s-expression node");
            var result = Parse(nodes[0]);
            return (List)result;
        }

        public override string ToString()
        {
            return String.Format("({0})", String.Join(" ", Elements));
        }
    }
}
