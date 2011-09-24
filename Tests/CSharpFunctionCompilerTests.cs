using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw.Tests
{
    public class CSharpFunctionCompilerTests
    {
        public static void Test(string s, params object[] args)
        {
            Console.WriteLine("Testing {0} with args ({1})", s, String.Join(", ", args));
            try
            {
                var f = CSharpFunctionCompiler.CompileLambda(s);
                var r = f.DynamicInvoke(args);
                Console.WriteLine("Result is {0}", r);
            }
            catch (Exception e)
            {
                Console.WriteLine("error occured " + e.Message);
            }
        }

        public static void Tests()
        {
            Test("() => 42");
            Test("() => 41 + 1");
            Test("x => x", 42);
            Test("x => x + 1", 41);
            Test("(x) => x + 1", 14);
            Test("(a, b) => a + b", 19, 23);
            Test("(a, b) => (a + a) * b", 3, 7);
            Test("() => { return 42; }");
            Test("() => \"hello world\"");
            Test("() => \"hello\" + \"world\"");
            Test("(a, b) => a + b", "hello ", "world");
        }
    }
}
