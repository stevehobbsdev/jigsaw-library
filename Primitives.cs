using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw
{
    public static class Primitives
    {
        public static void Noop() { }
        public static dynamic Add(dynamic a, dynamic b) { return a + b; }
        public static dynamic Subtract(dynamic a, dynamic b) { return a - b; }
        public static dynamic Multiply(dynamic a, dynamic b) { return a * b; }
        public static dynamic Divide(dynamic a, dynamic b) { return a / b; }
        public static dynamic Modulo(dynamic a, dynamic b) { return a % b; }
        public static dynamic Negative(dynamic a) { return -a; }
        public static dynamic Positive(dynamic a) { return +a; }
        public static dynamic ShiftLeft(dynamic a, dynamic b) { return a << b; }
        public static dynamic ShiftRight(dynamic a, dynamic b) { return a >> b; }
        public static dynamic GreaterThan(dynamic a, dynamic b) { return a > b; }
        public static dynamic LessThan(dynamic a, dynamic b) { return a < b; }
        public static dynamic GreaterThanOrEquals(dynamic a, dynamic b) { return a >= b; }
        public static dynamic LessThanOrEquals(dynamic a, dynamic b) { return a <= b; }
        new public static dynamic Equals(dynamic a, dynamic b) { return a == b; }
        public static dynamic NotEquals(dynamic a, dynamic b) { return a != b; }
        public static dynamic Conditional(dynamic a, dynamic b, dynamic c) { return a ? b : c; }    
        public static dynamic Not(dynamic a) { return !a; }
        public static dynamic LogicalOr(dynamic a, dynamic b) { return a || b; }
        public static dynamic LogicalAnd(dynamic a, dynamic b) { return a && b; }
        public static dynamic XOr(dynamic a, dynamic b) { return a ^ b; }
        public static dynamic BitwiseOr(dynamic a, dynamic b) { return a | b; }
        public static dynamic BitwiseAnd(dynamic a, dynamic b) { return a & b; }
        public static dynamic Complement(dynamic a) { return ~a; }
        public static dynamic Invoke(dynamic f, dynamic args) { return f(args); }
        public static dynamic True() { return true; }
        public static dynamic False() { return false; }
        public static dynamic Succ(dynamic a) { return a + 1; }
        public static dynamic Pred(dynamic a) { return a - 1; }       
        public static void Execute(dynamic f, dynamic args) { f(args); }
    }
}
