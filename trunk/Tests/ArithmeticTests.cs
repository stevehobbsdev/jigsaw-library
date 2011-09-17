using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw.Tests
{
    class ArithmeticTests
    {
        public static void Test(string s)
        {
            try
            {
                Console.Write("Running test {0}, result = ", s);
                var result = ArithmeticEvaluator.Eval(s);
                Console.WriteLine(result);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occured " + e.Message);
            }
        }

        public static void Tests()
        {
            Test("12");
            Test("1 + 2");
            Test("1 + 2 + 3");
            Test("1 + 2 * 3");
            Test("1 * 2 + 3");
            Test("1 * 2 + 3 * 4");
            Test("1 + 2 * 3 + 4");
            Test("1 * 2 * 3 + 4");
            Test("1 + 2 * 3 * 4");
            Test("(1 + 2) * 3");
            Test("10 - 9 - 8");
            Test("(10 - 9) - 8");
            Test("10 - (9 - 8)");
            Test("10 - 9 - 8 - 7");
            Test("-12");
            Test("--12");
            Test("---12");
            Test("-(12)");
            Test("(12)");
            Test("12 / 4");
            Test("12 % 4");
            Test("12 % 5");
        }
    }
}
