using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Adhesive {
    internal class EndpointMember {

        public PropertyInfo Property { get; }
        public List<OneWayBinding> Bindings { get; }

        internal EndpointMember(PropertyInfo property, OneWayBinding binding = null) {
            Property = property;

            if (binding != null)
                Bindings = new List<OneWayBinding>(new[] { binding });
            else
                Bindings = new List<OneWayBinding>();
        }
    }

    public class BindEndpoint {

        private object _bindInstance;

        public bool Bindable { get; }

        internal Dictionary<string, EndpointMember> CachedMembers { get; }

        internal BindEndpoint(object bindSource) {
            _bindInstance = bindSource;

            if (_bindInstance is INotifyPropertyChanged bindableInstance) {
                this.Bindable = true;
                
                CachedMembers = new Dictionary<string, EndpointMember>();

                bindableInstance.PropertyChanged += BindableInstanceOnPropertyChanged;
            }
        }

        internal PropertyInfo GetCachedMember(MemberInfo member) {
            EndpointMember cachedMemberInfo;

            if (!CachedMembers.ContainsKey(member.Name)) {
                cachedMemberInfo = new EndpointMember(member as PropertyInfo);
                CachedMembers.Add(member.Name, cachedMemberInfo);
            } else {
                cachedMemberInfo = CachedMembers[member.Name];
            }

            return cachedMemberInfo.Property;
        }

        internal PropertyInfo AttachBinding(MemberInfo member, OneWayBinding binding) {
            EndpointMember cachedMemberInfo;

            if (!CachedMembers.ContainsKey(member.Name)) {
                cachedMemberInfo = new EndpointMember(member as PropertyInfo, binding);
                CachedMembers.Add(member.Name, cachedMemberInfo);
            } else {
                cachedMemberInfo = CachedMembers[member.Name];
                cachedMemberInfo.Bindings.Add(binding);
            }

            return cachedMemberInfo.Property;
        }

        private void BindableInstanceOnPropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (CachedMembers.TryGetValue(e.PropertyName, out EndpointMember propertyBindings)) {
                for (int i = 0; i < propertyBindings.Bindings.Count; i++) {
                    propertyBindings.Bindings[i].Run();
                }
            }
        }

    }
}
