using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw
{
    public static class SKCalculus
    {
        public delegate Combinator Combinator(Combinator c);

        public static Combinator S = (a) => (b) => (c) => a(c)(b(c));
        public static Combinator K = (a) => (b) => a;
        public static Combinator I = (S(K))(K);         // (a) => a
        public static Combinator B = S(K(S))(K);        // (a) => (b) => (c) => a(b(c))
        public static Combinator C = S(B(B)(S))(K(K));  // (a) => (b) => (c) => a(c)(b)
        public static Combinator E = B(B(B)(B));        // (a) => (b) => (c) => (d) => (e) => a(b)(c(d)(e))
        public static Combinator G = B(B)(C);           // (a) => (b) => (c) => (d) => a(b)(c(d))
        public static Combinator M = S(I)(I);           // (a) => a(a)
        public static Combinator L = C(B)(M);           // (a) => (b) => a(b(b))
        public static Combinator O = S(I);              // (a) => (b) => b(a(b))
        public static Combinator Q = C(B);              // (a) => (b) => (c) => b(a(c))
        public static Combinator T = C(I);              // (a) => (b) => b(a)
        public static Combinator F = E(T)(T)(E)(T);     // (a) => (b) => (c) => c(b)(a)
        public static Combinator R = B(B)(T);           // (a) => (b) => (c) => b(c)(a)
        public static Combinator U = L(O);              // (a) => (b) => b(a(a)(b))
        public static Combinator V = B(C)(T);           // (a) => (b) => (c) => c(a)(b)
        public static Combinator W = C(B(M)(R));        // (a) => (b) => a(b)(b)
        public static Combinator Y = S(L)(L);           // (a) => ((b) => a(bb) ((b) => a(bb)))

        public static Combinator Zero = K(I);
        public static Combinator True = K;
        public static Combinator False = Zero;

        public static Combinator Write(int x) { return (Combinator c) => { Console.WriteLine(x); return c; }; }

        public static void Test()
        {
            Console.WriteLine("Test S : expect 1 2 3 3");
            S(Write(1))(Write(2))(Write(3))(S);

            Console.WriteLine("Test K : expect 1");
            K(Write(1))(Write(2))(S);

            Console.WriteLine("Test I : expect 1");
            I(Write(1))(S);

            Console.WriteLine("Test B : expect 2 1 3");
            B(Write(1))(Write(2))(Write(3))(S);

            Console.WriteLine("Test C : expect 1 3 2");
            C(Write(1))(Write(2))(Write(3))(S);

            Console.WriteLine("Test E : expect 1 3 4 2 5");
            E(Write(1))(Write(2))(Write(3))(Write(4))(Write(5))(S);

            Console.WriteLine("Test M : expect 1 1");
            M(Write(1))(S);

            Console.WriteLine("Test L : expect 2 1 2");
            L(Write(1))(Write(2))(S);

            Console.WriteLine("Test O : expect 1 2 2");
            O(Write(1))(Write(2))(S);

            Console.WriteLine("Test Q : expect 1 2 3");
            Q(Write(1))(Write(2))(Write(3))(S);

            Console.WriteLine("Test T : expect 2 1");
            T(Write(1))(Write(2))(S);

            Console.WriteLine("Test F : expect 3 2 1");
            F(Write(1))(Write(2))(Write(3))(S);

            Console.WriteLine("Test R : expect 2 3 1");
            R(Write(1))(Write(2))(Write(3))(S);

            Console.WriteLine("Test U : expect 1 1 2 2");
            U(Write(1))(Write(2))(S);

            Console.WriteLine("Test V : expect 3 1 2");
            V(Write(1))(Write(2))(Write(3))(S);

            Console.WriteLine("Test W : expect 1 2 2");
            W(Write(1))(Write(2))(S);
        }
    }
}
