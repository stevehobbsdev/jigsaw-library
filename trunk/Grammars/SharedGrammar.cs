using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw
{
    public class SharedGrammar : Grammar
    {
        public static Rule MatchAnyString(params string[] xs) { return Choice(xs.Select(x => MatchString(x)).ToArray()); }
        public static Rule MatchStringSet(string s) { return MatchAnyString(s.Split(' ')); }

        public static Rule FullComment      = MatchString("/*") + AdvanceWhileNot(MatchString("*/"));
        public static Rule LineComment      = MatchString("//") + AdvanceWhileNot(MatchChar('\n'));
        public static Rule WS               = Pattern(@"\s*");
        public static Rule Digit            = MatchChar(Char.IsDigit);
        public static Rule Digits           = OneOrMore(Digit);
        public static Rule Letter           = MatchChar(Char.IsLetter);
        public static Rule LetterOrDigit    = MatchChar(Char.IsLetterOrDigit);
        public static Rule E                = (MatchChar('e') | MatchChar('E')) + Opt(MatchChar('+') | MatchChar('-'));
        public static Rule IdentFirstChar   = MatchChar(c => Char.IsLetter(c) || c == '_');
        public static Rule IdentNextChar    = MatchChar(c => Char.IsLetterOrDigit(c) || c == '_');
        public static Rule Identifier       = IdentFirstChar + ZeroOrMore(IdentNextChar);
        public static Rule Exp              = E + Digits;
        public static Rule Frac             = MatchChar('.') + Digits;
        public static Rule Integer          = Digits + Not(MatchChar('.'));
        public static Rule Float            = Digits + ((Frac + Opt(Exp)) | Exp);
        public static Rule HexDigit         = Digit | CharRange('a', 'f') | CharRange('A', 'F');
            
        public static Rule CharToken(char c) { return MatchChar(c) + WS; }
        public static Rule StringToken(string s) { return MatchString(s) + WS; } 
        public static Rule CommaDelimited(Rule r) { return Opt(r + (ZeroOrMore(CharToken(',') + r) + Opt(CharToken(',')))); }

        public static Rule Comma = CharToken(',');
        public static Rule Eos = CharToken(';');
        public static Rule Eq = CharToken('=');
        public static Rule Dot = CharToken('.');
        public static Rule Keyword(string s) { return MatchString(s) + Not(LetterOrDigit) + WS; } 
        public static Rule KeywordSet(string s) { return Choice(s.Split(' ').Select(x => Keyword(x)).ToArray()); }
        public static Rule Parenthesize(Rule r) { return CharToken('(') + r + WS + CharToken(')'); }

        static SharedGrammar() { InitGrammar(typeof(SharedGrammar)); }
    }
}
