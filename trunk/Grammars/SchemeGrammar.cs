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
        public static Rule Symbol = SExprGrammar.Symbol;
        public static Rule Atom = SExprGrammar.Atom;
        public static Rule SExpr = SExprGrammar.SExpr;

        public static Rule Quote = CharToken('`') + SExpr;
        public static Rule Primitive = Recursive(() => Lambda | If | Let | Begin | Define);
        public static Rule Term = (Atom | Primitive | Quote);
        public static Rule Terms = ZeroOrMore(Term + WS);
        public static Rule ParamList = Node(Paranthesize(ZeroOrMore(Symbol + WS)));
        public static Rule Lambda = Node(Paranthesize(Keyword("lambda") + ParamList + Terms));
        public static Rule Begin = Node(Keyword("begin") + OneOrMore(Term));
        public static Rule Define = Node(Keyword("define") + Symbol + WS + Term);
        public static Rule VarBinding = Node(Paranthesize(Symbol + WS + Term));
        public static Rule Let = Node(Keyword("let") + Paranthesize(ZeroOrMore(VarBinding)) + Term);
        public static Rule If = Node(Paranthesize(Keyword("if") + Term + Term + Opt(Term)));
    }
}
