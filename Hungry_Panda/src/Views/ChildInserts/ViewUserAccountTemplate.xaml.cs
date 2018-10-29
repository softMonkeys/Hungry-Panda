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
    /// Interaction logic for ViewUserAccountTemplate.xaml
    /// </summary>
    public partial class ViewUserAccountTemplate : UserControl
    {
        public ViewUserAccountTemplate()
        {
             InitializeComponent();
            Uid = Guid.NewGuid().ToString();
        }
        public void SetUser(UserObj user)
        {
            this.userName.Content = user.userName;
            this.userImg.Source = Constants.PathToSource(Constants.Paths_Images.getImagesUserIconsPath() + user.userIco);
        }
        public void Btn_MainFavorite(object sender, RoutedEventArgs e)
        {
            MainWindow._viewHome.FavoriteRecipes();
            Switcher.Switch(MainWindow.Views.favorites);
        }
        public void Btn_SignOut(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(MainWindow.Views.signin);
        }
        public void Btn_MainComment(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(MainWindow.Views.commentHistory);
        }
    }
}
