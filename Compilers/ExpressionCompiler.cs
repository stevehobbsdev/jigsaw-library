using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Diggins.Jigsaw
{
    public class ExpressionCompiler
    {
        private readonly string ReturnTargetName = "$return";

        public Context Context = new Context();

        public static Expression Noop
        {
            get { return Expression.Call(null, typeof(Primitives).GetMethod("noop")); }
        }

        public Expression AddToContext(ParameterExpression param)
        {
            Context = Context.AddContext(param.Name, param);
            return param;
        }

        public Expression Lookup(String s)
        {
            return (Expression)Context[s];
        }

        public Expression ScopedExpr(Func<Expression> x)
        {
            Context old = Context;
            var result = x();
            Context = old;
            return result;
        }

        public LabelTarget GetReturnTarget()
        {
            return (LabelTarget)Context.Find(ReturnTargetName);
        }

        public LabelTarget CreateReturnTarget()
        {
            var target = Expression.Label();
            Context = Context.AddContext(ReturnTargetName, target);
            return target;
        }

        public Expression CreateBinaryExpression(string op, Expression left, Expression right)
        {
            switch (op)
            {
                case "+":
                    return Expression.Add(left, right);
                case "-":
                    return Expression.Subtract(left, right);
                case "*":
                    return Expression.Multiply(left, right);
                case "/":
                    return Expression.Divide(left, right);
                case "%":
                    return Expression.Modulo(left, right);
                case ">>":
                    return Expression.RightShift(left, right);
                case "<<":
                    return Expression.LeftShift(left, right);
                case ">":
                    return Expression.GreaterThan(left, right);
                case ">=":
                    return Expression.GreaterThanOrEqual(left, right);
                case "<":
                    return Expression.LessThan(left, right);
                case "<=":
                    return Expression.LessThanOrEqual(left, right);
                case "==":
                    return Expression.Equal(left, right);
                case "!=":
                    return Expression.NotEqual(left, right);
                case "??":
                    return Expression.Coalesce(left, right);
                case "=":
                    return Expression.Assign(left, right);
                case "+=":
                    return Expression.AddAssign(left, right);
                case "-=":
                    return Expression.SubtractAssign(left, right);
                case "*=":
                    return Expression.MultiplyAssign(left, right);
                case "/=":
                    return Expression.DivideAssign(left, right);
                case "%=":
                    return Expression.ModuloAssign(left, right);
                case ">>=":
                    return Expression.RightShiftAssign(left, right);
                case "<<=":
                    return Expression.LeftShiftAssign(left, right);
                default:
                    return null;
            }
        }

        public LambdaExpression CreateStatementLambda(ParameterExpression[] ps, Func<Expression> bodyfunc)
        {
            return (LambdaExpression)ScopedExpr(() =>
            {                
                foreach (var p in ps) AddToContext((ParameterExpression)p);
                var returnTarget = CreateReturnTarget();
                var body = Expression.Block(ps, bodyfunc(), Expression.Label(returnTarget));
                return Expression.Lambda(body, ps);
            });
        }

        public LambdaExpression CreateExpressionLambda(ParameterExpression[] ps, Func<Expression> bodyfunc)
        {
            return (LambdaExpression)ScopedExpr(() =>
            {
                foreach (var p in ps) AddToContext(p);
                return Expression.Lambda(bodyfunc(), ps);
            });
        }
    }
}
