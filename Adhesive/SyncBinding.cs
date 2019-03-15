using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Adhesive {
    public class SyncBinding<TSyncMember> : MultiWayBinding {

        public override bool Enabled => this.Bindings.TrueForAll(m => m.Enabled);

        public SyncBinding(IEnumerable<Expression<Func<TSyncMember>>> syncMembers, TSyncMember initialValue) {

        }

        public override void Enable() {
            this.Bindings.ForEach(binding => binding.Enable());
        }

        public override void Disable() {
            this.Bindings.ForEach(binding => binding.Disable());
        }

    }
}
