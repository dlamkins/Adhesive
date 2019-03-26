using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Adhesive {
    public class ManyToOneBinding<TTargetMember, TSourceMembers>:MultiWayBinding {
        public override bool Enabled => this.Bindings.TrueForAll(binding => binding.Enabled);

        public ManyToOneBinding(Expression<Func<TTargetMember>> bindTarget, IEnumerable<Expression<Func<TSourceMembers>>> bindSources,  Func<TSourceMembers, TTargetMember> valueConverter = null) {
            foreach (var bindSource in bindSources) {
                this.Bindings.Add(
                    new OneWayBinding<TTargetMember, TSourceMembers>(
                        bindTarget,
                        bindSource,
                        valueConverter,
                        false
                    )
                );
            }
        }

        public override void Disable() {
            this.Bindings.ForEach(binding => binding.Enable());
        }

        public override void Enable() {
            this.Bindings.ForEach(binding => binding.Disable());
        }
    }
}
