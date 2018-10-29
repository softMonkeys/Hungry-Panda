using System;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Model model;
        // TODO: make these private and write proper handlers
        public enum Views{
            main = 0,
            loading = 1,
            home=2,
            recipeStart=3,
            recipeStep=4,
            recipeEnd=5,
            search=6,
            ingredient = 7,
            popular=8,
            favorites = 9,
            results = 10,
            share = 11,
            comment = 12,
            user=13,
            signin=14,
            commentHistory = 15,
            createAccount = 16
        }

        // TODO: make these view objects private and write proper handlers
        public static ViewMainWindowTemplate _viewMain = new ViewMainWindowTemplate();
        public static ViewRecipeMainTemplate _viewRecipe = new ViewRecipeMainTemplate();
        public static ViewLoadingTemplate _viewLoading = new ViewLoadingTemplate();
        public static ViewHomeTemplate _viewHome = new ViewHomeTemplate();
        public static ViewSearchTemplate _viewSearch = new ViewSearchTemplate();
        public static ViewSignInTemplate _viewSignIn= new ViewSignInTemplate();

        // TODO: tie these to an actual userobject, so it does not require info in the constructor.
        public static ViewShareTemplate _viewShare = new ViewShareTemplate();
        public static ViewCommentTemplate _viewComment = new ViewCommentTemplate();
        public static ViewCommentHistoryTemplate _viewCommentHistory = new ViewCommentHistoryTemplate();
        public static ViewUserAccountTemplate _viewUserAccount = new ViewUserAccountTemplate();
        public static ViewCreateAccountTemplate _viewCreateAccount;// = new ViewCreateAccountTemplate();

        // TODO: tie this to an actual ingredient object, so it does not require info in the constructor.
        public static ViewIngredientTemplate _viewIngredient = new ViewIngredientTemplate();

        // TODO: tie these to actual recipe objects, so they do not require info, they pull the current recipe info instead;
        public static ViewRecipeStartTemplate _viewRecipeStart = new ViewRecipeStartTemplate();
        public static ViewRecipeStepTemplate _viewRecipeStep = new ViewRecipeStepTemplate();
        public static ViewRecipeEndTemplate _viewRecipeEnd = new ViewRecipeEndTemplate();
        //

        public MainWindow()
        {
            Trace.WriteLine("Init MainWindow");
            InitializeComponent();
          
            Switcher.pageSwitcher = this;
            Switcher.Switch(Views.loading);
            _viewLoading.loader.RunWorkerAsync();
            Trace.WriteLine("main switched to loading view");
            Trace.WriteLine("    Leaving constructor MainWindow");
        }

        public void Navigate(Views view)
        {
            Trace.WriteLine(view);
            switch (view.ToString())
            {
                case "loading":
                    Trace.WriteLine("content = loading");
                    Content = _viewLoading;
                    break;
                case "signin":
                    Trace.WriteLine("content = signin");
                    Content = _viewSignIn;
                    break;
                case "createAccount":
                    Trace.WriteLine("content = create Account");
                    _viewCreateAccount = new ViewCreateAccountTemplate();
                    _viewCreateAccount.ConfigCreate();
                    Content = _viewCreateAccount;
                    break;
                case "main":
                    Trace.WriteLine("content = main");
                    if (Model.user != null)
                    {
                        Trace.WriteLine("configuring user");
                        _viewMain.SetUser(Model.user);
                        _viewUserAccount.SetUser(Model.user);
                    }
                    if (Content != _viewMain)
                        Content = _viewMain;
                    Navigate(Views.popular);
                    break;
                case "home":
                    Trace.WriteLine("content = home");
                    if (Content != _viewMain)
                        Content = _viewMain;
                    _viewHome.PopularRecipes();
                    _viewMain.MainViewPane.Children.Clear();
                    _viewMain.MainViewPane.Children.Add(_viewHome);
                    _viewMain.SwapGuid(_viewHome);
                    break;
                case "recipeStart":
                    Trace.WriteLine("content = recipe start");
                    if (Content != _viewRecipe)
                        Content = _viewRecipe;
                    _viewRecipe.MainViewPane.Children.Clear();
                    _viewRecipe.SwapGuid(_viewRecipeStart);
                    _viewRecipeStart.ConfigureRecipe();
                    _viewRecipe.SetNavButtons();
                    _viewRecipe.MainViewPane.Children.Add(_viewRecipeStart);
                    break;
                case "recipeStep":
                    Trace.WriteLine("content = recipe step");
                    //if we already are recipeStep, so should not alter viewpane
                    if (Content != _viewRecipe)
                        Content = _viewRecipe;
                    if (!_viewRecipe.MainViewPane.Children.Contains(_viewRecipeStep)){
                        _viewRecipe.MainViewPane.Children.Clear();
                        _viewRecipe.MainViewPane.Children.Add(_viewRecipeStep);
                    }
                    _viewRecipe.SwapGuidStep();
                    break;
                case "recipeEnd":
                    Trace.WriteLine("content = recipe end");
                    if (Content != _viewRecipe)
                        Content = _viewRecipe;
                    _viewRecipe.MainViewPane.Children.Clear();
                    _viewRecipe.MainViewPane.Children.Add(_viewRecipeEnd);
                    _viewRecipe.SwapGuid(_viewRecipeEnd);
                    break;
                case "search":
                    Trace.WriteLine("content = search");
                    if (Content!=_viewMain)
                        Content = _viewMain;
                    _viewMain.MainViewPane.Children.Clear();
                    _viewMain.MainViewPane.Children.Add(_viewSearch);
                    _viewMain.SwapGuid(_viewSearch);
                    break;
                case "ingredient":
                    Trace.WriteLine("content = ingredient");
                    if (Content != _viewRecipe)
                        Content = _viewRecipe;
                    _viewRecipe.MainViewPane.Children.Clear();
                    _viewRecipe.MainViewPane.Children.Add(_viewIngredient);
                    _viewRecipe.SwapGuid(_viewIngredient);
                    break;
                case "favorites":
//                    _viewHome.FavoriteRecipes();
                    if (Content != _viewMain)
                        Content = _viewMain;
                    if (!_viewMain.MainViewPane.Children.Contains(_viewHome)){
                        _viewMain.MainViewPane.Children.Clear();
                        _viewMain.MainViewPane.Children.Add(_viewHome);
                    }
                    _viewMain.SwapGuid(_viewHome);
                    break;
                case "popular":
                    _viewHome.PopularRecipes();
                    if (Content != _viewMain)
                        Content = _viewMain;
                    if (!_viewMain.MainViewPane.Children.Contains(_viewHome)){
                        _viewMain.MainViewPane.Children.Clear();
                        _viewMain.MainViewPane.Children.Add(_viewHome);
                    }
                    _viewMain.SwapGuid(_viewHome);
                    break;
                case "results":
                    if (Content != _viewMain)
                        Content = _viewMain;
                    if (!_viewMain.MainViewPane.Children.Contains(_viewHome))
                    {
                        _viewMain.MainViewPane.Children.Clear();
                        _viewMain.MainViewPane.Children.Add(_viewHome);
                    }
                    _viewMain.SwapGuid(_viewHome);
                    break;
                case "share":
                    if (Content != _viewMain)
                        Content = _viewMain;
                    if (!_viewMain.MainViewPane.Children.Contains(_viewShare))
                    {
                        _viewMain.MainViewPane.Children.Clear();
                        _viewMain.MainViewPane.Children.Add(_viewShare);
                    }
                    _viewMain.SwapGuid(_viewHome);
                    break;
                case "comment":
                    if (Content != _viewMain)
                        Content = _viewMain;
                    if (!_viewMain.MainViewPane.Children.Contains(_viewComment))
                    {
                        _viewMain.MainViewPane.Children.Clear();
                        _viewMain.MainViewPane.Children.Add(_viewComment);
                    }
                    _viewMain.SwapGuid(_viewHome);
                    break;
                case "commentHistory":
                    if (Content != _viewMain)
                        Content = _viewMain;
                    if (!_viewMain.MainViewPane.Children.Contains(_viewCommentHistory))
                    {
                        _viewMain.MainViewPane.Children.Clear();
                        _viewMain.MainViewPane.Children.Add(_viewCommentHistory);
                    }
                    _viewMain.SwapGuid(_viewCommentHistory);
                    break;
                case "user":
                    if (Content != _viewMain)
                        Content = _viewMain;
                    if (!_viewMain.MainViewPane.Children.Contains(_viewUserAccount))
                    {
                        _viewMain.MainViewPane.Children.Clear();
                        _viewMain.MainViewPane.Children.Add(_viewUserAccount);
                    }
                    _viewMain.SwapGuid(_viewUserAccount);
                    break;
            }
        }
    }
}

