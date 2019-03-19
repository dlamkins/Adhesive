﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Adhesive.Sample.NotifyingControls;

namespace Adhesive.Sample {

    public class Employee : INotifyPropertyChanged {

        private string _firstName;
        private string _lastName;

        public string FirstName {
            get => _firstName;
            set {
                if (_firstName == value) return;

                _firstName = value;
                OnPropertyChanged();
            }
        }

        public string LastName {
            get => _lastName;
            set {
                if (_lastName == value) return;

                _lastName = value;
                OnPropertyChanged();
            }
        }

        public Employee(string firstName, string lastName) {
            this.FirstName = firstName;
            this.LastName = lastName;
        }
        
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }

    public class Nameplate {

        private string _inscribedName;

        public string InscribedName {
            get => _inscribedName;
            set {
                if (_inscribedName == value) return;

                _inscribedName = value;
                OnPropertyChanged();
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }

    public struct Fullname {
        public string FirstName { get; }
        public string LastName { get; }

        public Fullname(string firstName, string lastName) {
            this.FirstName = firstName;
            this.LastName = lastName;
        }
    }

    public partial class SampleWindow:Form {
        public SampleWindow() {
            InitializeComponent();
        }

        private List<Fullname> _names = new List<Fullname>();

        //private List<BasicBinding> _adhesiveBindings = new List<BasicBinding>();

        private int _sampleCount = 6;

        private void SetupAdhesive() {
            //var firstNameBinding = new Adhesive.BasicBinding<string, string>(
            //                                                                 () => sampleAdhesive.displayNameTextBox.Text,
            //                                                                 () => sampleAdhesive.firstNameTextBox.Text
            //                                                                 //(tbFirstName) => $"{sampleAdhesive.lastNameTextBox.Text}, {sampleAdhesive.firstNameTextBox.Text}",
            //                                                                 //(tbDisplayName) => sampleAdhesive.firstNameTextBox.Text
            //                                                                );

            //var lastNameBinding = new Adhesive.BasicBinding<string, string>(
            //                                                                () => sampleAdhesive.displayNameTextBox.Text,
            //                                                                () => sampleAdhesive.lastNameTextBox.Text
            //                                                               (tbLastName) => $"{sampleAdhesive.lastNameTextBox.Text}, {sampleAdhesive.firstNameTextBox.Text}",
            //                                                               //(tbDisplayName) => sampleAdhesive.lastNameTextBox.Text
            //                                                               );
            //var updateDisplayname = new Func<object, string>((tbNamePart) => $"{sampleAdhesive.lastNameTextBox.Text}, {sampleAdhesive.firstNameTextBox.Text}");
            //var firstNameBinding = new OneWayBinding<string, string>(
            //                                                              () => sampleAdhesive.displayNameTextBox.Text,
            //                                                              () => sampleAdhesive.firstNameTextBox.Text,
            //                                                              updateDisplayname
            //                                                             );
            //var lastNameBinding = new OneWayBinding<string, string>(
            //                                                              () => sampleAdhesive.displayNameTextBox.Text,
            //                                                              () => sampleAdhesive.lastNameTextBox.Text,
            //                                                              updateDisplayname
            //                                                             );

            var henry = new Employee("Henry", "Johnson");
            var henrysNameplate = new Nameplate();

            var nameplateBinding = new Adhesive.OneWayBinding<string, string>(() => henrysNameplate.InscribedName,() => henry.FirstName);
            var nameplateBinding = new Adhesive.TwoWayBinding<string, string>(
                                                                              () => henry.FirstName, 
                                                                              () => henrysNameplate.InscribedName, 
                                                                              o => o.ToString().ToUpper(), 
                                                                              o => o.ToString().ToLower(), 
                                                                              InitialBindingProcedure.ApplyRight
                                                                              );

            var nameplateBinding = new Adhesive.ManyToOneBinding<string, string>(
                                                                                 () => henrysNameplate.InscribedName,
                                                                                 new List<Expression<Func<string>>>() {
                                                                                     () => henry.FirstName,
                                                                                     () => henry.LastName
                                                                                 },
                                                                                 o => $"{henry.LastName}, {henry.FirstName}"
                                                                                );

            var nameplateBinding = new Adhesive.OneToManyBinding<string, string>(
                                                                                 new List<Expression<Func<string>>>() {
                                                                                     () => henry.LastName,
                                                                                     () => henrysNameplate.InscribedName
                                                                                 },
                                                                                 () => henry.FirstName,
                                                                                 o => o.ToString().Reverse().ToString()
                                                                                );

            List<Expression<Func<string>>> exampleTargets = new List<Expression<Func<string>>>();

            exampleTargets.Add(() => notifyingTextBox1.Text);
            exampleTargets.Add(() => notifyingTextBox2.Text);
            exampleTargets.Add(() => sampleAdhesive.displayNameTextBox.Text);
            exampleTargets.Add(() => sampleAdhesive.firstNameTextBox.Text);
            exampleTargets.Add(() => sampleAdhesive.lastNameTextBox.Text);

            var extraBinding = new SyncBinding<string>(exampleTargets, "TEST - INITIAL");

            //_adhesiveBindings.Add(firstNameBinding);
            //_adhesiveBindings.Add(lastNameBinding);
        }

        private void SetupWinBindings() {
            var firstNameBinding = sampleWinForms.displayNameTextBox.DataBindings.Add(
                                                                                         "Text",
                                                                                         sampleWinForms,
                                                                                         "CombinedBindMountPoint",
                                                                                         true,
                                                                                         DataSourceUpdateMode.OnPropertyChanged
                                                                                         );
            firstNameBinding.Format += delegate(object sender, ConvertEventArgs args) { args.Value = $"{sampleWinForms.lastNameTextBox.Text}, {sampleWinForms.firstNameTextBox.Text}"; };
        }

        private void SetupPraeclarum() {
            //Libraries.Praeclarum.Bind.Binding.Create(
                //() => samplePraeclarum.displayNameTextBox.Text == samplePraeclarum.lastNameTextBox.Text + ", " + samplePraeclarum.firstNameTextBox.Text &&
                //      notifyingTextBox1.Text == samplePraeclarum.displayNameTextBox.Text &&
                //      notifyingTextBox2.Text == samplePraeclarum.displayNameTextBox.Text);
        }

        private void SetupOnUpdate() {
            var updateDisplayName = new EventHandler((sender, e) => { sampleOnEvent.displayNameTextBox.Text = $"{sampleOnEvent.lastNameTextBox.Text}, {sampleOnEvent.firstNameTextBox.Text}"; });

            sampleOnEvent.firstNameTextBox.TextChanged += updateDisplayName;
            sampleOnEvent.lastNameTextBox.TextChanged += updateDisplayName;
        }

        private void SampleWindow_Shown(object sender, EventArgs e) {
            SetupAdhesive();
            SetupWinBindings();
            SetupPraeclarum();
            SetupOnUpdate();

            Task.Run(() => LoadSampleData());
        }

        private void LoadSampleData() {
            string[] rawNames = File.ReadAllLines("Names.txt");

            foreach (string rawName in rawNames) {
                string firstName = rawName.Split(' ')[0];
                string lastName = rawName.Split(' ')[1];

                _names.Add(new Fullname(firstName, lastName));
            }

            bttnAdhesiveStart.BeginInvoke(new Action(() => { bttnAdhesiveStart.Enabled = true; }));
            bttnWinFormsStart.BeginInvoke(new Action(() => { bttnWinFormsStart.Enabled = true; }));
            bttnPraeclarumBindStart.BeginInvoke(new Action(() => { bttnPraeclarumBindStart.Enabled = true; }));
            bttnUpdateOnEventStart.BeginInvoke(new Action(() => { bttnUpdateOnEventStart.Enabled = true; }));

            lblLoadSampleData.BeginInvoke(new Action(() => { lblLoadSampleData.Text = "Ready"; }));
        }

        private void bttnPraeclarumBindStart_Click(object sender, EventArgs e) {
            bttnPraeclarumBindStart.Enabled = false;

            long totalRuntime = 0;

            for (int i = 0; i < _sampleCount; i++) {
                lblPraeclarumResult.Text = $"Samples Completed{Environment.NewLine}{i}/{_sampleCount}";
                totalRuntime += DoBenchmark(samplePraeclarum);
            }

            lblPraeclarumResult.Text        = $"Average of{Environment.NewLine}{totalRuntime / _sampleCount}ms{Environment.NewLine}over {_sampleCount} samples (x{_names.Count} entries)";
            bttnPraeclarumBindStart.Enabled = true;
        }

        private void bttnAdhesiveStart_Click(object sender, EventArgs e) {
            bttnAdhesiveStart.Enabled = false;

            long totalRuntime = 0;

            for (int i = 0; i < _sampleCount; i++) {
                lblAdhesiveResult.Text =  $"Samples Completed{Environment.NewLine}{i}/{_sampleCount}";
                totalRuntime             += DoBenchmark(sampleAdhesive);
            }

            lblAdhesiveResult.Text = $"Average of{Environment.NewLine}{totalRuntime / _sampleCount}ms{Environment.NewLine}over {_sampleCount} samples (x{_names.Count} entries)";
            bttnAdhesiveStart.Enabled = true;
        }

        private void bttnWinFormsStart_Click(object sender, EventArgs e) {
            bttnWinFormsStart.Enabled = false;

            long totalRuntime = 0;

            for (int i = 0; i < _sampleCount; i++) {
                lblWinFormsResults.Text =  $"Samples Completed{Environment.NewLine}{i}/{_sampleCount}";
                totalRuntime           += DoBenchmark(sampleWinForms);
            }

            lblWinFormsResults.Text = $"Average of{Environment.NewLine}{totalRuntime / _sampleCount}ms{Environment.NewLine}over {_sampleCount} samples (x{_names.Count} entries)";
            bttnWinFormsStart.Enabled = true;
        }

        private void bttnUpdateOnEventStart_Click(object sender, EventArgs e) {
            bttnUpdateOnEventStart.Enabled = false;

            long totalRuntime = 0;

            for (int i = 0; i < _sampleCount; i++) {
                lblUpdateOnEventResults.Text =  $"Samples Completed{Environment.NewLine}{i}/{_sampleCount}";
                totalRuntime            += DoBenchmark(sampleOnEvent);
            }

            lblUpdateOnEventResults.Text = $"Average of{Environment.NewLine}{totalRuntime / _sampleCount}ms{Environment.NewLine}over {_sampleCount} samples (x{_names.Count} entries)";
            bttnUpdateOnEventStart.Enabled = true;
        }

        private void TypeXIntoY(string x, NotifyingTextBox y) {
            char[] nameChars = x.ToCharArray();

            foreach (char c in nameChars) {
                y.Text += c;

                Application.DoEvents();
            }
        }

        private long DoBenchmark(SampleControls.BindBenchmark.StandardControlGroupings grouping) {
            var timer = new Stopwatch();
            timer.Start();

            foreach (var fullname in _names) {
                // Clear textboxes
                grouping.firstNameTextBox.Clear();
                grouping.lastNameTextBox.Clear();

                // Text first and last name in
                TypeXIntoY(fullname.FirstName, grouping.firstNameTextBox);
                TypeXIntoY(fullname.LastName, grouping.lastNameTextBox);
            }

            timer.Stop();
            return timer.ElapsedMilliseconds;
        }
        
    }
}
