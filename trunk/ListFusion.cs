using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw
{
    class ListFusion
    {
        public static IEnumerable<ResultT> Comprehension<SourceT, ResultT>(IEnumerable<SourceT> source, 
            Func<SourceT, ResultT> transform, Func<SourceT, bool> predicate)
        {
            return source.Where(predicate).Select(transform);
        }

        public static IEnumerable<ResultT> Map<SourceT, ResultT>(IEnumerable<SourceT> source,
            Func<SourceT, ResultT> transform)
        {
            return Comprehension(source, transform, x => true);
        }

        public static IEnumerable<SourceT> Filter<SourceT>(IEnumerable<SourceT> source,
            Func<SourceT, bool> predicate)
        {
            return Comprehension(source, x => x, predicate);
        }


        /*
        public static U GenFold<T, U>(T x, U init, Func<U, T, U> f, Predicate<T> halt, Func<T, T> next)
        {
            var result = init;
            while (!halt(x))
            {
                result = f(result, x);
                x = next(x);
            }
            return result; 
        }

        public static U GenFold<T, U>(T x, U init, Func<U, U, U> f, Predicate<T> halt, Predicate<T> select, Func<T, U> map, Func<T, T> next)
        {
            return GenFold(x, init, (u, t) => select(t) ? f(map(t), u) : u;  
        }
        
        public static dynamic Fold(Func<object, object, object> f, object acc, List xs)
        {
            return GenFold<List, object>(xs, acc, f, ys => ys.IsEmpty, ys => true, ys => ys, ys => ys.Tail);
        }

        public static dynamic Map(Func<object, object> f, List xs)
        {
            return GenFold(xs, List.Empty, (y, ys) => List.Cons(y, (List)ys), ys => ((List)ys).IsEmpty, y => true, f, ys => xs.Tail);
        }

        public static dynamic Filter(Predicate<object> p, List xs)
        {
            return GenFold(xs, List.Empty, (y, ys) => List.Cons(y, (List)ys), ys => ((List)ys).IsEmpty, y => p(y), ys => ys, ys => xs.Tail);
        }
        */
    }
}
