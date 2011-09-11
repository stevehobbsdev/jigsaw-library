using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Diggins.Jigsaw
{
    public class Grammar
    {
        /// <summary>
        /// This rule should be used only with a named rule, since the name
        /// of the rule is used as the label.
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        public static Rule Node(Rule rule)
        {
            var result = new Rule();

            result.Function = state =>
            {
                var node = new Node(state.pos, result.Name, state.input);
                var oldNodes = state.nodes;
                state.nodes = new List<Node>();
                if (rule.Match(state))
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
            };

            return result;
        }

        /// <summary>
        /// This creates rules that can recursively refer to themselves
        /// directly or indirectly. 
        /// </summary>
        /// <param name="ruleGen"></param>
        /// <returns></returns>
        public static Rule Recursive(Func<Rule> ruleGen)
        {
            if (ruleGen == null) throw new ArgumentNullException();
            Rule rule = null;
            return new Rule(state =>
            {
                // Generate the rule if needed, but only once.
                rule = rule ?? ruleGen();
                return rule.Match(state);
            });
        }

        public static Rule At(Rule rule)
        {
            if (rule == null) throw new ArgumentNullException();
            return new Rule(state =>
            {
                var old = state.Clone();
                bool result = rule.Match(state);
                state.Assign(old);
                return result;
            });
        }

        public static Rule Seq(params Rule[] rs)
        {
            if (rs.Any(x => x == null)) throw new ArgumentNullException();
            return new Rule(state =>
            {
                var old = state.Clone();
                foreach (var r in rs)
                    if (!r.Match(state))
                    {
                        state.Assign(old);
                        return false;
                    }
                return true;
            });
        }

        public static Rule Choice(params Rule[] rs)
        {
            if (rs.Any(x => x == null)) throw new ArgumentNullException();
            return new Rule(state =>
            {
                var old = state.Clone();
                foreach (var r in rs)
                {
                    if (r.Match(state)) return true;
                    state.Assign(old);
                }
                return false;
            });
        }

        public static Rule End = new Rule(state => state.pos == state.input.Length);

        public static Rule Not(Rule r)
        {
            if (r == null) throw new ArgumentNullException();
            return new Rule(state =>
            {
                var old = state.Clone();
                if (r.Match(state))
                {
                    state.Assign(old);
                    return false;
                }
                return true;
            });
        }

        public static Rule ZeroOrMore(Rule r)
        {
            if (r == null) throw new ArgumentNullException();
            return new Rule(state => { while (r.Match(state)) { }; return true; });
        }

        public static Rule Repeat(int n, Rule r)
        {
            if (r == null) throw new ArgumentNullException();
            return new Rule(state =>
            {
                var old = state.Clone();
                for (int i = 0; i < n; ++i)
                {
                    if (!r.Match(state))
                    {
                        state.Assign(old);
                        return false;
                    }
                }
                return true;
            });
        }

        public static Rule OneOrMore(Rule r)
        {
            if (r == null) throw new ArgumentNullException();
            return Seq(r, ZeroOrMore(r));
        }

        public static Rule Opt(Rule r)
        {
            if (r == null) throw new ArgumentNullException();
            return new Rule(state => { r.Match(state); return true; });
        }

        public static Rule MatchString(string s)
        {
            if (String.IsNullOrEmpty(s)) throw new ArgumentException();
            return new Rule(state =>
            {
                if (!state.input.Substring(state.pos).StartsWith(s))
                    return false;
                state.pos += s.Length;
                return true;
            });
        }

        public static Rule MatchChar(Predicate<char> f)
        {
            if (f == null) throw new ArgumentNullException();
            return new Rule(state =>
            {
                if (End.Match(state))
                    return false;
                if (!f(state.input[state.pos]))
                    return false;
                state.pos++;
                return true;
            });
        }

        public static Rule MatchChar(char c)
        {
            return MatchChar(x => x == c);
        }

        public static Rule MatchRegex(Regex re)
        {
            if (re == null) throw new ArgumentNullException();
            return new Rule(state =>
            {
                var m = re.Match(state.input, state.pos);
                if (m == null || m.Index != state.pos) return false;
                state.pos += m.Length;
                return true;
            });
        }

        public static Rule CharSet(string s)
        {
            if (String.IsNullOrEmpty(s)) throw new ArgumentException();
            return MatchChar(c => s.Contains(c));
        }

        public static Rule CharRange(char a, char b)
        {
            return MatchChar(c => (c >= a) && (c <= b));
        }

        public static Rule ExceptCharSet(string s)
        {
            if (String.IsNullOrEmpty(s)) throw new ArgumentException();
            return MatchChar(c => !s.Contains(c));
        }

        public static Rule AnyChar = MatchChar(c => true);

        public static Rule AdvanceWhileNot(Rule r)
        {
            if (r == null) throw new ArgumentNullException();
            return ZeroOrMore(Seq(Not(r), AnyChar));
        }

        public static Rule Pattern(string s)
        {
            if (String.IsNullOrEmpty(s)) throw new ArgumentException();
            return MatchRegex(new Regex(s));
        }

        /// <summary>
        /// Provides a name to all fields of type Rule.
        /// </summary>
        public static void InitGrammar(Type type)
        {
            foreach (var fi in type.GetFields())
            {
                if (fi.FieldType.Equals(typeof(Rule)))
                {
                    var rule = fi.GetValue(null) as Rule;
                    if (rule == null)
                        throw new Exception("Unexpected null rule");
                    rule.Name = fi.Name;
                }
            }
        }
    }
}
