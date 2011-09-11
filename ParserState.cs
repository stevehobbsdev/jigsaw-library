using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw
{
    public class ParserState
    {
        public string input;
        public int pos;
        public List<Node> nodes = new List<Node>();
        public string Current { get { return input.Substring(pos); } }
        public ParserState Clone() { return new ParserState { input = input, pos = pos, nodes = new List<Node>(nodes) }; }
        public void Assign(ParserState state) { input = state.input; pos = state.pos; nodes = state.nodes; }
        public void RestoreAfter(Action action) { var state = Clone(); action(); Assign(state); }
        public override string ToString() { return String.Format("{0}/{1}", pos, input.Length); }
    }
}
