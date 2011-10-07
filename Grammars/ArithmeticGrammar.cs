using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw
{
    class ArithmeticGrammar : SharedGrammar
    {
        new public static Rule Integer = Node(SharedGrammar.Integer);
        new public static Rule Float = Node(SharedGrammar.Float);
        public static Rule RecExpr = Recursive(() => Expression);
        public static Rule ParanExpr = Node(CharToken('(') + RecExpr + WS + CharToken(')'));
        public static Rule Number = (Integer | Float) + WS;
        public static Rule PrefixOp = Node(MatchStringSet("! - ~"));
        public static Rule PrefixExpr = Node(PrefixOp + Recursive(() => SimpleExpr));
        public static Rule SimpleExpr = PrefixExpr | Number | ParanExpr;
        public static Rule BinaryOp = Node(MatchStringSet("<= >= == != << >> && || < > & | + - * % / ^"));
        public static Rule Expression = Node(SimpleExpr + ZeroOrMore(BinaryOp + WS + SimpleExpr));
        static ArithmeticGrammar() { InitGrammar(typeof(ArithmeticGrammar)); }
    }
}
