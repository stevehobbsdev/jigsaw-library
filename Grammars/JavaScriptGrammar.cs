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
        public static Rule Literal = Recursive(() => String | Integer | Float | Object | Array | True | False | Null);

        // Redefine Identifier so that it creates nodes in the parse tree
        public new static Rule Identifier = Node(SharedGrammar.Identifier);

        // The following rules are redefined from JsonGrmmar because arbitrary expressions are allowed, not just literals
        public static Rule PairName = Identifier | DoubleQuotedString | SingleQuotedString;
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
        public static Rule NewExpr = Node(Keyword("new") + Recursive(() => PostfixExpr));
        public static Rule LeafExpr = ParenExpr | NewExpr | Function | Literal | Identifier;
        public static Rule PrefixExpr = Node(PrefixOp + Recursive(() => PrefixOrLeafExpr));
        public static Rule PrefixOrLeafExpr = PrefixExpr | LeafExpr;
        public static Rule PostfixOp = Field | Index | ArgList;
        public static Rule PostfixExpr = Node(PrefixOrLeafExpr + WS + OneOrMore(PostfixOp + WS));
        public static Rule UnaryExpr = PostfixExpr | PrefixOrLeafExpr;
        public static Rule BinaryOp = Node(MatchStringSet("<= >= == != << >> && || < > & | + - * % /"));
        public static Rule BinaryExpr = Node(UnaryExpr + WS + BinaryOp + WS + RecExpr);
        public static Rule AssignOp = Node(MatchStringSet("&&= ||= >>= <<= += -= *= %= /= &s= |= ^= ="));
        public static Rule AssignExpr = Node((Identifier | PostfixExpr) + WS + AssignOp + WS + RecExpr);
        public static Rule TertiaryExpr = Node((AssignExpr | BinaryExpr | UnaryExpr) + WS + CharToken('?') + RecExpr + CharToken(':') + RecExpr + WS);
        public static Rule Expr = Node((TertiaryExpr | AssignExpr | BinaryExpr | UnaryExpr) + WS);

        // Statement rules
        public static Rule Block = Node(CharToken('{') + ZeroOrMore(RecStatement) + CharToken('}'));
        public static Rule VarDecl = Node(Keyword("var") + Identifier + WS + Opt(Eq + Expr) + Eos);
        public static Rule While = Node(Keyword("while") + Parenthesize(Expr) + RecStatement);
        public static Rule For = Node(Keyword("for") + Parenthesize(VarDecl + Expr + WS + Eos + Expr + WS) + RecStatement);
        public static Rule Else = Node(Keyword("else") + RecStatement);
        public static Rule If = Node(Keyword("if") + Parenthesize(Expr) + RecStatement + Opt(Else));
        public static Rule ExprStatement = Node(Expr + WS + Eos);
        public static Rule Return = Node(Keyword("return") + Opt(Expr) + WS + Eos);
        public static Rule Empty = Node(WS + Eos);
        public static Rule Statement = Block | For | While | If | Return | VarDecl | ExprStatement | Empty;

        // The top-level rule
        public static Rule Script = Node(ZeroOrMore(Statement) + WS + End);
    
        // Grammar initiatlization
        static JavaScriptGrammar()
        {
            InitGrammar(typeof(JavaScriptGrammar));
        }
    }
}
