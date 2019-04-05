using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Adhesive.Sample.NotifyingControls {
    public class TestObject:IBindable {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _value = "";
        public string Value {
            get => _value;
            set {
                if (_value == value) return;

                _value = value;

                OnPropertyChanged();
            }
        }

        #region INotifyPropertyChanged

        private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public void Dispose() {
            
        }
    }
}
