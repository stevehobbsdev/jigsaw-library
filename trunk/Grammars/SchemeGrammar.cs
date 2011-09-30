using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw.Grammars
{
    /// <summary>
    /// For a short introduction to Scheme see: http://cs.gettysburg.edu/~tneller/cs341/scheme-intro/index.html
    /// See also: http://people.cs.aau.dk/~normark/scheme/r4rs/r4rs_toc.htm#SEC2
    /// </summary>
    class SchemeGrammar : SharedGrammar
    {
        public static Rule Symbol = SExpressionGrammar.Symbol;
        public static Rule Atom = SExpressionGrammar.Atom;
        public static Rule SExpr = SExpressionGrammar.SExpr;

        public static Rule Quote = CharToken('`') + SExpr;
        public static Rule Primitive = Recursive(() => Lambda | If | Let | Begin | Define);
        public static Rule Term = (Atom | Primitive | Quote);
        public static Rule Param = Node(Symbol);
        public static Rule ParamList = Node(Parenthesize(ZeroOrMore(Param + WS)));
        public static Rule Lambda = Node(Parenthesize(Keyword("lambda") + ParamList + Term));
        public static Rule Begin = Node(Keyword("begin") + OneOrMore(Term));
        public static Rule Define = Node(Keyword("define") + Symbol + WS + Term);
        public static Rule VarBinding = Node(Parenthesize(Symbol + WS + Term));
        public static Rule Let = Node(Keyword("let") + Parenthesize(ZeroOrMore(VarBinding)) + Term);
        public static Rule If = Node(Parenthesize(Keyword("if") + Term + Term + Opt(Term)));
    }
}
