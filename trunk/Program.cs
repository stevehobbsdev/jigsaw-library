using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Dynamic;

namespace Diggins.Jigsaw
{
    using Tests;

    class Program
    {
        static void Test(IDynamicMetaObjectProvider mop)
        {
            Console.WriteLine(mop);
        }

        static object AddTest(object a, object b)
        {
            return Primitives.add(a, b);
        }

        static void ETreeTest()
        {
            Expression<Func<object, object, object>> e = (object a, object b) => Primitives.add(a, b);
            dynamic f = e.Compile();
            Console.WriteLine(f(20, 22));

            /*
            var a = Expression.Parameter(typeof(int), "a");
            var b = Expression.Parameter(typeof(int), "b");
            var l = Expression.Lambda(Expression.Add(a, b), a, b);
            dynamic f = l.Compile();
            Console.WriteLine(f(3, 4));
             */
        }

        static void Main(string[] args)
        {
            ArithmeticTests.Tests();
            SExprTests.Tests();
            //DynamicExtensions.Test();
            //JsonTests.Tests();
            //ETreeTest();
            //UnifierTests.Tests();
            //CatTests.Tests();
            //ILTests.Tests();
            //LambdaCalculus.Test();
            //EmbeddedScheme.Tests();
            //CodeDOMCompilerTests.Tests();
            CSharpFunctionCompilerTests.Tests();
            Console.ReadKey();
        }
    }
}
