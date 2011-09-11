using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw.Grammars
{
    public class SExprGrammar : SharedGrammar
    {
        public static Rule Term = Node(Recursive(() => (Atom | Lambda | SExpr)));
        public static Rule Terms = Node(ZeroOrMore(Term + WS));
        new public static Rule Integer = Node(SharedGrammar.Integer);
        new public static Rule Float = Node(SharedGrammar.Float);
        public static Rule FirstSymbolChar = Letter | CharSet(@"~@#$%^&*-_=+:<>,./\");
        public static Rule NextSymbolChar = FirstSymbolChar | Digit;
        public static Rule Symbol = Node(FirstSymbolChar + ZeroOrMore(NextSymbolChar));
        public static Rule String = Node(MatchChar('"') + AdvanceWhileNot(MatchChar('"')) + MatchChar('"'));
        public static Rule Atom = Node(Integer | Float | String | Symbol);
        public static Rule ParamList = Node(CharToken('(') + ZeroOrMore(Symbol + WS) + CharToken(')'));
        public static Rule Lambda = Node(CharToken('(') + StringToken("lambda") + ParamList + Terms + WS + CharToken(')'));
        public static Rule SExpr = Node(CharToken('(') + Terms + WS + CharToken(')'));

        static SExprGrammar() { InitGrammar(typeof(SExprGrammar)); }
    }
}
