using System;                    
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;

namespace Diggins.Jigsaw
{
    public class JsonObject : DynamicObject    
    {
        static string Unquote(string s)
        {
            int n = s.Length;
            return (n > 2 && s.First() == '"' && s[n - 1] == '"')
                ? s.Substring(1, n - 2)
                : s;
        }

        public static readonly JsonObject Null = new JsonObject();

        ExpandoObject x = new ExpandoObject();

        public IDictionary<string, Object> AsDictionary
        {
            get { return x; }
        }

        public bool HasField(string s)
        {
            return AsDictionary.ContainsKey(Unquote(s));
        }

        public dynamic this[string name]
        {
            get 
            {
                return AsDictionary[Unquote(name)];
            }
            set
            {
                AsDictionary[Unquote(name)] = value;
            }
        }

        public dynamic this[int index]
        {
            get
            {
                return x.ElementAt(index).Value;
            }
            set
            {
                var key = x.AsCollection().ElementAt(index).Key;
                this[key] = value;
            }
        }

        public IEnumerable<KeyValuePair<string, Object>> KeyValues
        {
            get
            {
                return x;
            }
        }

        public StringBuilder BuildString(StringBuilder sb)
        {
            sb.AppendLine("{");
            int n = 0;
            foreach (var kv in KeyValues)
            {
                if (n++ > 0) sb.Append(", ");
                sb.AppendFormat("\"{0}\" : ", kv.Key);
                JsonValueToString(kv.Value, sb);
                sb.AppendLine();
            }
            sb.AppendLine("}");
            return sb;
        }

        public override string ToString()
        {
            return BuildString(new StringBuilder()).ToString();
        }

        public static StringBuilder JsonValueToString(dynamic value, StringBuilder sb)
        {
            if (value is JsonObject)
            {
                return value.BuildString(sb);
            }
            else if (value is List<dynamic>)
            {
                var xs = (List<dynamic>)value;
                sb.Append("[");
                for (int i = 0; i < xs.Count; ++i)
                {
                    if (i > 0) sb.Append(", ");
                    JsonValueToString(xs[i], sb);
                }
                sb.Append("]");
            }
            else
            {
                sb.Append(value.ToString());
                sb.Append(" ");
            }
            return sb;
        }

        public static dynamic Eval(Node n)
        {
            switch (n.Label)
            {
                case "Name":
                    return Eval(n[0]);
                case "Value":
                    return Eval(n[0]);
                case "Number":
                    return Eval(n[0]);
                case "Object":
                    {
                        var r = new JsonObject();
                        foreach (var pair in n.Nodes)
                        {
                            var name = pair[0].Text;
                            var value = Eval(pair[1]);
                            r[name] = value;
                        }
                        return r;
                    }
                case "Array":
                    return n.Nodes.Select(Eval).ToList();
                case "Integer":
                    return Int32.Parse(n.Text);
                case "Float":
                    return Double.Parse(n.Text);
                case "String":
                    return n.Text.Substring(1, n.Text.Length - 2);
                case "True":
                    return true;
                case "False":
                    return false;
                case "Null":
                    return new JsonObject();
                default:
                    throw new Exception("Unexpected node type " + n.Label);
            }
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return KeyValues.Select(kv => kv.Key);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;
            if (!HasField(binder.Name)) return false;
            result = this[binder.Name];
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            this[binder.Name] = value;
            return true;
        }

        public static JsonObject Parse(string s)
        {
            var nodes = JsonGrammar.Object.Parse(s);
            return Eval(nodes[0]);
        }
    }
}
