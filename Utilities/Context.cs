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

        public Context FindContextOrDefault(string name)
        {
            return Contexts.FirstOrDefault(c => c.Name == name);
        }

        public Context FindContext(string name)
        {
            var r = FindContextOrDefault(name);
            if (r == null) throw new Exception("Name does not exist in context: " + name);
            return r; 
        }
        
        public object Find(string name)
        {
            return FindContext(name).Value;
        }
        
        public object this[string name]
        {
            get { return Find(name); }
        }

        public dynamic Assign(string name, dynamic value)
        {
            return FindContext(name).Value = value;
        }

        public dynamic AssignOrCreate(string name, dynamic value)
        {
            var c = FindContextOrDefault(name);
            return c != null ? c.Value = value : AddContext(name, value);
        }
    }
}
