using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for ViewShareTemplate.xaml
    /// </summary>
    public partial class ViewShareTemplate : UserControl
    {
        public ViewShareTemplate()
        {
            InitializeComponent();
            Uid = Guid.NewGuid().ToString();
        }
        public void Btn_back(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(Switcher.viewPrevEnum);
        }

        private void Btn_Social(object sender, MouseButtonEventArgs e)
        {
            Image img = sender as Image;
            System.Diagnostics.Process.Start("www."+img.Tag.ToString()+".com");
        }
    }
}
