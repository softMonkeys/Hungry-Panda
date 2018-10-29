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
    /// Interaction logic for ViewUserIconsElement.xaml
    /// </summary>
    public partial class ViewUserIconsElement : UserControl
    {
        string ico;
        public ViewUserIconsElement(string ico)
        {
            InitializeComponent();
            this.ico = ico;
            Icon.Source = Constants.PathToSource(Constants.Paths_Images.getImagesUserIconsPath() + ico);
        }
    }
}
