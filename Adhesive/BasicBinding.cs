using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Adhesive {
    public class BasicBinding<TLeft, TRight> : BasicBinding {

        private TAnchorType DefaultConverter<TAnchorType>(object anchor) {
            if (anchor.Equals(_leftAnchorInstance)) {
                return (TAnchorType)_leftAnchorProperty.GetValue(anchor);
            } else {
                return (TAnchorType)_rightAnchorProperty.GetValue(anchor);
            }
        }

        private MemberExpression _leftAnchor;
        private MemberExpression _rightAnchor;

        private INotifyPropertyChanged _leftAnchorInstance;
        private INotifyPropertyChanged _rightAnchorInstance;

        private PropertyInfo _leftAnchorProperty;
        private PropertyInfo _rightAnchorProperty;

        private Func<TRight, TLeft> getLeftAnchor;
        private Func<TLeft, TRight> getRightAnchor;

        public BasicBinding(Expression<Func<TLeft>> leftProperty, Expression<Func<TRight>> rightProperty) {
            PrepareBinding(leftProperty, rightProperty, DefaultConverter<TLeft>, DefaultConverter<TRight>);
        }

        public BasicBinding(Expression<Func<TLeft>> leftProperty, Expression<Func<TRight>> rightProperty, Func<object, TLeft> leftConverter, Func<object, TRight> rightConverter) {
            PrepareBinding(leftProperty, rightProperty, leftConverter, rightConverter);
        }

        private void PrepareBinding(Expression<Func<TLeft>> leftProperty, Expression<Func<TRight>> rightProperty, Func<object, TLeft> leftConverter, Func<object, TRight> rightConverter) {
            _leftAnchor  = leftProperty.Body as MemberExpression;
            _rightAnchor = rightProperty.Body as MemberExpression;

            if (_leftAnchor == null)
                throw new ArgumentException($"{nameof(leftProperty)} is expected to contain a MemberExpression, instead it got {leftProperty.Body.NodeType}.");
            if (_rightAnchor == null)
                throw new ArgumentException($"{nameof(rightProperty)} is expected to contain a MemberExpression, instead it got {leftProperty.Body.NodeType}.");

            _leftAnchorInstance  = EvaluateExpression(_leftAnchor.Expression) as INotifyPropertyChanged;
            _rightAnchorInstance = EvaluateExpression(_rightAnchor.Expression) as INotifyPropertyChanged;

            if (_leftAnchorInstance == null)
                throw new ArgumentException($"{nameof(leftProperty)} is expected to be a property in a class that implements {nameof(INotifyPropertyChanged)}.");
            if (_rightAnchorInstance == null)
                throw new ArgumentException($"{nameof(rightProperty)} is expected to be a property in a class that implements {nameof(INotifyPropertyChanged)}.");

            var syncToLeft  = Expression.Lambda<Action>(Expression.Assign(_leftAnchor, _rightAnchor)).Compile();
            var syncToRight = Expression.Lambda<Action>(Expression.Assign(_rightAnchor, _leftAnchor)).Compile();

            _leftAnchorProperty  = _leftAnchor.Member as PropertyInfo;
            _rightAnchorProperty = _rightAnchor.Member as PropertyInfo;

            var setLeftAnchor = MakeLeftAssignmentDelegate(_leftAnchorProperty.SetMethod, _leftAnchorInstance);
            var setRightAnchor = MakeRightAssignmentDelegate(_rightAnchorProperty.SetMethod, _rightAnchorInstance);

            //getLeftAnchor = MakeLeftGetDelegate<TLeft>(_leftAnchorProperty.GetMethod, _leftAnchorInstance);
            //getRightAnchor = MakeRightGetDelegate<TRight>(_rightAnchorProperty.GetMethod, _rightAnchorInstance);

            //_leftAnchorProperty.SetValue(_leftAnchorInstance, leftConverter(_rightAnchorInstance));
            if (_leftAnchor != null)
                setLeftAnchor.Invoke(leftConverter(_rightAnchorInstance));
            else
                syncToLeft();

            _leftAnchorInstance.PropertyChanged += (sender, e) => {
                if (_leftAnchorProperty.Name == e.PropertyName) {
                    if (_leftAnchor != null)
                        setRightAnchor.Invoke(rightConverter(_leftAnchorInstance));
                    else
                        syncToRight();
                }
            };

            _rightAnchorInstance.PropertyChanged += (sender, e) => {
                if (_rightAnchorProperty.Name == e.PropertyName) {
                    if (_leftAnchor != null)
                        setLeftAnchor.Invoke(leftConverter(_rightAnchorInstance));
                    else
                        syncToLeft();
                }
            };
        }

        public Action<TLeft> MakeLeftAssignmentDelegate(MethodInfo @set, object instance) {
            var f = (Action<TLeft>)Delegate.CreateDelegate(typeof(Action<TLeft>), instance, @set);
            return t => f(t);
        }

        public Action<TRight> MakeRightAssignmentDelegate(MethodInfo @set, object instance) {
            var f = (Action<TRight>)Delegate.CreateDelegate(typeof(Action<TRight>), instance, @set);
            return t => f(t);
        }

        //public Func<U> MakeLeftGetDelegate<U>(MethodInfo @get, object instance) {
        //    var f = (Func<U>)Delegate.CreateDelegate(typeof(Func<U>), instance, @get);
        //    return t => f();
        //}

        //public Func<U> MakeRightGetDelegate<U>(MethodInfo @get, object instance) {
        //    var f = (Func<U>)Delegate.CreateDelegate(typeof(Func<U>), instance, @get);
        //    return t => f();
        //}

        private static object EvaluateExpression(Expression expression) {
            if (expression.NodeType == ExpressionType.Constant) {
                return ((ConstantExpression)expression).Value;
            }

            var lambda = Expression.Lambda(expression, Enumerable.Empty<ParameterExpression>());
            return lambda.Compile().DynamicInvoke();
        }

    }

    public class BasicBinding { }

}
