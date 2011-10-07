using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw
{
    public class JavaScriptGrammar : JsonGrammar
    {
        public new static Rule Identifier = Node(SharedGrammar.Identifier);

        public static Rule RecExpr = Recursive(() => Expr);
        public static Rule RecStatement = Recursive(() => Statement);

        public static Rule ParamList = Node(Parenthesize(CommaDelimited(Identifier + WS)));
        public static Rule NamedFunc = Node(Keyword("function") + Identifier + WS + ParamList + RecStatement);
        public static Rule AnonFunc = Node(Keyword("function") + ParamList + RecStatement);
        public static Rule Function = NamedFunc | AnonFunc;

        public static Rule Args = Node(CommaDelimited(RecExpr));
        public static Rule ArgList = Node(CharToken('(') + Args + CharToken(')'));
        public static Rule Index = Node(CharToken('[') + RecExpr + CharToken(']'));
        public static Rule Field = Node(CharToken('.') + Identifier);
        public static Rule PrefixOp = Node(MatchStringSet("! - ~"));
        public static Rule ParenExpr = Node(CharToken('(') + RecExpr + CharToken(')'));
        public static Rule NewExpr = Node(Keyword("new") + RecExpr);
        public static Rule LeafExpr = Node(ParenExpr | NewExpr | Function | Value | Identifier);
        public static Rule PrefixExpr = Node(PrefixOp + Recursive(() => UnaryExpr));
        public static Rule UnaryExpr = PrefixExpr | LeafExpr;
        public static Rule PostfixOp = Node(Field | Index | ArgList);
        public static Rule PostfixExpr = Node(UnaryExpr + WS + ZeroOrMore(PostfixOp));
        public static Rule BinaryOp = Node(MatchStringSet("<= >= == != << >> && || < > & | + - * % /"));
        public static Rule BinaryExpr = Node(UnaryExpr + Opt(BinaryOp + WS + RecExpr));
        public static Rule AssignOp = Node(MatchStringSet("&&= ||= >>= <<= += -= *= %= /= &= |= ="));
        public static Rule AssignExpr = Node((Identifier | PostfixExpr) + WS + AssignOp + WS + Recursive(() => AssignExpr));
        public static Rule Expr = Node(BinaryExpr + Opt(CharToken('?') + RecExpr + CharToken(':') + RecExpr));
        public static Rule Block = Node(CharToken('{') + ZeroOrMore(RecStatement) + CharToken('}'));
        public static Rule VarDecl = Node(Keyword("var") + Identifier + WS + Opt(Eq + Expr) + Eos);
        public static Rule While = Node(Keyword("while") + Parenthesize(Expr) + RecStatement);
        public static Rule For = Node(Keyword("for") + Parenthesize(VarDecl + Expr + WS + Eos + Expr + WS) + RecStatement);
        public static Rule Else = Node(Keyword("else") + RecStatement);
        public static Rule If = Node(Keyword("if") + Parenthesize(Expr) + Block + Opt(Else));
        public static Rule Statement = Node(Block | For | While | If | VarDecl | Expr);
        public static Rule Script = Node(ZeroOrMore(Statement));

        static JavaScriptGrammar()
        {
            InitGrammar(typeof(JavaScriptGrammar));
        }
    }
}
