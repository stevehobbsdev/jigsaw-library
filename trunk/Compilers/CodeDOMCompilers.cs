using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using Microsoft.JScript;
using System.Reflection;
using System.IO;

namespace Diggins.Jigsaw
{
    // http://msdn.microsoft.com/en-us/library/system.codedom.compiler.codedomprovider.aspx
    public class CodeDOMCompilers
    {
        public static Assembly CompileJScriptFile(string filename)
        {
            return CompileJScript(File.ReadAllLines(filename));
        }

        public static Assembly CompileVBFile(string filename)
        {
            return CompileVB(File.ReadAllLines(filename));
        }

        public static Assembly CompileCSharpFile(string filename)
        {
            return CompileCSharp(File.ReadAllLines(filename));
        }

        public static Assembly CompileJScript(params string[] input)
        {
            return CompileWithProvider(new JScriptCodeProvider(), input);
        }

        public static Assembly CompileVB(params string[] input)
        {
            return CompileWithProvider(new VBCodeProvider(), input);
        }

        public static Assembly CompileCSharp(params string[] input)
        {
            return CompileWithProvider(new CSharpCodeProvider(), input);
        }

        public static Assembly CompileWithProvider(CodeDomProvider provider, params string[] input)
        {
            var param = new System.CodeDom.Compiler.CompilerParameters();
            var result = provider.CompileAssemblyFromSource(param, input);
            foreach (var e in result.Errors)
                Console.WriteLine("Error occured during compilation {0}", e.ToString());
            if (result.Errors.Count > 0)
                return null;
            else
                return result.CompiledAssembly;           
        }
    }
}
