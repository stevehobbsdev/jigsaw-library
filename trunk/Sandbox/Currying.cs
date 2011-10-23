using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw
{
    class Currying
    {
        public static Func<A, Func<B, R>> Curry<A, B, R>(Func<A, B, R> f)
        {
            return a => b => f(a, b);
        }

        public static Func<A, Func<B, Func<C, R>>> Curry<A, B, C, R>(Func<A, B, C, R> f)
        {
            return a => b => c => f(a, b, c);
        }

        public static Func<A, Func<B, Func<C, Func<D, R>>>> Curry<A, B, C, D, R>(Func<A, B, C, D, R> f)
        {
            return a => b => c => d => f(a, b, c, d);
        }
        
        public static Func<B, R> PartialApplyFirst<A, B, R>(Func<A, B, R> f, A a)
        {
            return b => f(a, b);
        }

        public static Func<A, R> PartialApplySecond<A, B, R>(Func<A, B, R> f, B b)
        {
            return a => f(a, b);
        }

        public static Func<R> PartialApplyBoth<A, B, R>(Func<A, B, R> f, A a, B b)
        {
            return () => f(a, b);
        }
    }
}
