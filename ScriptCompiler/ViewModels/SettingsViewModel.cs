using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;
using mze9412.ScriptCompiler.Helpers;
using mze9412.ScriptCompiler.Properties;

namespace mze9412.ScriptCompiler.ViewModels
{
    /// <summary>
    /// Settings view model ;)
    /// </summary>
    public sealed class SettingsViewModel : INotifyPropertyChanged
    {
        private string _scriptCodeDirectory;
        private string _scriptOutputDirectory;
        private bool _copyScriptToClipboard;

        /// <summary>
        /// Ctor
        /// </summary>
        public SettingsViewModel()
        {
            ScriptCodeDirectory = Settings.Default.ScriptCodeDirectory;
            ScriptOutputDirectory = Settings.Default.ScriptOutputDirectory;
            CopyScriptToClipboard = Settings.Default.CopyScriptToClipboard;

            SelectDirectoryCommand = new DelegateCommand(SelectDirectory);
        }

        /// <summary>
        /// Root directory where code is located
        /// </summary>
        public string ScriptCodeDirectory
        {
            get { return _scriptCodeDirectory; }
            set
            {
                _scriptCodeDirectory = value;
                Settings.Default.ScriptCodeDirectory = value;
                Settings.Default.Save();
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Root directory where generated script code should be sent to
        /// </summary>
        public string ScriptOutputDirectory
        {
            get { return _scriptOutputDirectory; }
            set
            {
                _scriptOutputDirectory = value;
                Settings.Default.ScriptOutputDirectory = value;
                Settings.Default.Save();
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// If true, generated script is copied to clipboard
        /// </summary>
        public bool CopyScriptToClipboard
        {
            get { return _copyScriptToClipboard; }
            set
            {
                _copyScriptToClipboard = value;
                Settings.Default.CopyScriptToClipboard = value;
                Settings.Default.Save();
                RaisePropertyChanged();
            }
        }

        public ICommand SelectDirectoryCommand { get; set; }

        public void SelectDirectory(object argument)
        {
            bool isForOutputDirectory = (string) argument == "ScriptOutputDirectory";

            var rootFolder = isForOutputDirectory ? ScriptOutputDirectory : ScriptCodeDirectory;

            var dialog = new FolderBrowserDialog()
            {
                Description = isForOutputDirectory ? Resources.SelectFolder_OutputDirectory : Resources.SelectFolder_CodeDirectory,
                SelectedPath = rootFolder
            };
            var res = dialog.ShowDialog();
            if (res == DialogResult.OK)
            {
                if (isForOutputDirectory)
                {
                    ScriptOutputDirectory = dialog.SelectedPath;
                }
                else
                {
                    ScriptCodeDirectory = dialog.SelectedPath;
                }
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
