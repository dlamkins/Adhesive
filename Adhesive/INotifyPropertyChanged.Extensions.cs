using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Adhesive {
    public static class INotifyPropertyChangedExtensions {

        public static IReadOnlyCollection<Binding> GetBindings(this INotifyPropertyChanged instance) {
            List<Binding> bindings = new List<Binding>();

            //foreach (var member in BindManager.GetBindEndpoint(instance).CachedMembers.SelectMany<string, EndpointMember>(d => d.Value)) {
                
            //}

            return new List<Binding>().AsReadOnly();
        }

    }
}
