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
    /// Interaction logic for HomeViewTemplate.xaml
    /// </summary>
    public partial class ViewHomeTemplate : UserControl
    {
//        Point p;
        public ViewHomeTemplate()
        {
            InitializeComponent();
            Uid = Guid.NewGuid().ToString();

        }

        public void PopularRecipes()
        {
            Trace.WriteLine("entry of PopularRecipes() in viewHomeTemplate");
            recipeList.Children.Clear();
            noRecipe("", false);
            Trace.WriteLine(string.Format("Recipes = {0}", Constants.Paths_Images.getImagesRecipesBasePath()));
            if (Model.recipes.ToArray().Length > 0)
            {
                foreach (string recipe in Model.recipes.Keys)
                {
                    RecipeObj recipeObj = Model.recipes[recipe];
                    Trace.WriteLine(String.Format("assigning {0} image {1} at {2}", recipeObj.name, recipeObj.image_thumb, Constants.Paths_Images.getImagesRecipesBasePath() + recipeObj.image_thumb));
                    recipeList.Children.Add(new ViewRecipeListElement(recipeObj.name, Constants.Paths_Images.getImagesRecipesBasePath() + recipeObj.image_thumb, recipeObj.difficulty, recipeObj.ethnicity, recipeObj.steps.Count()));
                }
            }
            else
                noRecipe("No Recipes Available", true);
            paneLabel.Content = "Dishes of the Day";
            Trace.WriteLine(String.Format("size of recipeList = {0}", this.ActualHeight));
            Trace.WriteLine("    exit from PopularRecipes() in viewHomeTemplate");
        }

        // TODO: filter the results via favorited.
        public void FavoriteRecipes()
        {
            string imagePath = Constants.Paths_Images.getImagesRecipesBasePath();
            paneLabel.Content = "Your Favorites";
            recipeList.Children.Clear();
            noRecipe("", false);
            Trace.WriteLine(string.Format("root = {0} and Recipes = {1}", Constants.GetRootFolder(),imagePath));
            if (Model.user.favorites.Count > 0)
            {
                foreach (string recipe in Model.user.favorites)
                {
                    if (Model.recipes.ContainsKey(recipe))
                    {
                        RecipeObj recipeObj = Model.recipes[recipe];
                        recipeList.Children.Add(new ViewRecipeListElement(recipeObj.name, imagePath + recipeObj.image_thumb, recipeObj.difficulty, recipeObj.ethnicity, recipeObj.steps.Count()));
                    }
                }
            }
            else
                noRecipe("You Have No Favorites", true);

            paneLabel.Content = "Your Favorites";
        }

        public void noRecipe(string content,bool hide)
        {
            Trace.WriteLine(string.Format("noRecipe called with content \"{0}\", hide = {1}",content,hide));
            if (MainWindow._viewHome == null)
                Trace.WriteLine("_viewHome is null");
            else if (MainWindow._viewHome.NoRecipes == null)
                Trace.WriteLine("_viewHome.NoRecipes is null");
            MainWindow._viewHome.NoRecipes.Content = content;
            MainWindow._viewHome.NoRecipes.Visibility = hide ? Visibility.Visible : Visibility.Hidden;
        }

        public void ReportMousePos()
        {
            Point p = Mouse.GetPosition(this);
            Trace.WriteLine(string.Format("mouse is currently at ({0},{1})", p.X, p.Y));
        }

        private void RecipeList_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Trace.WriteLine(string.Format("RecipeList_MouseUp e.handled? {0}", e.Handled));
            if (e.Handled) return;//already dealt with, else...
            Trace.WriteLine(String.Format("RecipeList MouseUp Event called"));
            //            ReportMousePos();
//                ViewRecipeListElement elem = sender as ViewRecipeListElement;
            ViewRecipeListElement elem = Common.TryFindAncestorFromPoint<ViewRecipeListElement>(sender as UIElement, Mouse.GetPosition(MainWindow._viewHome.recipeList));
            Trace.WriteLine(String.Format("RecipeList MouseUp Event called via {0}", elem.RecipeName));
            Model.recipe = Model.recipes[elem.RecipeName];
            MainWindow._viewRecipeStart.ConfigureRecipe();
            Switcher.Switch(MainWindow.Views.recipeStart);
        }
    }
}
