 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using System.Dynamic;

namespace Diggins.Jigsaw
{
    public class JsonTests
    {
        public static void Test(string input, Rule r)
        {
            var nodes = r.Parse(input);
            if (nodes == null)
                throw new Exception("Parser failed");
            foreach (var n in nodes)
            {
                if (n.Label == "Object")
                {
                     var d = JsonObject.Eval(n);
                    var s = d.ToString();
                    Console.WriteLine(s);
                }
                else
                {
                    Console.WriteLine(n.ToXml.ToString());
                }
            }
        }

        public static void Test(string s)
        {
            Test(s, JsonGrammar.Object);
        }

        public static void Tests()
        {
            Test("1", JsonGrammar.Integer);
            Test("true", JsonGrammar.True);
            Test("false", JsonGrammar.False);
            Test("\"hello\"", JsonGrammar.String);
            Test("{}", JsonGrammar.Object);
            Test("{\"a\":123}", JsonGrammar.Object);
            Test("[]", JsonGrammar.Array);
            Test("[1]", JsonGrammar.Array);
            Test("[1,2,3]", JsonGrammar.Array);
            Test("[1, 2, 3]", JsonGrammar.Array);
            Test("[ 1, 2, 3 ]", JsonGrammar.Array);
            Test("{\"x\":12}");
            Test("{\"x\" : 12}");
            Test("{ \"x\" : 12 }");
            Test("{ \"x\" : true }");
            Test("{ \"a\" : 1, \"b\" : 2 }");
            Test("{ \"a\" : 1, \"b\" : 2 }");
            Test("{ \"a\" : { \"b\" : 2 } }");
            Test("{ \"a\" : [ 1, 2, 3 ] }");
            Test("{ \"a\" : \"b\" }");

            dynamic d = JsonObject.Parse("{ \"answer\" : 42 }");
            Console.WriteLine(d.answer);

            dynamic d2 = JsonObject.Parse(@"{ ""point"" : [12, 13, 14] }");
            Console.WriteLine("x={0}, y={1}, z={2}", d2.point[0], d2.point[1], d2.point[2]);

            dynamic d3 = JsonObject.Parse(@"{ ""author"" : ""Christopher"", ""title"" : ""Implementing Languages"", ""price"" : 145.99 }");
            Console.WriteLine("author", d3.author); 
        }
    }
}
