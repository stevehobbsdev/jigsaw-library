using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Diggins.Jigsaw.Evaluators
{
    public class CSharpEvaluator
    {
        public static void Test()
        {
            var x = Expression.Variable(typeof(int), "x");
            var blk = Expression.Block(
                    Expression.Assign(x, Expression.Constant(42)),
                    Expression.Call(null, typeof(Console).GetMethod("WriteLine"), x),           
                    Expression.Assign(x, Expression.Constant(13)),
                    Expression.Call(null, typeof(Console).GetMethod("WriteLine"), x)        
                );
        }
    }
}
