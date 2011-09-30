using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Diggins.Jigsaw.Tests
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
            TestParse("42", Grammars.CSharpExprGrammar.Integer);
            TestParse("42", Grammars.CSharpExprGrammar.Literal);
            TestParse("42", Grammars.CSharpExprGrammar.LeafExpr);
            TestParse("42", Grammars.CSharpExprGrammar.UnaryExpr);
            TestParse("42", Grammars.CSharpExprGrammar.BinaryExpr);
            TestParse("42", Grammars.CSharpExprGrammar.TertiaryExpr);
            TestParse("42", Grammars.CSharpExprGrammar.Expr);

            TestParse("x", Grammars.CSharpExprGrammar.Identifier);
            TestParse("x", Grammars.CSharpExprGrammar.LeafExpr);
            TestParse("x", Grammars.CSharpExprGrammar.UnaryExpr);
            TestParse("x", Grammars.CSharpExprGrammar.BinaryExpr);
            TestParse("x", Grammars.CSharpExprGrammar.TertiaryExpr);
            TestParse("x", Grammars.CSharpExprGrammar.Expr);

            TestParse("\"hello world\"", Grammars.CSharpExprGrammar.String);
            TestParse("\"hello world\"", Grammars.CSharpExprGrammar.LeafExpr);
            TestParse("\"hello world\"", Grammars.CSharpExprGrammar.UnaryExpr);
            TestParse("\"hello world\"", Grammars.CSharpExprGrammar.BinaryExpr);
            TestParse("\"hello world\"", Grammars.CSharpExprGrammar.TertiaryExpr);
            TestParse("\"hello world\"", Grammars.CSharpExprGrammar.Expr);
            TestParse("() => \"hello world\"", Grammars.CSharpExprGrammar.LambdaExpr);

            TestParse("x", Grammars.CSharpExprGrammar.LambdaParam);
            TestParse("x", Grammars.CSharpExprGrammar.LambdaParams);
            TestParse("()", Grammars.CSharpExprGrammar.LambdaParams);
            TestParse("(x)", Grammars.CSharpExprGrammar.LambdaParams);
            TestParse("() => x", Grammars.CSharpExprGrammar.LambdaExpr);
            TestParse("x => x", Grammars.CSharpExprGrammar.LambdaExpr);

            TestParse("1 + 2", Grammars.CSharpExprGrammar.BinaryExpr);
            TestParse("(1)", Grammars.CSharpExprGrammar.ParenthesizedExpr);
            TestParse("(1 + 2)", Grammars.CSharpExprGrammar.ParenthesizedExpr);
            TestParse("(1 + 2) * 3", Grammars.CSharpExprGrammar.BinaryExpr);
            TestParse("1 + (2 * 3)", Grammars.CSharpExprGrammar.BinaryExpr);
            TestParse("3 * (1 + 2)", Grammars.CSharpExprGrammar.BinaryExpr);

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
