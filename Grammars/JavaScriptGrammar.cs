using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw.Grammars
{
    public class JavaScriptGrammar : JsonGrammar 
    {
        public static Rule RecExpr = Recursive(() => Expr);
        public static Rule Args = Node(CommaDelimited(Expr));
        public static Rule ArgList = CharToken('(') + Args + CharToken(')');
        public static Rule Indexer = Node(CharToken('[') + RecExpr + CharToken(']'));
        public static Rule Select = Node(CharToken('.') + Identifier);
        public static Rule Inc = Node(MatchString("++"));
        public static Rule Dec = Node(MatchString("--"));
        public static Rule PostfixOp = Node(Inc | Dec | Indexer | ArgList | Select));
        public static Rule PrefixOp = Node(MatchStringSet("++ -- ! - ~"));
        public static Rule BinaryOp = Node(MatchStringSet(">>= <<= <= >= == != << >> += -= *= %= /= && || < > & | + - * % / ="));
        public static Rule ParanthesizedExpr = Node(CharToken('(') + RecExpr + CharToken(')'));
        public static Rule NewExpr = Node(Keyword("new") + UnaryExpr);
        public static Rule LeafExpr = Node(ParanthesizedExpr | NewExpr | Identifier | Integer | Float | JsonGrammar.String);
        public static Rule Prefixes = Node(ZeroOrMore(PrefixOp + WS));
        public static Rule Postfixes = Node(ZeroOrMore(PostfixOp + WS));
        public static Rule UnaryExpr = Node(Prefixes + WS + Postfixes);
        public static Rule BinaryExpr = Node(UnaryExpr + Opt(BinaryOp + WS + RecExpr));
        public static Rule Expr = Node(BinaryExpr + Opt(CharToken('?') + RecExpr + CharToken(':') + RecExpr));

        public static Rule RecStatement = Recursive(() => Statement);
        public static Rule Block = Node(CharToken('{') + ZeroOrMore(Statement) + CharToken('}'));
        public static Rule VarDecl = Node(Opt(Keyword("var")) + Name + WS + Opt(Eq + Expr) + Eos);
        public static Rule While = Node(Keyword("while") + Parenthesize(Expr) + Block);
        public static Rule For = Node(Keyword("for") + Parenthesize(VarDecl + Expr + WS + Eos + Expr + WS) + Block);
        public static Rule Else = Node(Keyword("else") + Block);
        public static Rule If = Node(Keyword("if") + Parenthesize(Expr) + Block + Opt(Else));
        public static Rule ParamList = Node(Parenthesize(CommaDelimited(Name + WS)));
        public static Rule NamedFunction = Node(Keyword("function") + Name + WS + ParamList + Block);
        public static Rule AnonymousFunction = Node(Keyword("function") + ParamList + Block);
        public static Rule Function = NamedFunction | AnonymousFunction;
        public static Rule Statement = Node(Block | For | While | If | VarDecl | Function);
        public static Rule Script = Node(ZeroOrMore(Statement));
    }
}
