using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Adhesive {
    public class OneToManyBinding<TTargetMembers, TSourceMember> : MultiWayBinding {

        public override bool Enabled => this.Bindings.TrueForAll(binding => binding.Enabled);

        public OneToManyBinding(IEnumerable<Expression<Func<TTargetMembers>>> bindTargets, Expression<Func<TSourceMember>> bindSource, Func<TSourceMember, TTargetMembers> valueConverter = null, bool applyLeft = false) {
            foreach (var bindTarget in bindTargets) {
                this.Bindings.Add(
                    new OneWayBinding<TTargetMembers, TSourceMember>(
                        bindTarget,
                        bindSource,
                        valueConverter,
                        applyLeft
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
