using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Diggins.Jigsaw
{
    public static class Primitives
    {
        public static void noop() { }
        public static dynamic add(dynamic a, dynamic b) { return a + b; }
        public static dynamic subtract(dynamic a, dynamic b) { return a - b; }
        public static dynamic multiply(dynamic a, dynamic b) { return a * b; }
        public static dynamic divide(dynamic a, dynamic b) { return a / b; }
        public static dynamic modulo(dynamic a, dynamic b) { return a % b; }
        public static dynamic negative(dynamic a) { return -a; }
        public static dynamic shift_left(dynamic a, dynamic b) { return a << b; }
        public static dynamic shift_right(dynamic a, dynamic b) { return a >> b; }
        public static dynamic gt(dynamic a, dynamic b) { return a > b; }
        public static dynamic lt(dynamic a, dynamic b) { return a < b; }
        public static dynamic gteq(dynamic a, dynamic b) { return a >= b; }
        public static dynamic lteq(dynamic a, dynamic b) { return a <= b; }
        public static dynamic eq(dynamic a, dynamic b) { return a.Equals(b); }
        public static dynamic hash(dynamic a) { return a.GetHashCode(); }
        public static dynamic neq(dynamic a, dynamic b) { return a != b; }
        public static dynamic cond(dynamic a, dynamic b, dynamic c) { return a ? b : c; }    
        public static dynamic not(dynamic a) { return !a; }
        public static dynamic or(dynamic a, dynamic b) { return a || b; }
        public static dynamic and(dynamic a, dynamic b) { return a && b; }
        public static dynamic xor(dynamic a, dynamic b) { return a ^ b; }
        public static dynamic bit_or(dynamic a, dynamic b) { return a | b; }
        public static dynamic bit_and(dynamic a, dynamic b) { return a & b; }
        public static dynamic complement(dynamic a) { return ~a; }
        public static dynamic invoke(dynamic f, dynamic args) { return f(args); }
        public static void invoke_void(dynamic f, dynamic args) { f(args); }
        public static dynamic @true() { return true; }
        public static dynamic @false() { return false; }
        public static dynamic succ(dynamic a) { return a + 1; }
        public static dynamic pred(dynamic a) { return a - 1; }
        public static dynamic head(dynamic a) { return a.Head; }
        public static dynamic tail(dynamic a) { return a.Tail; }
        public static dynamic nil() { return List.Empty; }
        public static dynamic pair(dynamic a, dynamic b) { return List.Cons(a, b); }
        public static dynamic type(dynamic a) { return a.GetType(); }
        public static void print(dynamic a) { Console.Write(a); }
        public static dynamic tostring(dynamic a) { return a.ToString(); } 
        
        public static MethodInfo GetMethod(string s)
        {
            return typeof(Primitives).GetMethod(s);
        }

        public static string UnaryOperatorToMethodName(string s)
        {
            switch(s)
            {
                case "-": return "negative";
                case "!": return "not";
                case "~": return "complement";
                default: throw new Exception("Not a recognized unary operator " + s);
            }
        }

        public static string BinaryOperatorToMethodName(string s)
        {
            switch (s)
            {
                case "+": return "add";
                case "-": return "subtract";
                case "*": return "multiply";
                case "/": return "divide";
                case "%": return "modulo";
                case ">>": return "shl";
                case "<<": return "shr";
                case ">": return "gt";
                case "<": return "lt";
                case ">=": return "gteq";
                case "<=": return "lteq";
                case "==": return "eq";
                case "!=": return "neq";
                case "||": return "or";
                case "&&": return "and";
                case "^": return "xor";
                case "|": return "bit_or";
                case "&": return "bit_nand";
                default: throw new Exception("Not a recognized operator");
            }
        }

        public static MethodInfo GetMethodFromBinaryOperator(string s)
        {
            return GetMethod(BinaryOperatorToMethodName(s));
        }
    }
}
