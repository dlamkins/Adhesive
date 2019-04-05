using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Adhesive {
    internal class EndpointMember {

        public PropertyInfo Property { get; }

        private List<OneWayBinding> _targetBindings;
        public List<OneWayBinding> TargetBindings => _targetBindings ?? (_targetBindings = new List<OneWayBinding>());

        private List<OneWayBinding> _sourceBindings;
        public List<OneWayBinding> SourceBindings => _sourceBindings ?? (_sourceBindings = new List<OneWayBinding>());

        internal EndpointMember(PropertyInfo property) {
            Property = property;
        }

    }

    public class BindEndpoint {

        private WeakReference _bindReference;

        public bool Bindable { get; }

        internal Dictionary<string, EndpointMember> CachedMembers { get; }

        internal BindEndpoint(object bindSource) {
            _bindReference = new WeakReference(bindSource, false);

            if (bindSource is INotifyPropertyChanged bindableInstance) {
                this.Bindable = true;
                
                CachedMembers = new Dictionary<string, EndpointMember>();

                bindableInstance.PropertyChanged += BindableInstanceOnPropertyChanged;
            }
        }

        internal PropertyInfo GetMemberAsTarget(MemberInfo member, OneWayBinding binding) {
            EndpointMember cachedMemberInfo = GetMember(member);
            cachedMemberInfo.TargetBindings.Add(binding);

            return cachedMemberInfo.Property;
        }

        internal PropertyInfo GetMemberAsSource(MemberInfo member, OneWayBinding binding) {
            EndpointMember cachedMemberInfo = GetMember(member);
            cachedMemberInfo.SourceBindings.Add(binding);

            return cachedMemberInfo.Property;
        }

        internal void RemoveTargetBinding(MemberInfo member, OneWayBinding binding) {

        }

        internal void RemoveSourceBinding(MemberInfo member, OneWayBinding binding) {

        }

        private EndpointMember GetMember(MemberInfo member) {
            EndpointMember cachedMemberInfo = null;

            if (!CachedMembers.ContainsKey(member.Name)) {
                cachedMemberInfo = new EndpointMember(member as PropertyInfo);
                CachedMembers.Add(member.Name, cachedMemberInfo);
            }

            cachedMemberInfo = cachedMemberInfo ?? CachedMembers[member.Name];

            return cachedMemberInfo;
        }

        private void BindableInstanceOnPropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (CachedMembers.TryGetValue(e.PropertyName, out EndpointMember propertyBindings)) {
                for (int i = 0; i < propertyBindings.SourceBindings.Count; i++) {
                    propertyBindings.SourceBindings[i].Run();
                }
            }
        }

    }
}
