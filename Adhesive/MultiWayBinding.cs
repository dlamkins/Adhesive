using System;
using System.Collections.Generic;
using System.Text;

namespace Adhesive
{
    public abstract class MultiWayBinding : Binding {

        private List<OneWayBinding> _bindings;
        protected List<OneWayBinding> Bindings => _bindings ?? (_bindings = new List<OneWayBinding>());

    }
}
