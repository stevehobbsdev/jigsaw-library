using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LambdaCalculus
{
    class Program
    {
        public delegate Lambda Lambda(Lambda x);

        public static Lambda Id = (x) => x;
        public static Lambda Zero = (f) => (x) => x;
        public static Lambda True = (x) => (y) => x;
        public static Lambda False = (x) => (y) => y;
        public static Lambda One = (f) => (x) => f(x);
        public static Lambda Two = (f) => (x) => f(f(x));
        public static Lambda Succ = (n) => (f) => (x) => f(n(f)(x));
        public static Lambda Three = Succ(Two);
        public static Lambda Pred = (n) => (f) => (x) => n((g) => (h) => h(g(f)))((u) => x)(Id);
        public static Lambda Plus = (m) => (n) => (f) => (x) => m(f)(n(f)(x));
        public static Lambda Sub = (m) => (n) => n (Pred) (m);
        public static Lambda And = (p) => (q) => p(q)(p);
        public static Lambda Or = (p) => (q) => p(p)(q);
        public static Lambda Not = (p) => (a) => (b) => p(b)(a);
        public static Lambda IfThenElse = (p) => (a) => (b) => p(a)(b);
        public static Lambda IsZero = (n) => n((x) => False)(True);
        public static Lambda IsLtEqOne = (n) => IsZero(Pred(n));
        public static Lambda Pair = (x) => (y) => (f) => f(x)(y);
        public static Lambda First = (pair) => pair(True);
        public static Lambda Second = (pair) => pair(False);
        public static Lambda Nil = (x) => True;
        public static Lambda Null = (p) => p((x) => (y) => False);
        public static Lambda LtEq = (x) => (y) => IsZero(Sub(x)(y));
        public static Lambda Gt = (x) => (y) => LtEq(y)(x);
        public static Lambda Eq = (x) => (y) => And(LtEq(x)(y))(LtEq(y)(x));

        public static Lambda Write(string s) { return (x) => { Console.WriteLine(s); return x; }; }

        public static void TestLambdaCalculus()
        {
            Console.WriteLine("Expect true");
            True(Write("True"))(Write("False"))(Id);

            Console.WriteLine("Expect false");
            False(Write("True"))(Write("False"))(Id);

            Console.WriteLine("Expect zero");
            IsZero(Zero)(Write("is zero"))(Write("is not zero"))(Id);

            Console.WriteLine("Expect zero");
            IsZero(Pred(One))(Write("is zero"))(Write("is not zero"))(Id);

            Console.WriteLine("Expect not zero");
            IsZero(One)(Write("is zero"))(Write("is not zero"))(Id);

            Console.WriteLine("Expect not zero");
            IsZero(Two)(Write("is zero"))(Write("is not zero"))(Id);

            Console.WriteLine("Expect not zero");
            IsZero(Succ(Zero))(Write("is zero"))(Write("is not zero"))(Id);
            
            Console.WriteLine("Expect <= one");
            IsLtEqOne(One)(Write("is <= one"))(Write("is > one"))(Id);

            Console.WriteLine("Expect <= one");
            IsLtEqOne(Zero)(Write("is <= one"))(Write("is > one"))(Id);

            Console.WriteLine("Expect > one");
            IsLtEqOne(Two)(Write("is <= one"))(Write("is > one"))(Id);

            Console.ReadKey();
        }
    }
}
