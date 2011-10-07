using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw
{
    class ILGrammar : SharedGrammar
    {
        public static Rule Name = Node(Identifier);
        public static Rule DotName = Node(Name + ZeroOrMore(Dot + Name));
        public static Rule Arg = Node(DotName + WS + Opt(Name + WS));
        public static Rule ArgList = Node(Parenthesize(CommaDelimited(Arg)));
        public static Rule Label = Node(Name + CharToken(':'));
        public static Rule Literal = Node(CSharpLiteralsGrammar.Literal);
        public static Rule Operand = Node(Name + WS + (Name | Literal));
        public static Rule VarDecl = Node(Keyword("var") + DotName + WS + Name + WS + Eos);
        public static Rule OpStatement = Node(Name + WS + Opt(Operand) + WS + Eos);
        public static Rule Statement = Node(VarDecl | Label | OpStatement);
        public static Rule Block = Node(CharToken('{') + ZeroOrMore(Statement + WS) + CharToken('}'));
        public static Rule ILFunc = Node(DotName + WS + Name + ArgList + Block);
    
        static ILGrammar() { InitGrammar(typeof(ILGrammar)); }
    }
}
