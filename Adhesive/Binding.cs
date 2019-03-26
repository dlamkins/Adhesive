using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Adhesive {
    public abstract class Binding : INotifyPropertyChanged {

        public abstract bool Enabled { get; }

        public abstract void Enable();
        public abstract void Disable();

        #region Binding Builders

        #region OneWayBinding

        public static OneWayBinding<TTargetMember, TSourceMember> CreateOneWayBinding<TTargetMember, TSourceMember>(Expression<Func<TTargetMember>> bindTarget, Expression<Func<TSourceMember>> bindSource, Func<TSourceMember, TTargetMember> valueConverter = null, bool applyLeft = false) {
            return new OneWayBinding<TTargetMember, TSourceMember>(bindTarget, bindSource, valueConverter, applyLeft);
        }

        #endregion

        #region TwoWayBinding

        public static TwoWayBinding<TLeftMember, TRightMember> CreateTwoWayBinding<TLeftMember, TRightMember>(Expression<Func<TLeftMember>> leftTarget, Expression<Func<TRightMember>> rightTarget, Func<TRightMember, TLeftMember> leftValueConverter = null, Func<TLeftMember, TRightMember> rightValueConverter = null, InitialBindingProcedure setupProcedure = InitialBindingProcedure.ApplyLeft) {
            return new TwoWayBinding<TLeftMember, TRightMember>(leftTarget, rightTarget, leftValueConverter, rightValueConverter, setupProcedure);
        }

        #endregion

        #region OneToManyBinding

        public static OneToManyBinding<TTargetMembers, TSourceMember> CreateOneToManyBinding<TTargetMembers, TSourceMember>(IEnumerable<Expression<Func<TTargetMembers>>> bindTargets, Expression<Func<TSourceMember>> bindSource, Func<TSourceMember, TTargetMembers> valueConverter = null, bool applyLeft = false) {
            return new OneToManyBinding<TTargetMembers, TSourceMember>(bindTargets, bindSource, valueConverter, applyLeft);
        }

        #endregion

        #region ManyToOneBinding

        public static ManyToOneBinding<TTargetMember, TSourceMembers> CreateManyToOneBinding<TTargetMember, TSourceMembers>(Expression<Func<TTargetMember>> bindTarget, IEnumerable<Expression<Func<TSourceMembers>>> bindSources, Func<TSourceMembers, TTargetMember> valueConverter = null) {
            return new ManyToOneBinding<TTargetMember, TSourceMembers>(bindTarget, bindSources, valueConverter);
        }

        #endregion

        #region SyncBinding

        public static SyncBinding<TSyncMember> CreateSyncBinding<TSyncMember>(IEnumerable<Expression<Func<TSyncMember>>> syncMembers) {
            return new SyncBinding<TSyncMember>(syncMembers);
        }

        #endregion

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
