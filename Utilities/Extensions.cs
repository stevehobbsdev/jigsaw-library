using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Xml.Linq;
using System.Linq.Expressions;

namespace Diggins.Jigsaw
{
    public delegate dynamic DynamicFunction(params dynamic[] args);
    public delegate dynamic DynamicFunc0();
    public delegate dynamic DynamicFunc1(dynamic a0);
    public delegate dynamic DynamicFunc2(dynamic a0, dynamic a1);
    public delegate dynamic DynamicFunc3(dynamic a0, dynamic a1, dynamic a2);
    public delegate dynamic DynamicMethod(dynamic self, params dynamic[] args);

    public static class Extensions
    {
        public static char LastChar(this string self)
        {
            return self[self.Length - 1];
        }
        
        public static string StripQuotes(this string self)
        {
            if (self.Length > 1 && ((self[0] == '"' && self.LastChar() == '"') || (self[0] == '\'' && self.LastChar() == '\'')))
                return self.Substring(1, self.Length - 2);
            else
                return self;
        }

        public static ExpandoObject AddField(this ExpandoObject self, string name, dynamic value)
        {
            var d = self as IDictionary<string, object>;
            d[name] = value;
            return self;
        }

        public static ExpandoObject AddMethod(this ExpandoObject self, string name, DynamicMethod m)
        {
            DynamicFunction f = (args) => m(self, args);
            return AddField(self, name, f);
        }

        public static IDictionary<string, object> AsDictionary(this ExpandoObject self) 
        { 
            return (IDictionary<string, object>)self; 
        }
        
        public static ICollection<KeyValuePair<string, object>> AsCollection(this ExpandoObject self)
        {
            return (ICollection<KeyValuePair<string, object>>)self; 
        }
    }
}
