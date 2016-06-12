using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace mze9412.ScriptCompiler.Helpers
{
    public static class UIHelper
    {
        public static void UIInvoke(Action action)
        {
            Application.Current.Dispatcher.Invoke(action);
        }
    }
}
