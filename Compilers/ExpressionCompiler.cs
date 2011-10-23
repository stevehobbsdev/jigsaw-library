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

        VarBindings env = new VarBindings();

        public static Expression Noop
        {
            get { return Expression.Call(null, typeof(Primitives).GetMethod("noop")); }
        }

        public Expression AddBinding(ParameterExpression param)
        {
            return AddBinding(param.Name, param);
        }

        public Expression AddBinding(string name, Expression exp)
        {
            env = env.AddBinding(name, exp);
            return exp;
        }

        public Expression Lookup(String s)
        {
            return (Expression)env[s];
        }

        public Expression ScopedExpr(Func<Expression> x)
        {
            VarBindings old = env;
            var result = x();
            env = old;
            return result;
        }

        public LabelTarget GetReturnTarget()
        {
            var label = (LabelExpression)env.Find(ReturnTargetName);
            return label.Target;
        }

        public LabelExpression CreateReturnTarget()
        {
            var target = Expression.Label(typeof(Object), ReturnTargetName);
            var label = Expression.Label(target, Expression.Constant(null));
            env = env.AddBinding(ReturnTargetName, label);
            return label;
        }

        public static Expression CompileIndexExpr(Expression self, Expression index)
        {
            return Expression.Call(null, Primitives.GetMethod("index"), new Expression[] { self, index });
        }

        public static Expression CompileCallExpr(Expression func, IEnumerable<Expression> args)
        {
            return Expression.Call(null, Primitives.GetMethod("dynamic_invoke"), args);
        }

        public Expression CompileForLoop(Expression init, Expression cond, Expression step, Expression body)
        {
            throw new NotImplementedException();
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

        public LambdaExpression CreateStatementLambda(IEnumerable<ParameterExpression> paramlist, Func<Expression> bodyfunc)
        {
            return (LambdaExpression)ScopedExpr(() =>
            {
                var ps = paramlist.ToArray();
                foreach (var p in ps) AddBinding(p);
                var returnTarget = CreateReturnTarget();
                var body = Expression.Block(typeof(Object), bodyfunc(), returnTarget);
                return Expression.Lambda(body, ps);
            });
        }

        public LambdaExpression CreateExpressionLambda(IEnumerable<ParameterExpression> paramlist, Func<Expression> bodyfunc)
        {
            return (LambdaExpression)ScopedExpr(() =>
            {
                var ps = paramlist.ToArray();
                foreach (var p in ps) AddBinding(p);
                return Expression.Lambda(bodyfunc(), ps);
            });
        }
    }
}
