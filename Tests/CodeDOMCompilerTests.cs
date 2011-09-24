using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw.Tests
{
    class CodeDOMCompilerTests
    {
        public static void Tests()
        {
            string s = @"
using System;

namespace Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(""C# compiles C#!"");
            int argc = 0;
            foreach (var a in args)
                Console.WriteLine(""Argument #{0} is {1}"", argc++, a);
        }
    }
}
";
            var asm = CodeDOMCompilers.CompileCSharp(s);
            if (asm != null)
                Utilities.RunMain(asm, "Happy", "Sad");
            else
                Console.WriteLine("Compilation failed");
        }
    }
}
