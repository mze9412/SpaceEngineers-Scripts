using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using mze9412.ScriptCompiler.ViewModels;

namespace mze9412.ScriptCompiler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private DetectedScriptsViewModel _detectedScriptsViewModel;
        private SettingsViewModel _settingsViewModel;

        /// <summary>
        /// Ctor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            SettingsViewModel = new SettingsViewModel();
            DetectedScriptsViewModel = new DetectedScriptsViewModel(SettingsViewModel);
        }

        /// <summary>
        /// Settings view model
        /// </summary>
        public SettingsViewModel SettingsViewModel
        {
            get { return _settingsViewModel; }
            set
            {
                _settingsViewModel = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// View model for detected scripts view
        /// </summary>
        public DetectedScriptsViewModel DetectedScriptsViewModel
        {
            get { return _detectedScriptsViewModel; }
            set
            {
                _detectedScriptsViewModel = value;
                RaisePropertyChanged();
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
