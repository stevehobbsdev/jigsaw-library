using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw.Grammars
{
    class CSharpLiteralsGrammar : SharedGrammar
    {
        public static Rule F = CharSet("fF");
        public static Rule L = CharSet("lL");
        public static Rule U = CharSet("uU");

        public static Rule IntSuffix = (U + Opt(L)) | (L + Opt(U));
        public static Rule FloatSuffix = CharSet("FfDdMm");
        
        new public static Rule Identifier = Node(SharedGrammar.Identifier);
        new public static Rule Integer = Node(SharedGrammar.Integer + Opt(IntSuffix));
        new public static Rule Float = Node(SharedGrammar.Float + Opt(FloatSuffix));

        public static Rule EscapedChar = Node(MatchChar('\\') + CharSet("\\bnrt'\""));
        public static Rule CharContent = Node(EscapedChar | ExceptCharSet("\""));
        public static Rule String = Node(MatchChar('"') + ZeroOrMore(CharContent) + MatchChar('"'));
        public static Rule Char = MatchChar('\'') + CharContent + MatchChar('\'');
        public static Rule Bool = Node(Keyword("true") | Keyword("false"));
        public static Rule Literal = Node(String | Char | Integer | Float | Bool);

        static CSharpLiteralsGrammar() { InitGrammar(typeof(CSharpLiteralsGrammar)); }
    }
}
