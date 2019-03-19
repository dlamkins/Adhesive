using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Adhesive {
    public class BindTarget<TTargetMember> {

        private object _targetInstance;

        internal BindTarget(Expression<Func<TTargetMember>> bindTargetExpression) {
            var targetReference = bindTargetExpression.Body as MemberExpression;

            if (targetReference == null)
                throw new ArgumentException($"{nameof(bindTargetExpression)} is expected to contain a MemberExpression, instead it got {bindTargetExpression.Body.NodeType}.");



        }

    }
}
