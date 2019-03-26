using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Adhesive {
    public class SyncBinding<TSyncMember> : MultiWayBinding {
        
        protected ManyToOneBinding<TSyncMember, TSyncMember> InboundBinding { get; }
        protected OneToManyBinding<TSyncMember, TSyncMember> OutboundBinding { get; }

        public override bool Enabled => this.Bindings.TrueForAll(m => m.Enabled);

        private TSyncMember _centralUpdater;
        private TSyncMember CentralUpdater {
            get => _centralUpdater;
            set {
                if (object.Equals(_centralUpdater, value)) return;

                _centralUpdater = value;
                OnPropertyChanged();
            }
        }

        public SyncBinding(IEnumerable<Expression<Func<TSyncMember>>> syncMembers) {
            InboundBinding = new ManyToOneBinding<TSyncMember, TSyncMember>(
                () => CentralUpdater,
                syncMembers
            );

            OutboundBinding = new OneToManyBinding<TSyncMember, TSyncMember>(
                syncMembers,
                () => CentralUpdater
            );
        }

        public SyncBinding(IEnumerable<Expression<Func<TSyncMember>>> syncMembers, TSyncMember initialValue) {
            _centralUpdater = initialValue;

            InboundBinding = new ManyToOneBinding<TSyncMember, TSyncMember>(
                () => CentralUpdater,
                syncMembers
            );

            OutboundBinding = new OneToManyBinding<TSyncMember, TSyncMember>(
                syncMembers,
                () => CentralUpdater,
                applyLeft: true
            );
        }

        public override void Enable() {
            this.Bindings.ForEach(binding => binding.Enable());
        }

        public override void Disable() {
            this.Bindings.ForEach(binding => binding.Disable());
        }

    }
}
