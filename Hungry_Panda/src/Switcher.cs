using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Hungry_Panda
{
    class Switcher
    {
        public static MainWindow pageSwitcher;
        public static MainWindow.Views viewCurrentEnum;
        public static MainWindow.Views viewPrevEnum;
        public static void Switch(MainWindow.Views view)
        {
            if (viewCurrentEnum != view 
                && view != MainWindow.Views.createAccount 
                && view != MainWindow.Views.signin
                && view != MainWindow.Views.loading)//                && viewPrevEnum != view 

            {
                viewPrevEnum = viewCurrentEnum;
                viewCurrentEnum = view;
            }
            else
            {
                Trace.WriteLine(string.Format("not switching because view({0}) != viewCurr({1}) && view({0}) != viewPrev({2}) && view({0}) != createAccount({3})",view,viewPrevEnum,viewCurrentEnum,MainWindow.Views.createAccount));
            }
            pageSwitcher.Navigate(view);
        }

    }
}
