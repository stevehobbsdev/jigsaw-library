using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw.Tests
{
    using Grammars;

    public class SExprTests
    {
        public static void TestParse(string s, Rule r)
        {
            Console.WriteLine("Parsing: " + s);
            var nodes = Parser.Parse(s, r);
            if (nodes == null)
                Console.WriteLine("Parsing failed!");
            else
                Console.WriteLine("Parsing suceeded");
        }

        public static dynamic LambdaTest(string s)
        {
            var node = Parser.Parse(s, SExprGrammar.Lambda).First();
            return SExprLambdaCompiler.NodeToLambda(node).Compile();
        }

        public static void Test()
        {
            TestParse("a", SExprGrammar.Symbol);
            TestParse("a123", SExprGrammar.Symbol);
            TestParse("_", SExprGrammar.Symbol);
            TestParse(" ", SExprGrammar.WS);
            TestParse("\t\t", SExprGrammar.WS);
            TestParse("123", SExprGrammar.Integer);
            TestParse("0", SExprGrammar.Integer);
            TestParse("a", SExprGrammar.Atom);
            TestParse("12", SExprGrammar.Atom);
            TestParse("a", SExprGrammar.Term);
            TestParse("12", SExprGrammar.Term);
            TestParse("a)", SExprGrammar.Term);
            TestParse(")", SExprGrammar.Term);
            TestParse("(a)", SExprGrammar.SExpr);
            TestParse("( a)", SExprGrammar.SExpr);
            TestParse("(a )", SExprGrammar.SExpr);
            TestParse("( a )", SExprGrammar.SExpr);
            TestParse("(a b)", SExprGrammar.SExpr);
            TestParse("()", SExprGrammar.SExpr);
            TestParse("((a b) c)", SExprGrammar.SExpr);
            TestParse("(c (a b))", SExprGrammar.SExpr);
            TestParse("(c (a 12) \"hello\")", SExprGrammar.SExpr);
            TestParse("(+ 1 2)", SExprGrammar.SExpr);
            TestParse("(lambda())", SExprGrammar.Lambda);
            TestParse("(lambda () )", SExprGrammar.Lambda);
            TestParse("(lambda ( ) )", SExprGrammar.Lambda);
            TestParse("(lambda (a) )", SExprGrammar.Lambda);
            TestParse("(lambda (a) a)", SExprGrammar.Lambda);
            TestParse("(lambda (a b) a)", SExprGrammar.Lambda);
            TestParse("(lambda (a b) 12)", SExprGrammar.Lambda);
            TestParse("(lambda (a b) (Add a b))", SExprGrammar.Lambda);

            Console.WriteLine(LambdaTest("(lambda (a b) (Add a b))")(3, 4));
        }
    }
}
