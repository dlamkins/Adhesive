using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Adhesive {
    internal static class ExpressionHelper {

        internal static object EvaluateExpression(Expression expression) {
            if (expression.NodeType == ExpressionType.Constant) {
                return ((ConstantExpression)expression).Value;
            }

            var lambda = Expression.Lambda(expression, Enumerable.Empty<ParameterExpression>());
            return lambda.Compile().DynamicInvoke();
        }

        public static Action<TTarget> MakeAssignmentAction<TTarget>(MethodInfo @set, object instance) {
            var f = (Action<TTarget>)Delegate.CreateDelegate(typeof(Action<TTarget>), instance, @set);
            return t => f(t);
        }

        public static Func<TSource> MakeGetFunc<TSource>(MethodInfo @get, object instance) {
            var f = (Func<TSource>)Delegate.CreateDelegate(typeof(Func<TSource>), instance, @get);
            return f;
        }

    }
}
