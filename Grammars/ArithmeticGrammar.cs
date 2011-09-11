using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw.Grammars
{
    class ArithmeticGrammar : SharedGrammar
    {
        new public static Rule Integer = Node(SharedGrammar.Integer);
        new public static Rule Float = Node(SharedGrammar.Float);
        public static Rule ParanExpr = Node(CharToken('(') + Recursive(() => Expression) + WS + CharToken(')'));
        public static Rule SimpleExpr = Opt(CharToken('-') + (Number | ParanExpr));
        public static Rule Number = (Integer | Float) + WS;            
        public static Rule BinaryOp = Node(MatchStringSet("+ - * / %"));
        public static Rule Expression = Node(SimpleExpr + ZeroOrMore(BinaryOp + WS + Expression));
    }
}
