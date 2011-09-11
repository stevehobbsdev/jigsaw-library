using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw
{
    public class Unifier
    {
        public static bool IsConstant(object e)
        {
            return !(e is List) && !(e is string);
        }

        public static bool Occurs(string s, List e)
        {
            if (e.Head is string)
                if (e.Head == s) 
                    return true;
            if (e.Head is List) 
                return Occurs(s, e.Head);
            return e.Tail == null ? false : Occurs(s, e.Tail);
        }

        public static dynamic Substitute(Dictionary<string, dynamic> bindings, dynamic x)
        {
            if (x is String && bindings.ContainsKey(x)) 
                return bindings[x];
            if (x is List && !x.IsEmpty)
                return List.Cons(Substitute(bindings, x.Head), Substitute(bindings, x.Tail));
            return x;
        }

        public static Dictionary<string, object> Unify(dynamic e1, dynamic e2)
        {
            if ((IsConstant(e1) && IsConstant(e2)))
            {
                if (e1 == e2)
                    return new Dictionary<string,object>();
                throw new Exception("Unification failed");
            }

            if (e1 is string)
            {
                if (e2 is List && Occurs(e1, e2))
                    throw new Exception("Cyclical binding");
                return new Dictionary<string, object>() { { e1, e2 } };
            }

            if (e2 is string)
            {
                if (e1 is List && Occurs(e2, e1))
                    throw new Exception("Cyclical binding");
                return new Dictionary<string, object>() { { e2, e1 } };
            }

            if (!(e1 is List) || !(e2 is List))
                throw new Exception("Expected either list, string, or constant arguments");

            if (e1.IsEmpty || e2.IsEmpty)
            {
                if (!e1.IsEmpty || !e2.IsEmpty)
                    throw new Exception("Lists are not the same length");

                return new Dictionary<string, object>(); 
            }

            var b1 = Unify(e1.Head, e2.Head);
            var b2 = Unify(Substitute(b1, e1.Tail), Substitute(b1, e2.Tail));

            foreach (var kv in b2)
                b1.Add(kv.Key, kv.Value);
            return b1;
        }
    }
}
