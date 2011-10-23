using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
    
namespace Diggins.Jigsaw
{
    /// <summary>
    /// The SECD machine was invented by David Landin and is an abstract machine useful for evaluating 
    /// functional languages. Its basic architecture closely resembles that of modern virtual machines like 
    /// the .NET VM and the Java VM. 
    /// 
    /// This implementation uses the Lisp list structure. 
    /// </summary>
    class SECDMachine
    {
        #region registers
        List Stack;
        List Env;
        List Code;
        List Dump;
        #endregion

        #region stack operations
        public void Push(object o, ref List list) 
        { 
            list = List.Cons(o, list); 
        }

        public dynamic Nth(int n, List list)
        {
            return n == 0 
                ? list.Head 
                : Nth(n - 1, list.Tail);
        }

        public void Push(object o)
        {
            Push(o, ref Stack);
        }

        public dynamic Pop(ref List list) 
        { 
            var top = list.Head; 
            list = list.Tail; 
            return top; 
        }

        public dynamic Pop()
        {
            return Pop(ref Stack);
        }
        #endregion 

        #region primitives
        public void LoadConstant(dynamic c) 
        { 
            Push(c); 
        }
        
        public void LoadVariable(List v) 
        {
            int level = v.Head;
            int pos = v.Tail;
            List frame = Nth(level, Env);
            dynamic val = Nth(pos, frame);
            Push(val);
        }

        public void Select(List a, List b) 
        { 
            Push(Code, ref Dump); 
            Code = Pop() == List.Empty ? a : b; 
        }
        
        public void Join() 
        { 
            Code = Pop(ref Dump); 
        }

        public void LoadFunction(List f) 
        { 
            Push(List.Cons(f, Env)); 
        }
        
        public void Apply() 
        { 
            Push(Stack, ref Dump); 
            Push(Env, ref Dump); 
            Push(Code, ref Dump); 
            List closure = Pop();
            List paramlist = Pop();
            Env = closure.Tail;
            Push(paramlist, ref Env);
            Code = closure.Head;
            Stack = List.Empty;
        }

        public void Return()
        {
            var r = Pop();
            Code = Pop(ref Dump);
            Env = Pop(ref Dump);
            Stack = Pop(ref Dump);
            Push(r);
        }

        public void Dummy()
        {
            Push(List.Empty, ref Env);
        }

        public void TailApply()
        {
            List closure = Pop();
            List paramlist = Pop();
            Env = List.Cons(closure.Tail, Env.Tail);
            Push(paramlist, ref Env);
            Code = closure.Head;
            Stack = List.Empty;
        }

        public void RecApply()
        {
            Push(Stack, ref Dump);
            Push(Env, ref Dump);
            Push(Code, ref Dump);
            List closure = Pop();
            List paramlist = Pop();
            Env = List.Cons(closure.Tail, Env.Tail);
            Push(paramlist, ref Env);
            Code = closure.Head;
            Stack = List.Empty;
        }
        #endregion 
    }
}
