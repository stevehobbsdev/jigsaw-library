using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Diggins.Jigsaw
{
    public delegate bool ParseFunction(ParserState state);

    public class Parser
    {
        public static List<Node> Parse(string input, Rule r)
        {
            var state = new ParserState() { input = input, pos = 0 };
            if (!r.Match(state))
                return null;
            else
                return state.nodes;
        }
    }


}


