﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw.Tests
{
    using Grammars;

    class SchemeTests
    {
        public static void Test(string input, params object[] args)
        {
            var node = Parser.Parse(input, SchemeGrammar.Lambda).First();
            var r = SchemeCompiler.NodeToLambda(node).Compile();
        }

        public static void Tests()
        {
            Test("(lambda())");
            Test("(lambda () )");
            Test("(lambda ( ) )");
            Test("(lambda (a) )");
            Test("(lambda (a) a)");
            Test("(lambda (a b) a)");
            Test("(lambda (a b) 12)");
            Test("(lambda (a b) (add a b))");
            Test("(lambda (a b) (add a b))", 3, 4);
        }
    }
}