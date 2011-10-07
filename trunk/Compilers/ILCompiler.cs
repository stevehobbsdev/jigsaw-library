using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

using System.Diagnostics;

namespace Diggins.Jigsaw
{
    public class ILCompiler
    {
        public static Dictionary<string, OpCode> opLookup = new Dictionary<string, OpCode>();
        public Dictionary<string, Label> labels = new Dictionary<string, Label>();
        public Dictionary<string, LocalVariableInfo> vars = new Dictionary<string, LocalVariableInfo>();
        ILGenerator g;

        public ILCompiler(ILGenerator g)
        {
            this.g = g;
        }

        public static Type GetTypeFromNode(Node n)
        {
            return Utilities.GetType(n.Text);
        }

        public static MethodInfo GetMethodFromNode(Node n)
        {
            var r =  typeof(Primitives).GetMethod(n.Text);
            if (r == null) throw new Exception("Could not find method " + n.Text);
            return r;
        }

        public static OpCode GetOpCode(string s)
        {
            if (!opLookup.ContainsKey(s))
                throw new Exception("Could not find op-code " + s);
            return opLookup[s];
        }

        public Label GetOrCreateLabel(string s)
        {
            if (!labels.ContainsKey(s))
                labels.Add(s, g.DefineLabel());
            return labels[s];
        }

        public dynamic GetOperand(Node n)
        {
            Debug.Assert(n.Count == 2);
            string type = n[0].Text;
            string value = n[1].Text;
            switch (type.ToLower())
            {
                case "byte":    return Byte.Parse(value);
                case "int16":   return Int16.Parse(value);
                case "int32":   return Int32.Parse(value);
                case "int64":   return Int64.Parse(value);
                case "uint16":  return UInt16.Parse(value);
                case "uint32":  return UInt32.Parse(value);
                case "uint64":  return UInt64.Parse(value);
                case "float":   return float.Parse(value);
                case "double":  return double.Parse(value);
                case "decimal": return decimal.Parse(value);
                case "string":  return value.Substring(1, value.Length - 2);
                case "char":    return char.Parse(value.Substring(1, value.Length - 2));
                case "function": return GetMethodFromNode(n[1]);
                case "type":    return GetTypeFromNode(n[1]);
                case "var":     return vars[value];
                case "label":   return GetOrCreateLabel(value);
                default: throw new Exception("Unreocognized operand type " + type);
            }
        }

        public void EmitTerm(Node n)
        {
            switch (n.Label)
            {
                case "Label":
                    {
                        var name = n["Name"].Text;                        
                        g.MarkLabel(GetOrCreateLabel(name));
                    }
                    break;
                case "Statement":
                    {
                        EmitTerm(n[0]);
                    }
                    break;
                case "Block":
                    {
                        foreach (var n2 in n.Nodes)
                            EmitTerm(n2);
                    }
                    break;
                case "VarDecl":
                    {
                        var name = n["Name"].Text;
                        var type = GetTypeFromNode(n["DotName"]);
                        var decl = g.DeclareLocal(type);
                        vars.Add(name, decl);
                    }
                    break;
                case "OpStatement":
                    {
                        var opcode = GetOpCode(n[0].Text);
                        if (n.Count > 1)
                        {
                            dynamic d = GetOperand(n[1]);                                                       
                            g.Emit(opcode, d);
                        }
                        else
                        {
                            g.Emit(opcode);
                        }
                    }
                    break;
                default:
                    throw new Exception("Unrecognized node type " + n.Label);
            }
        }

        public static System.Reflection.Emit.DynamicMethod Eval(Node n)
        {
            var type = GetTypeFromNode(n["DotName"]);
            var name = n["Name"].Text;
            var types = n["ArgList"].GetNodes("Arg").Select(arg => GetTypeFromNode(arg["TypeName"])).ToArray();
            var r = new System.Reflection.Emit.DynamicMethod(name, type, types, true);
            var e = new ILCompiler(r.GetILGenerator());
            e.EmitTerm(n["Block"]);
            return r;
        }

        public static System.Reflection.Emit.DynamicMethod Eval(string s)
        {
            var nodes = ILGrammar.ILFunc.Parse(s);            
            return Eval(nodes[0]);
        }

        static ILCompiler()
        {
            // Get all of the OpCode values and add them to the opcode lookup table.
            foreach (var fi in typeof(OpCodes).GetFields())
                if (fi.FieldType.Equals(typeof(OpCode)) && fi.IsStatic)
                    opLookup.Add(fi.Name.ToLower(), (OpCode)fi.GetValue(null));
        }
    }
}
