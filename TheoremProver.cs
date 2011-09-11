using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rudiments.Miscellaney
{
    public class TheoremProver
    {
        public class Term
        {
            public List<Term> Arguments = new List<Term>(); 
            public int Arity { get { return Arguments.Count; } }
        }

        public class Atom : Term
        {
            public string Name;
            public bool Equivalent(Atom a) { return Name == a.Name; }
        }

        public class Variable : Atom
        {
        }        

        public class Constant : Atom
        {
        }

        public class Compound : Term
        {
            public Atom Functor;
        }

        public class Relation 
        {
            public Term Head;
            public Term Body;
        }

        public class Database
        {
            List<Term> Facts;
            List<Relation> Relations;
        }

        public class Bindings
        {
            Dictionary<string, Term> dictionary = new Dictionary<string,Term>();

            public Bindings()
            {
            }

            public Bindings(Bindings other)
            {
                foreach (var kv in other.dictionary)
                    dictionary.Add(kv.Key, kv.Value);
            }

            public void Add(Variable v, Term t)
            {
                if (dictionary.ContainsKey(v.Name))
                    throw new Exception(String.Format("{0} is already bound", v.Name));
                dictionary.Add(v.Name, t);
            }

            public Bindings Clone()
            {
                return new Bindings(this);
            }
        }

        public class Unifier
        {
            public class UnificationFailure : Exception { }
            public class CyclicalBinding : Exception { }

            public IEnumerable<Bindings> Unify(Term t1, Term t2, Bindings bindings) 
            {
                if (t1.Arity != t2.Arity)
                    throw new UnificationFailure();

                Bindings result = bindings.Clone();
                
                if (t1 is Variable)
                {
                    result.Add((Variable)t1, t2);
                    yield return result;
                }
                else if (t2 is Variable)
                {                    
                    bindings.Add((Variable)t2, t1);
                    yield return result;
                }
                else if (t1 is Constant && t2 is Constant)
                {
                    if (!((Constant)t1).Equals((Constant)t2))
                        throw new UnificationFailure();
                    yield return result;
                }
                else
                {
                    if (t1.Arity == 0)
                        yield return result;

                    // I think I need to make a list ofa all of the bindings that were added. 
                    // If a failure occurs, then this unification fails. 
                    // The other question is where does the variable matching come into play. 
                    for (int i = 0; i < t1.Arity; ++i)
                            foreach (var b in Unify(t1.Arguments[i], t2.Arguments[i], bindings))
                                yield return b;
                }
            }
        }
    }
}
