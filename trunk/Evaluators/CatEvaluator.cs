using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;

namespace Diggins.Jigsaw
{    
    public class CatEvaluator
    {
        public class QuotedValue 
        {
            public QuotedValue(object v) { Value = v; }
            public dynamic Value;
        }
        
        public class Composition 
        {
            public Composition(object f, object g) { F = f; G = g; }
            public dynamic F;
            public dynamic G;
        }

        public class Environment
        {
            public class Callable : Attribute { }

            public Environment()
            {
                foreach (var mi in typeof(Primitives).GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static))
                    Functions.Add(mi.Name, mi);
                
                foreach (var mi in GetType().GetMethods())
                    if (mi.GetCustomAttributes(typeof(Callable), true).Length > 0)
                        Functions.Add(mi.Name, mi);
            }

            public Dictionary<string, dynamic> Functions = new Dictionary<string, dynamic>();

            public Stack<dynamic> Values = new Stack<dynamic>();
            
            public dynamic Pop() { return Values.Pop(); }            
            public dynamic Peek { get { return Values.Peek(); }  }
            public void Push(dynamic o) { Values.Push(o); }

            [Callable] public void Zap() { Pop(); }
            [Callable] public void Swap() { var a = Pop(); var b = Pop(); Push(a); Push(b); }
            [Callable] public void Apply() { Apply(Pop()); }
            [Callable] public void Dup() { Push(Peek); }
            [Callable] public void Quote() { Push(new QuotedValue(Pop())); }
            [Callable] public void Compose() { Push(new Composition(Pop(), Pop())); }
            [Callable] public void Nil() { Push(List.Empty); }
            [Callable] public void IsNil() { Push(List.Empty == Peek()); }
            [Callable] public void Cons() { Push(List.Cons(Pop(), Pop())); } 
            [Callable] public void Uncons() { var xs = Pop(); Push(xs.Tail); Push(xs.Head); } 
            [Callable] public void While() { var body = Pop(); while (Pop()) body.Apply(this); }
            [Callable] public void MakeList() { var q = Pop(); var stk = Values; Values = new Stack<dynamic>(); Apply(q); stk.Push(List.Build(Values)); Values = stk; }
            
            public void Eval(Node n) 
            {
                switch (n.Label)
                {
                    case "Integer": 
                        Push(Int32.Parse(n.Text)); 
                        break;
                    case "String": 
                        Push(n.Text.Substring(1, n.Text.Length - 2)); 
                        break;
                    case "Quotation": 
                        Push(n); 
                        break;
                    case "Terms": 
                        foreach (var t in n.Nodes) 
                            Eval(t); 
                        break;
                    case "Term": 
                        Eval(n.Nodes[0]); 
                        break;
                    case "Atom":
                        Eval(n.Nodes[0]);
                        break;
                    case "Symbol":
                        if (!Functions.ContainsKey(n.Text))
                            throw new Exception("Unrecognized symbol " + n.Text);
                        Eval(Functions[n.Text]); 
                        break;
                    case "Define": 
                        Functions.Add(n.GetNode("Symbol").Text, n.GetNode("Terms")); 
                        break;
                    default: 
                        throw new Exception("Can't evaluate node of type " + n.Label);
                }
            }

            public void Eval(MethodInfo mi)
            {
                var args = new List<dynamic>();
                for (int i = 0; i < mi.GetParameters().Length; ++i)
                    args.Add(Pop());                
                var result = mi.Invoke(this, args.ToArray());
                if (mi.ReturnType != typeof(void))
                    Push(result);
            }

            public void Apply(Node n)
            {
                if (n.Label != "Quotation") 
                    throw new Exception("Expected a quotation on the stack");
                Eval(n.GetNode("Terms"));
            }

            public void Apply(Composition c)
            {
                Apply(c.F);
                Apply(c.G);
            }

            public void Apply(QuotedValue q)
            {
                Push(q.Value);
            }
        }

        public static Stack<dynamic> Eval(Node node)
        {
            var env = new Environment();
            env.Eval(node);
            return env.Values;
        }

        public static Stack<dynamic> Eval(string s)
        {
            var nodes = CatGrammar.Terms.Parse(s);
            if (nodes == null) throw new Exception("Invalid Cat program");
            return Eval(nodes[0]);
        }
    }
}
