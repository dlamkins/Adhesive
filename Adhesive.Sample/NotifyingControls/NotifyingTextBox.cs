using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Adhesive.Sample.NotifyingControls {
    public class NotifyingTextBox:System.Windows.Forms.TextBox, INotifyPropertyChanged {

        protected override void OnTextChanged(EventArgs e) {
            base.OnTextChanged(e);

            OnPropertyChanged(nameof(this.Text));
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
