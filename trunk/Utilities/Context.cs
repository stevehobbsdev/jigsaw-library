using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw
{
    public class Context
    {
        public string Name;
        public object Value;
        public Context Tail;
        
        public Context AddContext(string name, object value)
        {
            return new Context { Name = name, Value = value, Tail = this };
        }
        
        public IEnumerable<Context> Contexts
        {
            get
            {
                for (Context c = this; c != null; c = c.Tail)
                    yield return c;
            }
        }
        
        public object Find(string name)
        {
            var r = Contexts.FirstOrDefault(c => c.Name == name);
            if (r == null) throw new Exception("Name does not exist in context: " + name);
            return r.Value;
        }
        
        public object this[string name]
        {
            get { return Find(name); }
        }
    }
}
