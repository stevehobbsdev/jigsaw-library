using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw
{
    public class CatGrammar : SharedGrammar
    {
        public static Rule Atom = SExpressionGrammar.Atom;
        public static Rule Symbol = SExpressionGrammar.Symbol;

        public static Rule Term = Node(Recursive(() => Define | Atom | Quotation) + WS);
        public static Rule Terms = Node(ZeroOrMore(Term));
        public static Rule Quotation = Node(CharToken('[') + Terms + CharToken(']'));
        public static Rule Define = Node(MatchString("def") + WS + Symbol + WS + Quotation);

        static CatGrammar() { InitGrammar(typeof(CatGrammar)); }
    }
}
