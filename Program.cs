using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Dynamic;

namespace Diggins.Jigsaw
{
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

        static void Main(string[] args)
        {
            /*
            ArithmeticTests.Tests();
            SExprTests.Tests();
            JsonTests.Tests();
            UnifierTests.Tests();
            ILTests.Tests();
            LambdaCalculus.Test();
            EmbeddedScheme.Tests();
            CodeDOMCompilerTests.Tests();
            CSharpFunctionCompilerTests.Tests();
             * */
            JavaScriptTests.Tests();
            CodeProjectArticleSnippets.Tests();

            Console.WriteLine("And that's it. Press any key to go home ...");
            Console.ReadKey();
        }
    }
}
