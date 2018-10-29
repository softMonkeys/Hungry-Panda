using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
    /// Interaction logic for ViewCreateAccountTemplate.xaml
    /// </summary>
    public partial class ViewCreateAccountTemplate : UserControl
    {
//        string userName;
//        string password;
        ImageSource ico;
        Boolean validName;
        Boolean pwdsMatch;
        Boolean icoPicked;
        double maxOffset_X = 410;
        double maxOffset_Y = 0;

        FrameworkElement ancestor;

        public ViewCreateAccountTemplate()
        {
            InitializeComponent();
            create.IsEnabled = false;
            ico = null;
            validName = false;
            pwdsMatch = true;
            icoPicked = false;
            Uid = Guid.NewGuid().ToString();
        }

        public void ConfigCreate()
        {
            profilePics.Children.Clear();
            foreach (string file in Directory.GetFiles(Constants.Paths_Images.getImagesUserIconsPath()))
            {
                Trace.WriteLine(string.Format("parsing user icon {0}", file));
                ViewUserIconsElement ico = new ViewUserIconsElement(file.Split('\\')[file.Split('\\').Length - 1]);
                profilePics.Children.Add(ico);
            }
        }


        /*this works?  wtf?
/*              ancestor = Common.GetAncestorObject(ancestor) as FrameworkElement;
                while (ancestor != null && !(ancestor is ListBox))
                {
                    object ancestor2 = Activator.CreateInstance(typeof(DependencyObject));
                    MethodInfo method = typeof(Common).GetMethod("TryFindAncestorFromPoint");
                    MethodInfo generic = method.MakeGenericMethod(ancestor.GetType());
                    generic.Invoke(null, new object[] { sender as UIElement, Mouse.GetPosition(this) });

                    Trace.WriteLine(string.Format("ancestor has type {0} and tryFindAncestorFromPoint can find? {1}", ancestor.GetType(), generic.Invoke(null, new object[] { sender as UIElement, Mouse.GetPosition(this) }) == null?false:true));
                    if (Common.GetAncestorObject(ancestor)==null) break;
                    ancestor = Common.GetAncestorObject(ancestor) as FrameworkElement;
                }
*/


        private void Control_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {//https://stackoverflow.com/questions/1495408/how-to-drag-a-usercontrol-inside-a-canvas
            maxOffset_X -= profilePics.ActualWidth;
            Trace.WriteLine(string.Format("setting create account offset to {0} because width is {1}", maxOffset_Y, profilePics.ActualWidth));
            ancestor = profilePics;
            if (!Common.TransformIsRegistered(this))
            {
                Trace.WriteLine(string.Format("registering new ancestor"));
                FrameworkElement ancestor = Common.TryFindAncestorFromPoint<StackPanel> (sender as UIElement, Mouse.GetPosition(sender as IInputElement));
                Common.registerTransformNamed<StackPanel>(new Point(maxOffset_X, maxOffset_Y), this,"MainViewPane");
            }
            Common.Control_MouseLeftButtonDown(sender, e, this);
        }

        private void Control_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {//https://stackoverflow.com/questions/1495408/how-to-drag-a-usercontrol-inside-a-canvas
            Common.Control_MouseLeftButtonUp(sender, e, this, ancestor);
            if (e.Handled) return;// work is done;
            SelectIco(sender, e);
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {//https://stackoverflow.com/questions/1495408/how-to-drag-a-usercontrol-inside-a-canvas
            Common.Control_MouseMove(sender, e, this, ancestor);
        }

        private void Control_Leave(object sender, RoutedEventArgs e)
        {//mouse out of elment, drop capture as a minimum!
            Common.Control_Leave(sender, e, this, ancestor);
        }

        private void Btn_cancel(object sender, RoutedEventArgs e)
        { 
            Switcher.Switch(MainWindow.Views.signin);
        }

        private void Btn_create(object sender, RoutedEventArgs e)
        {
            SaveAccountInfo(UserName.Text, pass1.Password, ico.ToString());
            Model.GetAccountsFromFile();
            Model.user = Model.users[UserName.Text];
            MainWindow._viewSignIn.addAccount(UserName.Text, new string[] { pass1.Password, ico.ToString() });

            MainWindow._viewCommentHistory.ConfigComments();
            MainWindow._viewRecipe.SignIn.Visibility = Visibility.Hidden;
            MainWindow._viewSignIn.DisableUser(false);

            //            Model.user = new UserObj(UserName.Text, pass1.Password, ico);//to delay image source creation until absolutely necessary
            //            string[] icoPath = ico.ToString().Split('/');
            //            Model.user = new UserObj(UserName.Text, pass1.Password, icoPath[icoPath.Length-1]);
            //            
            //            users.Add(account[0], new UserObj(account[0], account[1], account[2]));//delay source creation.
            //            Switcher.Switch(MainWindow.Views.main);//no, do not merely throw main, be as signin does
            if (Switcher.viewPrevEnum != MainWindow.Views.loading && Switcher.viewCurrentEnum != MainWindow.Views.user)
            {//if we didn't come from loading, or user, we return to where we were, else goto home-popular

                Trace.WriteLine(String.Format("switch(a) via signIn from {0} to {1}", Switcher.viewCurrentEnum, Switcher.viewPrevEnum));
                Switcher.Switch(Switcher.viewCurrentEnum);
            }
            else
            {
                Trace.WriteLine(String.Format("switch(b) via signIn from {0} to {1}", Switcher.viewCurrentEnum, MainWindow.Views.main));
                Switcher.Switch(MainWindow.Views.main);
            }
        }

        private void Btn_UserName(object sender, RoutedEventArgs e)
        {
            validName = false;
            ValidName.Visibility = (validName ? Visibility.Hidden : Visibility.Visible);
            if (UserName.Text.Length == 0)
            {
                Trace.WriteLine("invalid name,short");
                return;
            }
            foreach (string user in Model.users.Keys)
                if (Model.users[user].userName.Equals(UserName.Text))
                {
                    Trace.WriteLine("invalid name,taken");
                    return;
                }
            validName = true;
            ValidName.Visibility = (validName ? Visibility.Hidden : Visibility.Visible);
            CheckEnable();
        }

        private void Btn_UserPass(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine(String.Format("validatePassword {0} == {1} = {2}", pass1.Password, pass2.Password, pass1.Password.Equals(pass2.Password)));
            pwdsMatch = pass1.Password.Equals(pass2.Password);
            PwdNotMatch.Visibility = (pwdsMatch ? Visibility.Hidden : Visibility.Visible);
            CheckEnable();
        }

        private void CheckEnable()
        {
            if (pwdsMatch && icoPicked && validName)
                create.IsEnabled = true;
            else
                create.IsEnabled = false;
            Trace.WriteLine(String.Format("valid name? {0}, valid pwd? {1}, ico? {2}",!validName,pwdsMatch,icoPicked));
        }

        public void SaveAccountInfo(string userName, string pwd, string ico)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(Constants.Paths_Data.GetAccountsFilePath(),true);
            file.Write('\n'+userName + '\t' + pwd + '\t' + ico.Split('/')[ico.Split('/').Length-1]);
            file.Close();
        }

        private void SelectIco(object sender, MouseButtonEventArgs e)
        {
            ViewUserIconsElement element = e.Source as ViewUserIconsElement;
            if (element == null) return;
            int selected = profilePics.Children.IndexOf(element);
            for (int i = 0;i < profilePics.Children.Count; i++)
            {
                ViewUserIconsElement c = (profilePics.Children[i] as ViewUserIconsElement);
                if (i == selected)
                    c.Selected.Visibility = Visibility.Visible;
                else
                    c.Selected.Visibility = Visibility.Hidden;
            }
            ico = element.Icon.Source;
/*
            object obj = e.OriginalSource;
            if (obj.GetType().Equals(typeof(Image)))
                ico = ((Image)e.OriginalSource).Source;*/
            icoPicked = true;
            CheckEnable();
        }
    }
}
