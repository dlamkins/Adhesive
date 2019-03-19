using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Adhesive {
    public static class INotifyPropertyChangedExtensions {

        /// <summary>
        /// Gets a list of all bindings where a member of this instance is the source of the bind.
        /// </summary>
        public static IReadOnlyCollection<Binding> GetSourcedBindings(this INotifyPropertyChanged instance) {
            return BindManager.GetBindEndpoint(instance).CachedMembers.SelectMany(d => d.Value.SourceBindings).ToList<Binding>().AsReadOnly();
        }

        /// <summary>
        /// Gets a list of all bindings where a member of this instance is the target of the bind.
        /// </summary>
        public static IReadOnlyCollection<Binding> GetTargetedBindings(this INotifyPropertyChanged instance) {
            return BindManager.GetBindEndpoint(instance).CachedMembers.SelectMany(d => d.Value.TargetBindings).ToList<Binding>().AsReadOnly();
        }

        /// <summary>
        /// Gets a list of all bindings where a member of this instance is either the source or target of the bind.
        /// </summary>
        public static IReadOnlyCollection<Binding> GetAllBindings(this INotifyPropertyChanged instance) {
            // TODO: Revise for performance
            return GetSourcedBindings(instance).Union(GetTargetedBindings(instance)).ToList().AsReadOnly();
        }

    }
}
