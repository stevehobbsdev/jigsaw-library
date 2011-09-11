using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw.Grammars
{
    public class JsonGrammar : SharedGrammar
    {
        new public static Rule Integer = Node(SharedGrammar.Integer);
        new public static Rule Float = Node(SharedGrammar.Float);
        public static Rule Number = Node(Integer | Float);
        public static Rule True = Node(MatchString("true"));
        public static Rule False = Node(MatchString("false"));
        public static Rule Null = Node(MatchString("null"));
        public static Rule UnicodeControlChar = Node(MatchString("\\u") + Repeat(4, HexDigit));
        public static Rule ControlChar = Node(MatchChar('\\') + CharSet("\"\\/bfnt"));
        public static Rule PlainChar = Node(ExceptCharSet("\"\\"));
        public static Rule Char = Node(UnicodeControlChar | ControlChar | PlainChar);
        public static Rule StringChars = Node(ZeroOrMore(Char));
        public static Rule String = Node(MatchChar('"') + StringChars + MatchChar('"'));
        public static Rule Value = Node(Recursive(() => String | Number | Object | Array | True | False | Null));
        public static Rule Name = Node(String);
        public static Rule Pair = Node(Name + WS + CharToken(':') + Value + WS);
        public static Rule Members = Node(CommaDelimited(Pair));
        public static Rule Elements = Node(CommaDelimited(Value));
        public static Rule Array = Node(CharToken('[') + Elements + WS + CharToken(']'));
        public static Rule Object = Node(CharToken('{') + Members + WS + CharToken('}'));

        static JsonGrammar() { InitGrammar(typeof(JsonGrammar)); }
    }
}
