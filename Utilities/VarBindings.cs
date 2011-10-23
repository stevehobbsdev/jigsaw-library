using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw
{
    public class VarBindings
    {
        public string Name;
        public object Value;
        public VarBindings Tail;
        
        public VarBindings AddBinding(string name, object value)
        {
            return new VarBindings { Name = name, Value = value, Tail = this };
        }
        
        public IEnumerable<VarBindings> Bindings
        {
            get
            {
                for (var b = this; b != null; b = b.Tail)
                    yield return b;
            }
        }

        public VarBindings FindBindingOrDefault(string name)
        {
            return Bindings.FirstOrDefault(c => c.Name == name);
        }

        public VarBindings FindBinding(string name)
        {
            var r = FindBindingOrDefault(name);
            if (r == null) throw new Exception("Name does not exist in context: " + name);
            return r; 
        }
        
        public object Find(string name)
        {
            return FindBinding(name).Value;
        }
        
        public object this[string name]
        {
            get { return Find(name); }
        }

        public dynamic Bind(string name, dynamic value)
        {
            return FindBinding(name).Value = value;
        }
    }
}
