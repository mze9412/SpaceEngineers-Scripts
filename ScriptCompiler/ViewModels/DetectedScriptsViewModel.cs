using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using mze9412.ScriptCompiler.Domain;
using mze9412.ScriptCompiler.Helpers;

namespace mze9412.ScriptCompiler.ViewModels
{
    public sealed class DetectedScriptsViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Script> _detectedScripts;

        private readonly SettingsViewModel settingsViewModel;

        public const string InfoFileName = "ScriptInfo.txt";

        private Timer autoRefreshTimer;

        /// <summary>
        /// Ctor
        /// </summary>
        public DetectedScriptsViewModel(SettingsViewModel settingsViewModel)
        {
            this.settingsViewModel = settingsViewModel;
            DetectedScripts = new ObservableCollection<Script>();

            CompileCommand = new DelegateCommand(Compile);
            CompileAllCommand = new DelegateCommand(CompileAll);
            RefreshScriptsCommand = new DelegateCommand(RefreshScripts);

            autoRefreshTimer = new Timer();
            autoRefreshTimer.AutoReset = true;
            autoRefreshTimer.Interval = 5000;
            autoRefreshTimer.Elapsed += OnAutoRefreshTimerElapsed;
            autoRefreshTimer.Start();
        }

        /// <summary>
        /// List of detected scripts
        /// </summary>
        public ObservableCollection<Script> DetectedScripts
        {
            get { return _detectedScripts; }
            set
            {
                _detectedScripts = value;
                RaisePropertyChanged();
            }
        }

        public ICommand CompileCommand { get; set; }
        public ICommand CompileAllCommand { get; set; }
        public ICommand RefreshScriptsCommand { get; set; }

        private void Compile(object argument)
        {
            var script = argument as Script;
            if (script != null)
            {
                var result = new StringBuilder();
                ClipCopy.Includefile(result, script.ScriptFile, false, -1, true);

                if (settingsViewModel.CopyScriptToClipboard)
                {
                    Clipboard.SetText(result.ToString());
                }

                var outDir = Path.Combine(settingsViewModel.ScriptOutputDirectory, script.Name);
                if (!Directory.Exists(outDir))
                {
                    Directory.CreateDirectory(outDir);
                }
                var hash = result.ToString().GetHashCode().ToString();
                script.LastCompileHash = hash;
                script.CurrentHash = hash;
                script.LastCompile = DateTime.Now;

                File.WriteAllText(Path.Combine(outDir, script.Name + ".cs"), result.ToString());
                File.WriteAllLines(Path.Combine(outDir, InfoFileName), new [] {script.Name, script.LastCompile.ToString(), script.LastCompileHash});
            }
        }

        private void CompileAll(object argument)
        {
            foreach (var script in DetectedScripts)
            {
                Compile(script);
            }
        }

        private void RefreshScripts(object argument)
        {
            DetectedScripts.Clear();
            if (string.IsNullOrWhiteSpace(settingsViewModel.ScriptCodeDirectory))
            {
                return;
            }

            var directory = new DirectoryInfo(settingsViewModel.ScriptCodeDirectory);
            if (directory.Exists)
            {
                var dirs = directory.GetDirectories();
                foreach (var dir in dirs)
                {
                    var files = dir.GetFiles("*Program.cs");
                    foreach (var file in files)
                    {
                        //parse name
                        var scriptName = file.Name.Substring(0, file.Name.IndexOf("Program.cs"));
                        var script = new Script(scriptName, file.FullName);

                        //check for existing version
                        var outDir = new DirectoryInfo(Path.Combine(settingsViewModel.ScriptOutputDirectory, scriptName));
                        if (outDir.Exists && outDir.GetFiles().Any(x => x.Name == InfoFileName))
                        {
                            var lines = File.ReadLines(Path.Combine(outDir.FullName, InfoFileName)).ToList();
                            script.LastCompile = DateTime.Parse(lines[1]);
                            script.LastCompileHash = lines[2];
                        }
                        
                        var result = new StringBuilder();
                        ClipCopy.Includefile(result, script.ScriptFile, false, -1, true);
                        script.CurrentHash = result.ToString().GetHashCode().ToString();
                        DetectedScripts.Add(script);
                    }
                }
            }
        }

        private void OnAutoRefreshTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (settingsViewModel.AutoRefreshScripts)
            {
                UIHelper.UIInvoke(() => {RefreshScripts(null);});
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
