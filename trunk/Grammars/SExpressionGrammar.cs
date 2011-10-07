using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw
{
    public class SExpressionGrammar : SharedGrammar
    {
        new public static Rule Integer = Node(SharedGrammar.Integer);
        new public static Rule Float = Node(SharedGrammar.Float);
        
        public static Rule Term = Node(Recursive(() => (Atom | SExpr)));
        public static Rule Terms = Node(ZeroOrMore(Term + WS));
        public static Rule FirstSymbolChar = Letter | CharSet(@"~@#$%^&*-_=+:<>,./\");
        public static Rule NextSymbolChar = FirstSymbolChar | Digit;
        public static Rule Symbol = Node(FirstSymbolChar + ZeroOrMore(NextSymbolChar));
        public static Rule String = Node(MatchChar('"') + AdvanceWhileNot(MatchChar('"')) + MatchChar('"'));
        public static Rule Atom = Node(Integer | Float | String | Symbol);
        public static Rule SExpr = Node(CharToken('(') + Terms + CharToken(')'));

        static SExpressionGrammar() { InitGrammar(typeof(SExpressionGrammar)); }
    }
}
