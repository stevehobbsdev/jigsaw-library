using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw.Grammars
{
    class CSharpExprGrammar : SharedGrammar
    {
        new public static Rule Identifier = Node(SharedGrammar.Identifier);
        new public static Rule Integer = Node(SharedGrammar.Integer);
        new public static Rule Float = Node(SharedGrammar.Float);

        // This is a cheat, because we are not supporting all of the control codes.
        public static Rule String = JsonGrammar.String;

        public static Rule FullExpression = Recursive(() => Expression);
        public static Rule Args = Node(CommaDelimited(Expression));
        public static Rule ArgList = CharToken('(') + Args + CharToken(')');
        public static Rule Indexer = Node(CharToken('[') + FullExpression + CharToken(']'));
        public static Rule Selecter = Node(CharToken('.') + Identifier);
        public static Rule PostfixOp = Node((MatchStringSet("++ --") | Indexer | ArgList | Selecter));
        public static Rule PrefixOp = Node(MatchStringSet("++ -- ! + - ~"));
        public static Rule BinaryOp = Node(MatchStringSet(">>= <<= <= >= == != << >> += -= *= %= /= && || < > & | + - * % / ="));
        public static Rule ParanthesizedExpression = CharToken('(') + FullExpression + CharToken(')');
        public static Rule LeafExpression = ParanthesizedExpression | Identifier | Integer | Float | JsonGrammar.String;
        public static Rule Prefixes = Node(ZeroOrMore(PrefixOp + WS));
        public static Rule Postfixes = Node(ZeroOrMore(PostfixOp + WS));
        public static Rule UnaryExpression = Node(Prefixes + LeafExpression + WS + Postfixes);
        public static Rule BinaryExpression = Node(UnaryExpression + BinaryOp + WS + FullExpression);
        public static Rule Expression = Node(BinaryExpression + Opt(CharToken('?') + FullExpression + CharToken(':') + FullExpression));

        static CSharpExprGrammar() { InitGrammar(typeof(CSharpExprGrammar)); }
    }
}
