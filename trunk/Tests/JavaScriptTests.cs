using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Diggins.Jigsaw
{
    public class JavaScriptTests
    {
        public static void TestPrint(string s)
        {
            try
            {
                Console.WriteLine("input:");
                Console.WriteLine(s);
                var n = JavaScriptGrammar.Script.Parse(s)[0];
                Console.WriteLine("nodes");
                Console.WriteLine(n.ToXml);
                var p = JavaScriptSourcePrinter.ToString(n);
                Console.WriteLine("output:");
                Console.WriteLine(p);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR occured " + e.Message);
            }
        }

        public static void Test(string s, dynamic expected = null)
        {
            try
            {
                if (expected != null)
                {
                    Console.WriteLine("Testing: {0}", s);
                    Console.WriteLine("Expecting {0}", expected);
                    dynamic result = JavaScriptEvaluator.RunScript(s);
                    Console.WriteLine("Result {0}", result);
                    if (result != expected)
                        Console.WriteLine("ERROR!");
                }
                else
                {
                    Console.WriteLine("Testing: {0}", s);
                    JavaScriptEvaluator.RunScript(s);                    
                }
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

        public static void TimingTests()
        {
            Utilities.TimeExecution(() =>
                TestParse(@"var x = { a : 10, b : 20, c :  { x : 1, y : 2, z : 3 } };", JavaScriptGrammar.Script));

            Utilities.TimeExecution(() =>
                TestParse(@"var x = { a : 10, b : 20, c :  { x : 1, y : 2, z : a[1][2][3][4][5][6] } };", JavaScriptGrammar.Script));
            
            Utilities.TimeExecution(() =>
                TestParse(@"var x = { a : 10, b : 20, c :  { x : 1, y : 2, z : a[1][2][3][4][5][6], w : { a:1, b:2, c:3, d:4, e:5 } } };", JavaScriptGrammar.Script));

            Utilities.TimeExecution(() =>
                TestParse(@"var x = { a : [1,2,3,4,5,6,7,8,9], b : [1,2,3,4,5,6,7,8,9], c : [1,2,3,4,5,6,7,8,9] };", JavaScriptGrammar.Script));

            Utilities.TimeExecution(() =>
                TestParse(@"var x = { a : { b : { c : [1,2,3,4,5,6,7,8,9] } } };", JavaScriptGrammar.Script));

            Utilities.TimeExecution(() =>
                TestParse(@"var x = { a : { b : [1,2,3,4,5,6,7,8,9], c : [1,2,3,4,5,6,7,8,9], d : [1,2,3,4,5,6,7,8,9] } };", JavaScriptGrammar.Script));

            Utilities.TimeExecution(() =>
                TestParse(@"var x = { a : { b : { c : [1,2,3,4,5,6,7,8,9], d : [1,2,3,4,5,6,7,8,9], e : [1,2,3,4,5,6,7,8,9] } } };", JavaScriptGrammar.Script));

            Utilities.TimeExecution(() =>
                TestParse(@"var x = { a : { b : { c:1, d:1, e:1, f:1, g:1, h:1, i:1, j:1, k:1, l:1, m:1, n:1 }, o:1, p:1, q:1, r:1 }, s:1, t:1, u:1 };", JavaScriptGrammar.Script));

            Utilities.TimeExecution(() =>
                TestParse(@"var x = { a : { b : { c : [1,2,3,4,5,6,7,8,9], d : [1,2,3,4,5,6,7,8,9], e : [1,2,3,4,5,6,7,8,9] }, f:1, g:1, h:1, i:1, j:1, k:1, l:1, m:1, n:1 }, o:1, p:1, q:1, r:1 };", JavaScriptGrammar.Script));

            Utilities.TimeExecution(() =>
            TestParse(
@"var x = { 
    a : 10, 
    b : 20, 
    c : { 
        x : 1, y : 2, z : 
        [ 
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 
            [1, 2, 3, 4, 5, 6], 
            [7, 8, 9, 10, 11, 12], 
            { a:1, b:2, c:3, d:4 } 
        ] },
    d : a[1][2][3],
    'e' : f(a, b, {a:1, b:2, c:3}),
    ""f"" : f(),
    h : f([a, b, c, d, e, e])
    };", JavaScriptGrammar.Script));
        }

        public static void Tests()
        {
            TimingTests();

            TestParse("3 + 3", JavaScriptGrammar.Expr);
            TestParse("3 + 3", JavaScriptGrammar.BinaryExpr);
            TestParse("x + 3", JavaScriptGrammar.BinaryExpr);
            TestParse("x = 3", JavaScriptGrammar.Expr);
            TestParse("x = 3", JavaScriptGrammar.AssignExpr);
            TestParse("(3)", JavaScriptGrammar.ParenExpr);
            TestParse("(3 + 3)", JavaScriptGrammar.ParenExpr);
            TestParse("42;", JavaScriptGrammar.ExprStatement);
            TestParse("\"hello\"", JavaScriptGrammar.Expr);
            TestParse("\"\"", JavaScriptGrammar.Expr);
            TestParse("\"\\\"\"", JavaScriptGrammar.Expr);
            TestParse("'hello'", JavaScriptGrammar.Expr);
            TestParse("''", JavaScriptGrammar.Expr);
            TestParse("'\"'", JavaScriptGrammar.Expr);
            TestParse("\"'\"", JavaScriptGrammar.Expr);
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
            TestParse("if (x) return 42;", JavaScriptGrammar.Statement);
            TestParse("if (x) return 42; else return 42;", JavaScriptGrammar.Statement);
            TestParse("if (y) return x + 5; else { var z = 6; return z + x; }", JavaScriptGrammar.Statement);
            TestParse("function f(x, y) { if (y) return x + 5; else { var z = 6; return z + x; } }", JavaScriptGrammar.Function);
            TestParse("[ 'a', 'b', 'c']", JavaScriptGrammar.Expr);
            TestParse("{ a : 10, b : 20, c : { x : 1, y : 2, z : [ 'a', 'b', 'c'] } }", JavaScriptGrammar.Expr);

            Test("42;", 42);
            Test("6 * 7;", 42);
            Test("(3 + 3) * (3 + 4);", 42);
            Test("2 + 5 * 8;", 42);
            Test("2 + (5 * 8);", 42);
            Test("(2 + 5) * 8;", 35);
            Test("13; 42;", 42);
            Test("{ 13; 42; }", 42);
            Test("var i = 42; i;", 42);
            Test("(function() { return 42; })();", 42);
            Test("var i = 1; i = 5; 3; i + 2;", 7);
            Test("i = 42; i;", 42);
            Test("var i = 1; i = 5; 3; i += 2;", 7);
            Test("function f(x) { return x + 1; }; f(41);", 42);
            Test("var f = function(x) { return x + 1; }; f(41);", 42);
            Test("f = function(x) { return x + 1; }; f(41);", 42);
            Test("var x = 41; f = function() { return x + 1; }; f();", 42);
            Test("function f(a, b) { return a * b; }; f(6, 7);", 42);
            Test("var a = 6; function f(b) { return a * b; }; f(7);", 42);
            Test("var a = 6; function f(b) { return a * b; }; { a = 7; f(7); }", 49);
            Test("var a = 6; function f(b) { return a * b; }; { var a = 7; f(7); }", 42);
            Test("var x = { a : 42 }; x.a;", 42);
            Test("alert('Hello world');");
            Test("alert(\"Hello world\");");

            TestPrint("var a; a = 10; a += 5; function f(x, y) { if (y) return x + 5; else { var z = 6; return z + x; } }; alert(f(9));");            
            TestPrint("var x = { a : 10, b : 20, c : { x : 1, y : 2, z : [ 'a', 'b', 'c'] } };");

        }
    }
}
