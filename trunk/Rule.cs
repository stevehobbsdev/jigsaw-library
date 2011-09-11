using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw
{
    public class Rule
    {
        public Rule(ParseFunction f)
        {
            Function = f;
        }

        public Rule()
        {
        }

        public ParseFunction Function { get; set; }
        public string Name { get; set; }

        public bool Match(ParserState state)
        {
            // TIP: Set conditional breakpoints here during debugging
            // where Name == "xxx" to trace specific rules. 
            return Function(state);
        }

        public static Rule operator +(Rule r1, Rule r2)
        {
            return Grammar.Seq(r1, r2);
        }

        public static Rule operator |(Rule r1, Rule r2)
        {
            return Grammar.Choice(r1, r2);
        }

        public override string ToString()
        {
            return Name ?? "anonymous";
        }
    }
}
