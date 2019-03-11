using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Text;

namespace Adhesive {
    public class BindEndpoint {

        private object _bindInstance;

        public bool Bindable { get; }

        private Dictionary<string, Action> _bindings;

        internal BindEndpoint(INotifyPropertyChanged bindSource) {

            if (_bindInstance is INotifyPropertyChanged bindableInstance) {
                this.Bindable = true;
                
                _bindings = new Dictionary<string, Action>();

                bindableInstance.PropertyChanged += BindableInstanceOnPropertyChanged;
            }
        }

        private void BindableInstanceOnPropertyChanged(object sender, PropertyChangedEventArgs e) {
            _bindings[e.PropertyName]?.Invoke();
        }

    }
}
