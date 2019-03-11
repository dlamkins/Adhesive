using System;
using System.Collections.Generic;
using System.Text;

namespace Adhesive {
    public abstract class Binding {

        public abstract bool Enabled { get; }

        public abstract void Enable();
        public abstract void Disable();

    }
}
