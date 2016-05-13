using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace mze9412.ScriptCompiler.Converters
{
    public sealed class CompileButtonBackgroundConverter : IMultiValueConverter
    {
        /// <summary>
        /// Single instance
        /// </summary>
        public static readonly CompileButtonBackgroundConverter Default = new CompileButtonBackgroundConverter();

        /// <summary>
        /// Private ctor
        /// </summary>
        private CompileButtonBackgroundConverter()
        {
            
        }

        public object Convert(object[] values, Type type, object parameter, CultureInfo cultureInfo)
        {
            if (values.Length == 2 && values[0] is string && (values[1] is string || values[1] == null))
            {
                var hash1 = (string)values[0];
                var hash2 = (string)values[1];

                if (string.IsNullOrEmpty(hash2))
                {
                    return Brushes.Yellow;
                }

                return hash1 == hash2 ? Brushes.Transparent : Brushes.Red;
            }

            return Brushes.Transparent;
        }

        public object[] ConvertBack(object value, Type[] types, object parameter, CultureInfo cultureInfo)
        {
            throw new NotImplementedException();
        }
    }
}
