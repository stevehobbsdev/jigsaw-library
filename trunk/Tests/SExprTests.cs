using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw
{
    public class SExprTests
    {
        public static void TestParse(string s, Rule r)
        {
            GrammarTest.Test(s, r);
        }

        public static void Tests()
        {
            TestParse("a", SExpressionGrammar.Symbol);
            TestParse("a123", SExpressionGrammar.Symbol);
            TestParse("_", SExpressionGrammar.Symbol);
            //TestParse(" ", SExpressionGrammar.WS);
            //TestParse("\t\t", SExpressionGrammar.WS);
            TestParse("123", SExpressionGrammar.Integer);
            TestParse("0", SExpressionGrammar.Integer);
            TestParse("a", SExpressionGrammar.Atom);
            TestParse("12", SExpressionGrammar.Atom);
            TestParse("a", SExpressionGrammar.Term);
            TestParse("12", SExpressionGrammar.Term);
            TestParse("a)", SExpressionGrammar.Term);
            TestParse("()", SExpressionGrammar.Term);
            TestParse("(a)", SExpressionGrammar.SExpr);
            TestParse("( a)", SExpressionGrammar.SExpr);
            TestParse("(a )", SExpressionGrammar.SExpr);
            TestParse("( a )", SExpressionGrammar.SExpr);
            TestParse("(a b)", SExpressionGrammar.SExpr);
            TestParse("()", SExpressionGrammar.SExpr);
            TestParse("((a b) c)", SExpressionGrammar.SExpr);
            TestParse("(c (a b))", SExpressionGrammar.SExpr);
            TestParse("(c (a 12) \"hello\")", SExpressionGrammar.SExpr);
            TestParse("(+ 1 2)", SExpressionGrammar.SExpr);
        }
    }
}
