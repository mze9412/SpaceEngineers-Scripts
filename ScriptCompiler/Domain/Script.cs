using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace mze9412.ScriptCompiler.Domain
{
    /// <summary>
    /// Represents one detected script
    /// </summary>
    public sealed class Script : INotifyPropertyChanged
    {
        private string _name;
        private string _scriptFile;
        private DateTime _lastCompile;
        private string _lastCompileHash;
        private string _currentHash;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="scriptFile"></param>
        public Script(string name, string scriptFile)
        {
            Name = name;
            ScriptFile = scriptFile;
        }

        /// <summary>
        /// Name of the script
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Filepath of the script
        /// </summary>
        public string ScriptFile
        {
            get { return _scriptFile; }
            set
            {
                _scriptFile = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Last compile time
        /// </summary>
        public DateTime LastCompile
        {
            get { return _lastCompile; }
            set
            {
                _lastCompile = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Last compile hash
        /// </summary>
        public string LastCompileHash
        {
            get { return _lastCompileHash; }
            set
            {
                _lastCompileHash = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Current hash
        /// </summary>
        public string CurrentHash
        {
            get { return _currentHash; }
            set
            {
                _currentHash = value;
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
