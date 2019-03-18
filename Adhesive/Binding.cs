using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Adhesive {
    public abstract class Binding : INotifyPropertyChanged {

        public abstract bool Enabled { get; }

        public abstract void Enable();
        public abstract void Disable();        

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
