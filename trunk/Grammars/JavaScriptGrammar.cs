using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw
{
    public class JavaScriptGrammar : JsonGrammar
    {
        // Recursive rules defined at the top
        public static Rule RecExpr = Recursive(() => Expr);
        public static Rule RecStatement = Recursive(() => Statement);
        public static Rule Literal = Node(Recursive(() => String | Number | Object | Array | True | False | Null));

        // Redefine Identifier so that it creates nodes in the parse tree
        public new static Rule Identifier = Node(SharedGrammar.Identifier);

        // The following rules are redefined from JsonGrmmar because arbitrary expressions are allowed, not just 
        // Literal values.
        public static Rule PairName = Identifier | (MatchChar('"') + StringChars + MatchChar('"'));
        public new static Rule Pair = Node(PairName + WS + CharToken(':') + RecExpr + WS);
        public new static Rule Array = Node(CharToken('[') + CommaDelimited(RecExpr) + WS + CharToken(']'));
        public new static Rule Object = Node(CharToken('{') + CommaDelimited(Pair) + WS + CharToken('}'));

        // Function expressions
        public static Rule ParamList = Node(Parenthesize(CommaDelimited(Identifier + WS)));
        public static Rule NamedFunc = Node(Keyword("function") + Identifier + WS + ParamList + RecStatement);
        public static Rule AnonFunc = Node(Keyword("function") + ParamList + RecStatement);
        public static Rule Function = NamedFunc | AnonFunc;

        // Expression rules 
        public static Rule ArgList = Node(CharToken('(') + CommaDelimited(RecExpr) + CharToken(')'));
        public static Rule Index = Node(CharToken('[') + RecExpr + CharToken(']'));
        public static Rule Field = Node(CharToken('.') + Identifier);
        public static Rule PrefixOp = Node(MatchStringSet("! - ~"));
        public static Rule ParenExpr = Node(CharToken('(') + RecExpr + WS + CharToken(')'));
        public static Rule NewExpr = Node(Keyword("new") + RecExpr);
        public static Rule LeafExpr = Node(ParenExpr | NewExpr | Function | Literal | Identifier);
        public static Rule PrefixExpr = Node(PrefixOp + Recursive(() => UnaryExpr));
        public static Rule UnaryExpr = (PrefixExpr | LeafExpr) + WS;
        public static Rule PostfixOp = Field | Index | ArgList;
        public static Rule PostfixExpr = Node(UnaryExpr + WS + ZeroOrMore(PostfixOp));
        public static Rule BinaryOp = Node(MatchStringSet("<= >= == != << >> && || < > & | + - * % /"));
        public static Rule BinaryExpr = Node(PostfixExpr + ZeroOrMore(BinaryOp + WS + RecExpr));
        public static Rule AssignOp = Node(MatchStringSet("&&= ||= >>= <<= += -= *= %= /= &s= |= ^= ="));
        public static Rule AssignExpr = Node(((Identifier | PostfixExpr) + WS + AssignOp + WS + Recursive(() => AssignExpr)) | BinaryExpr);
        public static Rule Expr = Node(AssignExpr + Opt(CharToken('?') + RecExpr + CharToken(':') + RecExpr) + WS);

        // Statement rules
        public static Rule Block = Node(CharToken('{') + ZeroOrMore(RecStatement) + CharToken('}'));
        public static Rule VarDecl = Node(Keyword("var") + Identifier + WS + Opt(Eq + Expr) + Eos);
        public static Rule While = Node(Keyword("while") + Parenthesize(Expr) + RecStatement);
        public static Rule For = Node(Keyword("for") + Parenthesize(VarDecl + Expr + WS + Eos + Expr + WS) + RecStatement);
        public static Rule Else = Node(Keyword("else") + RecStatement);
        public static Rule If = Node(Keyword("if") + Parenthesize(Expr) + Block + Opt(Else));
        public static Rule ExprStatement = Expr + WS + Eos;
        public static Rule Return = Node(Keyword("return") + Opt(Expr) + WS + Eos); 
        public static Rule Statement = Node(Block | For | While | If | Return | VarDecl | ExprStatement);

        // The top-level rule
        public static Rule Script = Node(ZeroOrMore(Statement) + WS + End);
    
        // Grammar initiatlization
        static JavaScriptGrammar()
        {
            InitGrammar(typeof(JavaScriptGrammar));
        }
    }
}
