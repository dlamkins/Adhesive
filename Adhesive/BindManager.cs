using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Adhesive {
    public class BindManager {

        private static Dictionary<object, BindEndpoint> BindEndpointCache { get; }

        static BindManager() {
            BindEndpointCache = new Dictionary<object, BindEndpoint>();
        }

        internal static BindEndpoint GetBindEndpoint(object instance) {
            if (!BindEndpointCache.ContainsKey(instance)) {
                BindEndpointCache.Add(instance, new BindEndpoint(instance));
            }

            return BindEndpointCache[instance];
        }

    }
}
