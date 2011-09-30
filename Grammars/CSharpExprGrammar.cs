using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw.Grammars
{
    class CSharpExprGrammar : CSharpLiteralsGrammar
    {
        public static Rule RecExpr = Recursive(() => Expr);

        public static Rule Block = Node(CharToken('{') + ZeroOrMore(Recursive(() => Statement)) + WS + CharToken('}'));
        public static Rule ExprStatement = Node(RecExpr + WS + Eos);
        public static Rule ReturnStatement = Node(Keyword("return") + Opt(RecExpr) + WS + Eos);
        public static Rule Statement = Node(Block | ExprStatement | ReturnStatement);

        public static Rule SymbolChar = CharSet("~!^&*<>/+-=%?"); 
        public static Rule TypeName = Node(Identifier + WS + ZeroOrMore(Dot + Identifier + WS));
        public static Rule TypeArgs = Node(Opt(CharToken('<') + CommaDelimited(Recursive(() => TypeExpr)) + CharToken('>')));
        public static Rule TypeArrayIndicator = Node(CharToken('[') + CharToken(']'));
        public static Rule TypeExpr = Node(TypeName + Opt(TypeArgs) + Opt(TypeArrayIndicator));
        public static Rule TypedLambdaParam = Node(TypeExpr + WS + Identifier + WS);
        public static Rule UntypedLambdaParam = Node(Identifier + WS);
        public static Rule LambdaParam = TypedLambdaParam | UntypedLambdaParam;
        public static Rule LambdaParams = Node(UntypedLambdaParam | Parenthesize(CommaDelimited(LambdaParam)));
        public static Rule LambdaExpr = Node(LambdaParams + WS + MatchString("=>") + WS + (Block | RecExpr));
        public static Rule ArgList = Node(CharToken('(') + CommaDelimited(RecExpr) + CharToken(')'));
        public static Rule Indexer = Node(CharToken('[') + RecExpr + CharToken(']'));
        public static Rule Selector = Node(CharToken('.') + Identifier);
        public static Rule Inc = Node(MatchString("++"));
        public static Rule Dec = Node(MatchString("--"));
        public static Rule PostfixOp = Node(Inc | Dec | Indexer | ArgList | Selector);
        public static Rule PrefixOp = Node(At(SymbolChar) + MatchStringSet("++ -- ! - ~"));
        public static Rule BinaryOp = Node(At(SymbolChar) + MatchStringSet(">>= <<= <= >= == != << >> += -= *= %= /= && || ?? < > & | + - * % / ="));
        public static Rule ParenthesizedExpr = Node(Parenthesize(RecExpr));
        public static Rule FieldInitializer = Node(Identifier + WS + Eq + RecExpr);
        public static Rule TypeInitializerField = Node(FieldInitializer | Recursive(() => TypeInitializer) | RecExpr);
        public static Rule TypeInitializer = Node(CharToken('{') + CommaDelimited(TypeInitializerField) + CharToken('}'));
        public static Rule NewExpr = Node(Keyword("new") + Opt(TypeExpr) + WS + Opt(ArgList) + WS + Opt(TypeInitializer));
        public static Rule LeafExpr = Node((LambdaExpr | ParenthesizedExpr | NewExpr | Identifier | Integer | Float | String) + WS);
        public static Rule PrefixExpr = Node(PrefixOp + WS + Recursive(() => PrefixExpr) | LeafExpr);
        public static Rule UnaryExpr = Node(PrefixExpr + ZeroOrMore(PostfixOp + WS));
        public static Rule BinaryExpr = Node(UnaryExpr + ZeroOrMore(BinaryOp + WS + UnaryExpr));
        public static Rule TertiaryExpr = Node(BinaryExpr + Opt(CharToken('?') + RecExpr + WS + CharToken(':') + RecExpr));
        public static Rule Expr = Node(TertiaryExpr);

        static CSharpExprGrammar() { InitGrammar(typeof(CSharpExprGrammar));  }
    }
}
