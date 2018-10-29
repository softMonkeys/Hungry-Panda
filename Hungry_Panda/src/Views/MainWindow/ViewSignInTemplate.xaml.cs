using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for ViewSignInTemplate.xaml
    /// </summary>
    public partial class ViewSignInTemplate : UserControl
    {
        private Dictionary<String,String[]> accounts;
        public ViewSignInTemplate()
        {
            Trace.WriteLine("entering viewSignInTemplate constructor");
            InitializeComponent();
            accounts = new Dictionary<String,String[]>();
            getAccountsFromFile();
            Trace.WriteLine("leaving viewSignInTemplate constructor");
            Uid = Guid.NewGuid().ToString();
        }

        public void addAccount(string name, string[] acc)
        {
            accounts.Add(name, acc);
        }

        public void getAccountsFromFile(){
            Trace.WriteLine("opening accounts file at " + Constants.Paths_Data.GetAccountsFilePath());
            System.IO.StreamReader file = new System.IO.StreamReader(Constants.Paths_Data.GetAccountsFilePath());
            string line;
            file.ReadLine();
            while ((line = file.ReadLine()) != null){
                accounts.Add(line.Split('\t')[0],new String[] { line.Split('\t')[1], line.Split('\t')[2] });
            }
        }

        public void EditingFields(object sender, EventArgs e)
        {
            userNotFound.Visibility = Visibility.Hidden;
            BtnSignIn.IsEnabled = userName.Text.Length > 0;
        }

        public void Btn_SignIn(object sender, RoutedEventArgs e)
        {
            if (userName.Text.Length == 0) return;//can't sign in with no username
            Trace.WriteLine(String.Format("signIn testing {0}, {1}", userName.Text, userPwd.Password));
            Trace.WriteLine(String.Format("{0} in accounts? {1}, accounts pwd for {0} is {2}", userName.Text, accounts.ContainsKey(userName.Text), accounts.ContainsKey(userName.Text) ? accounts[userName.Text][0] : null));
            foreach (string key in accounts.Keys)
                Trace.WriteLine(string.Format("accounts: user {0} found, with password {1}", key, accounts[key]));
            foreach (string key in Model.users.Keys)
                Trace.WriteLine(string.Format("users: user {0} found", key));
            if (accounts.ContainsKey(userName.Text) && userPwd.Password.Equals(accounts[userName.Text][0])) {
                Trace.WriteLine(string.Format("attempting to load user {0}",userName.Text));
                String[] user = accounts[userName.Text];
                Model.user = Model.users[userName.Text];
                MainWindow._viewCommentHistory.ConfigComments();
                MainWindow._viewRecipe.SignIn.Visibility = Visibility.Hidden;
                DisableUser(false);
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
            }else
                userNotFound.Visibility = Visibility.Visible;
            MainWindow.model.ConfigureUser();
            if (Model.recipe!= null)
                MainWindow._viewRecipe.ConfigFavorite(Model.user.favorites.Contains(Model.recipe.name));
        }

        //disable visibility of controls that rely on being signed in
        public void Btn_skip(object sender, RoutedEventArgs e)
        {
            DisableUser(true);
            Model.user = null;
            Trace.WriteLine(String.Format("switch via skip from {0} to {1}", Switcher.viewCurrentEnum, MainWindow.Views.main));
            MainWindow.model.ConfigureUser();
            if (Switcher.viewPrevEnum!= MainWindow.Views.loading)
                if (Switcher.viewCurrentEnum != MainWindow.Views.user)
                    Switcher.Switch(Switcher.viewCurrentEnum);
                else
                    Switcher.Switch(Switcher.viewPrevEnum);
            else
                Switcher.Switch(MainWindow.Views.main);
        }

        public void DisableUser(Boolean set)
        {
            Visibility vis = set ? Visibility.Hidden : Visibility.Visible;
            MainWindow._viewMain.UserIco.Visibility =vis;
            MainWindow._viewUserAccount.userComments.Visibility = vis;
            MainWindow._viewUserAccount.userFavorites.Visibility = vis;
            MainWindow._viewRecipe.favoritesStarGrey.Visibility = vis;
            MainWindow._viewRecipe.favoritesStarYellow.Visibility = vis;
            MainWindow._viewRecipe.BtnComment.Visibility = vis;
            MainWindow._viewRecipe.BtnShare.Visibility = vis;
        }

        public void Btn_create(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(MainWindow.Views.createAccount);
        }
    }
}
