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
            try
            {
                if (!r.Match(state))
                    throw new Exception("Rule failed to match");
                return state.nodes;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occured while parsing: " + e.Message);
                return null;
            }
        }
    }


}


