using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Adhesive {
    public class OneWayBinding<TTargetMember, TSourceMember> : OneWayBinding {

        private MemberExpression _targetReference;
        private MemberExpression _sourceReference;

        private object _targetInstance;
        private INotifyPropertyChanged _sourceInstance;

        private PropertyInfo _targetProperty;
        private PropertyInfo _sourceProperty;
        
        private bool _enabled;
        public override bool Enabled => _enabled;

        private MultiWayBinding _parentBinding;
        public MultiWayBinding ParentBinding => _parentBinding;

        public bool IsMemberOfMultiWayBinding => _parentBinding != null;

        internal OneWayBinding(MultiWayBinding parentBinding, Expression<Func<TTargetMember>> bindTarget, Expression<Func<TSourceMember>> bindSource, Func<TSourceMember, TTargetMember> valueConverter, bool applyLeft) {
            _parentBinding = parentBinding;

            BuildBinding(bindTarget, bindSource, valueConverter, applyLeft);
        }

        /// <summary>
        /// Create a binding where everytime <paramref name="bindSource"/> changes, <paramref name="bindTarget"/> is updated to reflect the new value.
        /// </summary>
        /// <param name="bindTarget">The member that will be set whenever <paramref name="bindSource"/> is changed.</param>
        /// <param name="bindSource">The monitored member that, when changed, will set the value of the <paramref name="bindTarget"/>.</param>
        /// <param name="valueConverter">Allows you to manipulate, format, or cast the <paramref name="bindSource"/> before it is applied to <paramref name="bindTarget"/>.</param>
        /// <param name="applyLeft">If true, the <paramref name="bindTarget"/> is set to the value of the <paramref name="bindSource"/> as soon as the binding is made.</param>
        public OneWayBinding(Expression<Func<TTargetMember>> bindTarget, Expression<Func<TSourceMember>> bindSource, Func<TSourceMember, TTargetMember> valueConverter = null, bool applyLeft = false) {
            BuildBinding(bindTarget, bindSource, valueConverter, applyLeft);
        }

        internal void DisconnectParentBinding() {
            _parentBinding = null;
        }

        private void BuildBinding(Expression<Func<TTargetMember>> bindTarget, Expression<Func<TSourceMember>> bindSource, Func<TSourceMember, TTargetMember> valueConverter, bool applyLeft) {
            _targetReference = bindTarget.Body as MemberExpression;
            _sourceReference = bindSource.Body as MemberExpression;

            if (_targetReference == null)
                throw new ArgumentException($"{nameof(bindTarget)} is expected to contain a MemberExpression, instead it got {bindTarget.Body.NodeType}.");
            if (_sourceReference == null)
                throw new ArgumentException($"{nameof(bindSource)} is expected to contain a MemberExpression, instead it got {bindSource.Body.NodeType}.");

            _targetInstance = ExpressionHelper.EvaluateExpression(_targetReference.Expression);
            _sourceInstance = ExpressionHelper.EvaluateExpression(_sourceReference.Expression) as INotifyPropertyChanged;

            if (bindTarget == null)
                throw new ArgumentException($"{nameof(bindTarget)} must be the member of an instance.");
            if (_sourceInstance == null)
                throw new ArgumentException($"{nameof(bindSource)} must be the member of an instance that implements {nameof(INotifyPropertyChanged)}.");
            
            _targetProperty = BindManager.GetBindEndpoint(_targetInstance).GetMemberAsTarget(_targetReference.Member, this);
            _sourceProperty = BindManager.GetBindEndpoint(_sourceInstance).GetMemberAsSource(_sourceReference.Member, this);

            if (valueConverter == null) {
                // Default just directly applies the value of bindSource to bindTarget
                ApplyToTargetCall = Expression.Lambda<Action>(Expression.Assign(_targetReference, _sourceReference)).Compile();
            } else {
                // Converter was provided so we package it into an Action to call later
                Action<TTargetMember> setTargetAction = ExpressionHelper.MakeAssignmentAction<TTargetMember>(_targetProperty.SetMethod, _targetInstance);
                Func<TSourceMember> getSourceFunction = ExpressionHelper.MakeGetFunc<TSourceMember>(_sourceProperty.GetMethod, _sourceInstance);

                ApplyToTargetCall = () => setTargetAction.Invoke(valueConverter(getSourceFunction()));
            }

            _sourceInstance.PropertyChanged += (sender, e) => {
                if (_sourceProperty.Name == e.PropertyName && this.Enabled) {
                    Run();
                }
            };

            _enabled = true;

            if (applyLeft) {
                Run();
            }
        }

        public override void Run(bool force = false) {
            if (this.Enabled || force)
                ApplyToTargetCall?.Invoke();
        }

        public override void Enable() {
            _enabled = true;
        }

        public override void Disable() {
            _enabled = false;
        }

        protected override void OnBroken() {
            if (IsMemberOfMultiWayBinding) {
                // TODO: Determine what should happen to parent bindings of now broken OneWayBindings
            }

            // Clear object references ASAP
            _targetReference = null;
            _sourceReference = null;

            _targetInstance = null;
            _sourceInstance = null;

            _targetProperty = null;
            _sourceProperty = null;

            ApplyToTargetCall = null;
        }

    }

    public abstract class OneWayBinding : Binding {
        
        protected Action ApplyToTargetCall { get; set; }

        public abstract void Run(bool force = false);

    }

}
