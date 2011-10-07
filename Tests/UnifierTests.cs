using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw
{
    class UnifierTests
    {
        public static void PrintBindings(List e1, List e2)
        {
            var bindings = Unifier.Unify(e1, e2);
            foreach (var kv in bindings)
                Console.WriteLine("Variable = {0}, Value = {1}", kv.Key, kv.Value);
        }

        public static void PrintBindings(string s1, string s2)
        {
            PrintBindings(List.Parse(s1), List.Parse(s2));
        }

        public static void Tests()
        {
            PrintBindings("(forty_two)", "(42)");
            PrintBindings("(is_c b42)", "(c 42)");
            PrintBindings("()", "()");
            PrintBindings("(42)", "(a42)");
            PrintBindings("((a b) 3)", "((1 2) c)");
            PrintBindings("(list x)", "((1 2 3) 42)");
        }
    }
}
