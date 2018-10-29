using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Hungry_Panda
{
    /// <summary>
    /// Interaction logic for Loading.xaml
    /// </summary>
    public partial class ViewLoadingTemplate : UserControl
    {
        public BackgroundWorker loader;
        public ViewLoadingTemplate()
        {
//            SOTC_BindingErrorTracer.BindingErrorTraceListener.SetTrace();
            Trace.WriteLine("entering viewLoadingtemplate constructor");
            InitializeComponent();
            Trace.WriteLine("init model");
//            MainWindow.model = new Model();
            loader = new BackgroundWorker();
            loader.DoWork += Loader_DoWork;
            loader.ProgressChanged += Loader_ProgressChanged;
            loader.RunWorkerCompleted += Loader_RunWorkerCompleted;
            loader.WorkerReportsProgress = true;
            Trace.WriteLine(string.Format("loader is busy? {0}", loader.IsBusy));
            Trace.WriteLine("    leaving viewLoadingtemplate constructor");
            Uid = Guid.NewGuid().ToString();
        }

        private void Loader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // You have an exception, which you can examine through the e.Error property.
                Trace.WriteLine(e.Error.Message);
                Trace.WriteLine(e.Error.StackTrace);
            }
            else
            {
                // No exception in DoWork.
//                try
//                {
                    Trace.WriteLine(string.Format("Loader_RunWorkerCompleted"));
//                    this.Dispatcher.Invoke((Action)(() =>
//                    {
                        MainWindow.model = (Model)e.Result;
                        Switcher.Switch(MainWindow.Views.signin);
//                    }));
                    return;
//                }
//                catch (Exception ex)
//                {
//                    System.Windows.MessageBox.Show(ex.Message, "Error Encountered", MessageBoxButton.OK, MessageBoxImage.Exclamation);
//                }
            }
        }

        private void Loader_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Trace.WriteLine(string.Format("BackgroundWorder_ProgressChanged({0})",e.ProgressPercentage));
            //this.Dispatcher.Invoke((Action)(() => {
                ProgressLoading.Value += e.ProgressPercentage;
            //}));
        }

        private void Loader_DoWork(object sender, DoWorkEventArgs e)
        {
//            Model r = new Model();
            e.Result = new Model();
            ((Model)(e.Result)).LoadData();
//            MainWindow.model.LoadData();
            Trace.WriteLine(string.Format("BackgroundWorker_DoWork completed?"));
        }

        // TODO: replace with a timed even to 'fake' loading for demo
        // TODO: replace with an event tied to a real loading process for final
        public void Btn_next(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(MainWindow.Views.signin);
        }

    }
}
