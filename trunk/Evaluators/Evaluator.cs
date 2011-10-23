using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Diggins.Jigsaw
{
    public class Evaluator
    {
        public VarBindings env = new VarBindings();

        public dynamic AddBinding(string name, dynamic x)
        {
            env = env.AddBinding(name, x);
            return x;
        }

        public dynamic RebindOrCreate(string name, dynamic value)
        {
            var c = env.FindBindingOrDefault(name);
            return c != null ? c.Value = value : AddBinding(name, value);
        }

        public dynamic AddGlobal(string name, dynamic value)
        {
            var e = env; while (e.Tail != null) { Debug.Assert(e.Name != name); e = e.Tail; }
            e.Tail = new VarBindings { Name = name, Value = value }; ;
            return value;
        }

        public dynamic RebindOrCreateGlobalBinding(string name, dynamic value)
        {
            var c = env.FindBindingOrDefault(name);
            return c != null ? c.Value = value : AddGlobal(name, value);
        }

        public dynamic EvalScoped(Func<dynamic> f)
        {
            var e = env;
            var r = f();
            env = e;
            return r;
        }
    }
}
