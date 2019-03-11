using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Adhesive.Sample.SampleControls.BindBenchmark {
    public partial class StandardControlGroupings:UserControl, INotifyPropertyChanged {

        public NotifyingControls.NotifyingTextBox firstNameTextBox => tbFirstName;
        public NotifyingControls.NotifyingTextBox lastNameTextBox => tbLastName;
        public NotifyingControls.NotifyingTextBox displayNameTextBox => notifyingTextBox1;

        public string CombinedBindMountPoint => "";



        public StandardControlGroupings() {
            InitializeComponent();

            this.firstNameTextBox.PropertyChanged += delegate { OnPropertyChanged(nameof(this.CombinedBindMountPoint)); };
            this.lastNameTextBox.PropertyChanged += delegate { OnPropertyChanged(nameof(this.CombinedBindMountPoint)); };
        }


        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
