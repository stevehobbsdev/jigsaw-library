using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw
{
    public class PostfixCalculator
    {
        Stack<Object> stack = new Stack<object>();

        private object EvalTerm(string s)
        {
            switch (s)
            {
                case "+": return Primitives.add(stack.Pop(), stack.Pop());
                case "-": return Primitives.subtract(stack.Pop(), stack.Pop());
                case "*": return Primitives.multiply(stack.Pop(), stack.Pop());
                case "/": return Primitives.divide(stack.Pop(), stack.Pop());
                case "%": return Primitives.modulo(stack.Pop(), stack.Pop());
                default: return Double.Parse(s);  
            }
        }
        
        public void Eval(string s)
        {
            foreach (var t in s.Split(' '))
                stack.Push(EvalTerm(t));
        }
    }
}
