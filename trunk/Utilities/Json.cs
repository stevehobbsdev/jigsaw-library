using System;                    
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;

namespace Diggins.Jigsaw
{
    public class JsonObject : DynamicObject
    {
        ExpandoObject x = new ExpandoObject();

        public IDictionary<string, Object> AsDictionary
        {
            get { return x; }
        }

        public bool ValidJSONField(object o)
        {
            var t = o.GetType();
            return t == typeof(Int32) 
                || t == typeof(String) 
                || t == typeof(Double)
                || t == typeof(JsonObject)
                || t == typeof(List<dynamic>)
                || t == typeof(bool);
        }

        public bool HasField(string s)
        {
            return AsDictionary.ContainsKey(s);
        }

        public dynamic this[string name]
        {
            get 
            {
                return AsDictionary[name];
            }
            set
            {
                if (!ValidJSONField(value))
                    throw new ArgumentException();

                x.AsDictionary()[name] = value;
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

        public static dynamic JsonValueFromAst(Node node)
        {
            switch (node.Label)
            {
                case "Name":
                    return JsonValueFromAst(node[0]);
                case "Value":
                    return JsonValueFromAst(node[0]);
                case "Number":
                    return JsonValueFromAst(node[0]);
                case "Object":
                    {
                        var r = new JsonObject();
                        foreach (var pair in node["Members"].Nodes)
                        {
                            var name = pair["Name"]["String"]["StringChars"].Text;
                            var value = JsonValueFromAst(pair["Value"]);
                            r[name] = value;
                        }
                        return r;
                    }
                case "Array":
                    return node["Elements"].Nodes.Select(JsonValueFromAst).ToList();
                case "Integer":
                    return Int32.Parse(node.Text);
                case "Float":
                    return Double.Parse(node.Text);
                case "String":
                    return node.Text.Substring(1, node.Text.Length - 2);
                case "True":
                    return true;
                case "False":
                    return false;
                case "Null":
                    return new JsonObject();
                default:
                    throw new Exception("Unexpected node type " + node.Label);
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
            if (!HasField(binder.Name)) return false;
            this[binder.Name] = value;
            return true;
        }

        public static JsonObject Parse(string s)
        {
            var nodes = Parser.Parse(s, Grammars.JsonGrammar.Object);
            if (nodes == null)
                throw new Exception("Parser failed");
            return JsonValueFromAst(nodes[0]);
        }
    }
}
