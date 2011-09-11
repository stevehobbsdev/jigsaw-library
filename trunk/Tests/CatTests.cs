using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw.Tests
{
    public class CatTests
    {
        public static void Print(IEnumerable<object> xs)
        {
            Console.WriteLine("stack: " + String.Join(" ", xs));
        }

        public static void Test(string s)
        {
            Console.WriteLine("evaluating " + s);
            var stk = CatEvaluator.Eval(s);
            Print(stk);
        }

        public static void Tests()
        {
            Test("1");
            Test("1 2");
            Test("1 2 Swap");
            Test("3 4 Add");
            Test("3 Dup");
            Test("3 Dup Add");
            Test("[1 2 3] MakeList");
            Test("Nil");
            Test("Nil 1 Cons");
            Test("Nil 1 Cons 2 Cons");
            Test("[1 2 3] MakeList Uncons");
        }
    }
}
