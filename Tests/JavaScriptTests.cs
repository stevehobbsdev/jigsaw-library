using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw
{
    public class JavaScriptTests
    {
        public static void Test(string s, dynamic expected)
        {
            try
            {
                Console.WriteLine("Testing: {0}", s);
                Console.WriteLine("Expecting {0}", expected);
                dynamic result = JavaScriptEvaluator.RunScript(s);
                Console.WriteLine("Result {0}", result);
                if (result != expected)
                    Console.WriteLine("ERROR!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occured: " + e.Message);
            }
        }

        public static void TestParse(string s, Rule r)
        {
            GrammarTest.Test(s, r);
        }

        public static void Tests()
        {
            TestParse("3 + 3", JavaScriptGrammar.Expr);
            TestParse("3 + 3", JavaScriptGrammar.BinaryExpr);
            TestParse("x + 3", JavaScriptGrammar.BinaryExpr);
            TestParse("x = 3", JavaScriptGrammar.Expr);
            TestParse("x = 3", JavaScriptGrammar.AssignExpr);
            TestParse("(3)", JavaScriptGrammar.ParenExpr);
            TestParse("(3 + 3)", JavaScriptGrammar.ParenExpr);
            TestParse("42;", JavaScriptGrammar.ExprStatement);
            TestParse("{}", JavaScriptGrammar.Block);
            TestParse("{ }", JavaScriptGrammar.Block);
            TestParse("{ 42; }", JavaScriptGrammar.Block);
            TestParse("{ 3; 4; }", JavaScriptGrammar.Block);
            TestParse("f", JavaScriptGrammar.Expr);
            TestParse("()", JavaScriptGrammar.ArgList);
            TestParse("(  )", JavaScriptGrammar.ArgList);
            TestParse("(a)", JavaScriptGrammar.ArgList);
            TestParse("( a )", JavaScriptGrammar.ArgList);
            TestParse("(a,b)", JavaScriptGrammar.ArgList);
            TestParse("( a , b )", JavaScriptGrammar.ArgList);
            TestParse("f()", JavaScriptGrammar.Expr);
            TestParse("f(a)", JavaScriptGrammar.Expr);
            TestParse("f(a, b)", JavaScriptGrammar.Expr);
            TestParse("f(a)(b)", JavaScriptGrammar.Expr);
            TestParse("(f)(b)", JavaScriptGrammar.Expr);
            TestParse("a[1]", JavaScriptGrammar.Expr);
            TestParse("a[1](2)", JavaScriptGrammar.Expr);
            TestParse("[]", JavaScriptGrammar.Expr);
            TestParse("[a]", JavaScriptGrammar.Expr);
            TestParse("[a,1,2]", JavaScriptGrammar.Expr);
            TestParse("{}", JavaScriptGrammar.Expr);
            TestParse("{a:b}", JavaScriptGrammar.Expr);
            TestParse("{ a : b }", JavaScriptGrammar.Expr);
            TestParse("{a:b,\"b\":42}", JavaScriptGrammar.Expr);
            TestParse("{ a : { b : c }, d : [1, 2, 3] }", JavaScriptGrammar.Expr);
            TestParse("function f() { }", JavaScriptGrammar.Function);
            TestParse("function f() { }", JavaScriptGrammar.Expr);
            TestParse("function () { }", JavaScriptGrammar.Function);
            TestParse("function (a) { }", JavaScriptGrammar.Function);
            TestParse("function f(a) { }", JavaScriptGrammar.Function);
            TestParse("function (a, b) { }", JavaScriptGrammar.Function);
            TestParse("function f(a, b) { }", JavaScriptGrammar.Function);
            TestParse("function f(a, b) { };", JavaScriptGrammar.Statement);
            TestParse("(function f(a, b) { })()", JavaScriptGrammar.Expr);
            TestParse("(function f(a, b) { })();", JavaScriptGrammar.Statement);
            TestParse("return 42;", JavaScriptGrammar.Statement);            
            
            Test("42;", 42);
            Test("6 * 7;", 42);
            Test("(3 + 3) * (3 + 4);", 42);
            Test("13; 42;", 42);
            Test("{ 13; 42; }", 42);
            Test("var i = 42; i;", 42);
            Test("(function() { return 42; })();", 42);
            Test("var i = 1; i = 5; 3; i + 2;", 7);
            Test("i = 42; i;", 42);
            Test("function f(x) { return x + 1; }; f(41);", 42);
            Test("var f = function(x) { return x + 1; }; f(41);", 42);
            Test("f = function(x) { return x + 1; }; f(41);", 42);
            Test("var x = 41; f = function() { return x + 1; }; f();", 42);
            Test("function f(a, b) { return a * b; }; f(6, 7);", 42);
            Test("var a = 6; function f(b) { return a * b; }; f(7);", 42);
            Test("var a = 6; function f(b) { return a * b; }; { a = 7; f(7); }", 49);
            Test("var a = 6; function f(b) { return a * b; }; { var a = 7; f(7); }", 42);
            Test("var x = { a : 42 }; x.a;", 42);
        }
    }
}
