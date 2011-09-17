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
            Test("1 2 swap");
            Test("3 4 add");
            Test("3 dup");
            Test("3 dup add");
            Test("[1 2 3] list");
            Test("nil");
            Test("nil 1 cons");
            Test("nil 1 cons 2 cons");
            Test("[1 2 3] list uncons");
        }
    }
}
