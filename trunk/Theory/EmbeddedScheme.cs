using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw
{
    public class EmbeddedScheme
    {
        public delegate dynamic Function(params object[] args);

        public static Function Lambda(DynamicFunc0 f) { return (args) => f(); }
        public static Function Lambda(DynamicFunc1 f) { return (args) => f(args[0]); }
        public static Function Lambda(DynamicFunc2 f) { return (args) => f(args[0], args[1]); }
        public static Function Lambda(DynamicFunc3 f) { return (args) => f(args[0], args[1], args[2]); }

        public static void Tests()
        {
            {
                Function f = Lambda((a, b) => a + b);
                Console.WriteLine(f(1, 2));
            }

            // The self-applicative combinator
            Function M = Lambda(f => f(f));

            {
                // Factorial written as a fixed-point. 
                Function fact = M(Lambda(f => Lambda(n => n <= 1 ? 1 : n * M(f)(n - 1))));

                for (int i = 1; i < 10; ++i)
                    Console.WriteLine("Factorial of {0} is {1}", i, fact(i));
            }

            // Question: how can I implement the "letrec" in embedded Scheme? 
            // Question: how can I implement "define".
            // Todo: Note the cheats
        }
    }
}
