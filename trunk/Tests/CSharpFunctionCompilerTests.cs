using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Diggins.Jigsaw
{
    public class CSharpFunctionCompilerTests
    {
        public static void TestParse(string s, Rule r)
        {
            GrammarTest.Test(s, r);
        }

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
            TestParse("42", CSharpExprGrammar.Integer);
            TestParse("42", CSharpExprGrammar.Literal);
            TestParse("42", CSharpExprGrammar.LeafExpr);
            TestParse("42", CSharpExprGrammar.UnaryExpr);
            TestParse("42", CSharpExprGrammar.BinaryExpr);
            TestParse("42", CSharpExprGrammar.TertiaryExpr);
            TestParse("42", CSharpExprGrammar.Expr);

            TestParse("x", CSharpExprGrammar.Identifier);
            TestParse("x", CSharpExprGrammar.LeafExpr);
            TestParse("x", CSharpExprGrammar.UnaryExpr);
            TestParse("x", CSharpExprGrammar.BinaryExpr);
            TestParse("x", CSharpExprGrammar.TertiaryExpr);
            TestParse("x", CSharpExprGrammar.Expr);

            TestParse("\"hello world\"", CSharpExprGrammar.String);
            TestParse("\"hello world\"", CSharpExprGrammar.LeafExpr);
            TestParse("\"hello world\"", CSharpExprGrammar.UnaryExpr);
            TestParse("\"hello world\"", CSharpExprGrammar.BinaryExpr);
            TestParse("\"hello world\"", CSharpExprGrammar.TertiaryExpr);
            TestParse("\"hello world\"", CSharpExprGrammar.Expr);
            TestParse("() => \"hello world\"", CSharpExprGrammar.LambdaExpr);

            TestParse("x", CSharpExprGrammar.LambdaParam);
            TestParse("x", CSharpExprGrammar.LambdaParams);
            TestParse("()", CSharpExprGrammar.LambdaParams);
            TestParse("(x)", CSharpExprGrammar.LambdaParams);
            TestParse("() => x", CSharpExprGrammar.LambdaExpr);
            TestParse("x => x", CSharpExprGrammar.LambdaExpr);

            TestParse("1 + 2", CSharpExprGrammar.BinaryExpr);
            TestParse("(1)", CSharpExprGrammar.ParenthesizedExpr);
            TestParse("(1 + 2)", CSharpExprGrammar.ParenthesizedExpr);
            TestParse("(1 + 2) * 3", CSharpExprGrammar.BinaryExpr);
            TestParse("1 + (2 * 3)", CSharpExprGrammar.BinaryExpr);
            TestParse("3 * (1 + 2)", CSharpExprGrammar.BinaryExpr);

            Expression<Func<string,string>> f = (s) => s + "world";

            Test("() => 42");
            Test("() => 41 + 1");
            Test("x => x", 42);
            Test("x => 0", 42);
            Test("(int x) => x + 1", 14);
            Test("(int a, int b) => a + b", 19, 23);
            Test("(int a, int b) => (a + a) * b", 3, 7);
            Test("() => { return 42; }");
            Test("() => \"hello world\"");
        }
    }
}
