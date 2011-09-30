using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Diggins.Jigsaw
{
    public abstract class Rule
    {
        public Rule(IEnumerable<Rule> rules)
        {
            if (rules.Any(r => r == null)) throw new Exception("No child rule can be null");
            Children = new List<Rule>(rules);
        }

        public Rule(params Rule[] rules)
            : this((IEnumerable<Rule>)rules)
        {
        }

        public List<Rule> Children = new List<Rule>();

        public virtual string Name { get; set; }

        public Rule Child { get { return Children[0]; } }

        protected abstract bool InternalMatch(ParserState state);

        public bool Match(ParserState state)
        {
            // HINT: This is a good place to set a conditional break-point when debugging.
            // Using the Name = X as a condition.
            return InternalMatch(state);            
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

    public class AtRule : Rule
    {
        public AtRule(Rule r)
            : base(r)
        { }

        protected override bool InternalMatch(ParserState state)
        {
            var old = state.Clone();
            bool result = Child.Match(state);
            state.Assign(old);
            return result;
        }

        public override string ToString()
        {
            return String.Format("At{0}", Child.ToString());
        }
    }

    public class NotRule : Rule
    {
        public NotRule(Rule r)
            : base(r)
        { }

        protected override bool InternalMatch(ParserState state)
        {
            var old = state.Clone();
            if (Child.Match(state))
            {
                state.Assign(old);
                return false;
            }
            return true;
        }

        public override string ToString()
        {
            return String.Format("Not({0})", Child.ToString());
        }
    }

    public class NodeRule : Rule
    {
        public readonly bool UseCache = false;

        public NodeRule(Rule r)
            : base(r)
        { }

        protected override bool InternalMatch(ParserState state)
        {
            try
            {
                if (UseCache)
                    return InternalMatchWithCaching(state);
                else
                    return InternalMatchWithoutCaching(state);
            }
            catch (Exception e)
            {
                Console.WriteLine("While parsing rule {0}, an error occured: {1}", Name, e.Message);
                throw;
            }
        }

        private bool InternalMatchWithCaching(ParserState state)
        {
            Node node;

            int start = state.pos;

            if (state.GetCachedResult(this, out node))
            {
                if (node == null)
                    return false;

                state.pos = node.End;
                state.nodes.Add(node);
                return true;
            }

            node = new Node(state.pos, Name, state.input);
            var oldNodes = state.nodes;
            state.nodes = new List<Node>();

            if (Child.Match(state))
            {
                node.End = state.pos;
                node.Nodes = state.nodes;
                oldNodes.Add(node);
                state.nodes = oldNodes;
                state.CacheResult(this, start, node);
                return true;
            }
            else
            {
                state.nodes = oldNodes;
                state.CacheResult(this, start, null);
                return false;                
            }
        }


        private bool InternalMatchWithoutCaching(ParserState state)
        {
            Node node;

            node = new Node(state.pos, Name, state.input);
            var oldNodes = state.nodes;
            state.nodes = new List<Node>();
                
            if (Child.Match(state))
            {
                node.End = state.pos;
                node.Nodes = state.nodes;
                oldNodes.Add(node);
                state.nodes = oldNodes;
                return true;
            }
            else
            {
                state.nodes = oldNodes;
                return false;
            }
        }
    }

    public class RecursiveRule : Rule
    {
        Func<Rule> ruleGen;

        public RecursiveRule(Func<Rule> ruleGen)
        {
            this.ruleGen = ruleGen;
        }

        protected override bool InternalMatch(ParserState state)
        {
            if (Children.Count == 0)
                Children.Add(ruleGen());
            return Child.Match(state);
        }

        public override string ToString()
        {
            return Name ?? (Children.Count > 0 ? Children[0].ToString() : "recursive");
        }
    };

    public class SeqRule : Rule
    {
        public SeqRule(params Rule[] rs)
            : base(rs)
        { }

        protected override bool InternalMatch(ParserState state)
        {
            var old = state.Clone();
            foreach (var r in Children)
                if (!r.Match(state))
                {
                    state.Assign(old);
                    return false;
                }
            return true;
        }

        public override string ToString()
        {
            return Name ?? String.Join(" + ", Children);
        }
    }

    public class ChoiceRule : Rule
    {
        public ChoiceRule(params Rule[] rs)
            : base(rs)
        {
        }

        protected override bool InternalMatch(ParserState state)
        {
            var old = state.Clone();
            foreach (var r in Children)
            {
                if (r.Match(state)) return true;
                state.Assign(old);
            }
            return false;
        }

        public override string ToString()
        {
            return Name ?? String.Join(" | ", Children);
        }
    }

    public class EndRule : Rule
    {
        protected override bool InternalMatch(ParserState state)
        {
            return state.pos == state.input.Length;
        }

        public override string ToString()
        {
            return "_end_";
        }
    }

    public class StarRule : Rule
    {
        public StarRule(Rule r)
            : base(r)
        { }

        protected override bool InternalMatch(ParserState state)
        {
            while (Child.Match(state)) { };
            return true;
        }

        public override string ToString()
        {
            return Child.ToString() + "*";
        }
    }

    public class PlusRule : Rule
    {
        public PlusRule(Rule r)
            : base(r)
        { }

        protected override bool InternalMatch(ParserState state)
        {
            if (!Child.Match(state)) return false;
            while (Child.Match(state)) { }
            return true;
        }

        public override string ToString()
        {
            return Child.ToString() + "+";
        }
    }

    public class OptRule : Rule
    {
        public OptRule(Rule r)
            : base(r)
        { }

        protected override bool InternalMatch(ParserState state)
        {
            Child.Match(state);
            return true;
        }

        public override string ToString()
        {
            return Child.ToString() + "?";
        }
    }

    public class StringRule : Rule
    {
        string s;

        public StringRule(string s)
        {
            this.s = s;
        }

        protected override bool InternalMatch(ParserState state)
        {
            if (!state.input.Substring(state.pos).StartsWith(s))
                return false;
            state.pos += s.Length;
            return true;
        }

        public override string ToString()
        {
            return Name ?? s;
        }
    }

    public class CharRule : Rule
    {
        Predicate<char> predicate;

        public CharRule(Predicate<char> p)
        {
            predicate = p;
        }

        protected override bool InternalMatch(ParserState state)
        {
            if (state.pos >= state.input.Length)
                return false;
            if (!predicate(state.input[state.pos]))
                return false;
            state.pos++;
            return true;
        }
    }

    public class RegexRule : Rule
    {
        Regex re;

        public RegexRule(Regex re)
        {
            this.re = re;
        }

        protected override bool InternalMatch(ParserState state)
        {
            var m = re.Match(state.input, state.pos);
            if (m == null || m.Index != state.pos) return false;
            state.pos += m.Length;
            return true;
        }
    }
}
