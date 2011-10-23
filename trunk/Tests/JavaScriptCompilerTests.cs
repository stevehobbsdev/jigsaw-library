using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw
{
    class JavaScriptCompilerTests
    {
        public static void TestCompile(string s, params object[] args)
        {
            Console.WriteLine("Testing {0} with args ({1})", s, String.Join(", ", args));
            try
            {
                var f = JavaScriptExpressionCompiler.CompileLambda(s);
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
            TestCompile("function(x) { return x; }", 42);
            TestCompile("function() { return 42; }");
            TestCompile("function() { return 41 + 1; }");
            TestCompile("function(x) { return 42; }", 0);
            TestCompile("function(x) { return x; }", 42);
            TestCompile("function(x) { return x + 1; }", 41);
            TestCompile("function() { if (true) return 42; }");
            TestCompile("function (a, b) { return a + b; }", 19, 23);
            TestCompile("function (a, b) { if (a) return b + 1; else return b - 1; }", false, 43);
            TestCompile("function (a, b) { if (a) return b + 1; else return b - 1; }", true, 41);
        }
    }
}
