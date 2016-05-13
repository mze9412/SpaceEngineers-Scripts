using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using mze9412.ScriptCompiler.Domain;

namespace mze9412.ScriptCompiler.Views
{
    /// <summary>
    /// Interaction logic for DetectedScriptControl.xaml
    /// </summary>
    public partial class DetectedScriptControl : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty ScriptProperty = DependencyProperty.Register("Script", typeof(Script), typeof(DetectedScriptControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, OnDependencyPropertyChanged));

        public static readonly DependencyProperty CompileCommandProperty = DependencyProperty.Register("CompileCommand", typeof(ICommand), typeof(DetectedScriptControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, OnDependencyPropertyChanged));


        public DetectedScriptControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Triggered when a dependency property is changed
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="e"></param>
        private static void OnDependencyPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var control = dependencyObject as DetectedScriptControl;
            control?.RaisePropertyChanged(e.Property.Name);
        }

        public Script Script
        {
            get { return (Script)GetValue(ScriptProperty); }
            set { SetValue(ScriptProperty, value);}
        }

        public ICommand CompileCommand
        {
            get { return (ICommand)GetValue(CompileCommandProperty); }
            set {  SetValue(CompileCommandProperty, value);}
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
