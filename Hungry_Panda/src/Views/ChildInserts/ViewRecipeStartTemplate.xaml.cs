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
    /// Interaction logic for ViewRecipeStartTemplate.xaml
    /// </summary>
    public partial class ViewRecipeStartTemplate : UserControl
    {
        public double transform_y = 0;
        public ViewRecipeStartTemplate()
        {
            InitializeComponent();
            Uid = Guid.NewGuid().ToString();
        }

        public void Ingredients()
        {
            List<String[]> ingredients = Model.recipe.ingredients;
            IngredientsQty.Children.Clear();
            IngredientsName.Children.Clear();
            foreach (string[] ingredientPair in ingredients)
            {
                Button qty = new Button();
                Button name = new Button();
                Style thin = this.FindResource("ButtonThin") as Style;
                qty.Style = thin;
                name.Style = thin;
                qty.Click += new RoutedEventHandler(Btn_Ingredient);
                name.Click += new RoutedEventHandler(Btn_Ingredient);
                qty.Tag = ingredientPair[1];//associate name with object
                name.Tag = ingredientPair[1];//associate name with object
                name.Content = ingredientPair[1];
                qty.Content = " "+ingredientPair[0];
                IngredientsQty.Children.Add(qty);
                IngredientsName.Children.Add(name);
            }
            IngredientsGrid.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            double col0_width = (IngredientsGrid.Children[0]).DesiredSize.Width;
            double col2_width = (IngredientsGrid.Children[2]).DesiredSize.Width;
            for (int i = 0;i < IngredientsName.Children.Count; i++)
            {
                Button name = (Button)IngredientsName.Children[i];
                Button qty = (Button)IngredientsQty.Children[i];

                name.MaxWidth = col2_width*.9;
                qty.Width = col0_width+5;
                name.Measure(new Size(col2_width, double.PositiveInfinity));
                TextBlock text = new TextBlock
                {
                    FontSize = 24,
                    MaxWidth = col2_width*.85,
                    Text = name.Content.ToString(),
                    TextWrapping = TextWrapping.Wrap,
                    //Height = Double.NaN
                };
                name.Content = text;
                text.Measure(new Size(col2_width, double.PositiveInfinity));
                name.Height = text.DesiredSize.Height+5;
                qty.Height = name.Height;
                name.Measure(new Size(col2_width, double.PositiveInfinity));
            }
        }
                /// <summary>
                /// configure recipe configures step and end to match when triggered.  
                /// Call this when accessing a recipe start view.
                /// </summary>
            public void ConfigureRecipe()
        {
            RecipeStartLabel.Text = Model.recipe.name;
            PrepTime.Content = Model.recipe.times[0];
            CookingTime.Content = Model.recipe.times[1];
            RecipeStartImage.Source = Constants.PathToSource(Constants.Paths_Images.getImagesRecipesBasePath() + Model.recipe.image);
            Description.Text = Model.recipe.description;
            Ingredients();
            MainWindow._viewRecipeStep.ConfigRecipe();
            MainWindow._viewRecipeEnd.ConfigRecipe();
            MainWindow._viewComment.ConfigComments();
            MainWindow._viewComment.recipeName.Text = Model.recipe.name;
            Model.recipe.currentStep = 0;
            MainWindow._viewRecipe.Prev.IsEnabled = false;
            Uid = Model.recipe.Uids[0];
            if (Model.user == null) return;//if no user, don't try to config favorites
            MainWindow._viewRecipe.ConfigFavorite(Model.user.favorites.Contains(Model.recipe.name));
        }

        public void Btn_Ingredient(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            Trace.WriteLine(string.Format("RecipeStart -> ingredient = {0}", b.Tag));
            IEnumerable<string[]> ingredient =
                from stringArray in Model.recipe.ingredients
                where stringArray.Contains(b.Tag.ToString())
                select stringArray;
            MainWindow._viewIngredient.ConfigIngredient(ingredient.First());
            MainWindow._viewRecipe.SwapGuid(MainWindow._viewIngredient);
            if (!Common.TransformIsRegistered(this))
                Common.registerTransformNamed<StackPanel>(new Point(0, 0), this, "MainViewPane");
            Trace.WriteLine("replacing recipeMain transform in Ingredient");
            if(MainWindow._viewRecipe != null && MainWindow._viewRecipe.ancestor != null && MainWindow._viewRecipe.ancestor.RenderTransform != null && MainWindow._viewRecipe.ancestor.RenderTransform as TranslateTransform != null)
                Trace.WriteLine(string.Format("before (MainWindow._viewRecipe.ancestor.RenderTransform as TranslateTransform).Y = {0}", (MainWindow._viewRecipe.ancestor.RenderTransform as TranslateTransform).Y));
            MainWindow._viewRecipe.ancestor.RenderTransform = Common.getTransform(Uid);
            TranslateTransform transform = MainWindow._viewRecipe.ancestor.RenderTransform as TranslateTransform;
            transform_y = transform.Y;
            transform.Y = 0;
            MainWindow._viewRecipe.ancestor.RenderTransform = transform;
            Trace.WriteLine(string.Format("after (MainWindow._viewRecipe.ancestor.RenderTransform as TranslateTransform).Y = {0}", (MainWindow._viewRecipe.ancestor.RenderTransform as TranslateTransform).Y));
            Switcher.Switch(MainWindow.Views.ingredient);
        }
    }
}
