using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw.Grammars
{
    class JavaScriptGrammar : JsonGrammar 
    {
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

        public static Rule RecStatement = Recursive(() => Statement);
        public static Rule Block = Node(CharToken('{') + ZeroOrMore(Statement) + CharToken('}'));
        public static Rule VarDecl = Node(Opt(Keyword("var")) + Name + Eq + Expression + Eos);
        public static Rule While = Node(Keyword("while") + Paranthesize(Expression) + Block);
        public static Rule For = Node(Keyword("for") + Paranthesize(VarDecl + Expression + WS + Eos + Expression + WS) + Block);
        public static Rule Else = Node(Keyword("else") + Block);
        public static Rule If = Node(Keyword("if") + Paranthesize(Expression) + Block + ZeroOrMore(Opt(Else)));
        public static Rule Statement = Node(Block | For | While | If | VarDecl);
    }
}
