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
    /// Interaction logic for ViewRecipeEndTemplate.xaml
    /// </summary>
    public partial class ViewRecipeEndTemplate : UserControl
    {
        public ViewRecipeEndTemplate()
        {
            InitializeComponent();
            Uid = Guid.NewGuid().ToString();

        }
        // TODO: kill me, only reason to exist is to allow a quick and forced recipe demo
        public void ConfigDemoRecipe()
        {
            RecipeEndLabel.Text = "Recipe Complete : Kung-Pao Chicken";
        }
        public void ConfigRecipe()
        {
            RecipeObj recipe = Model.recipe;
            Trace.WriteLine(string.Format("config Recipe called with current step= {0}", recipe.currentStep));
            RecipeEndLabel.Text = String.Format("Recipe Complete : {0}\n{1}",recipe.name,recipe.steps[recipe.totalSteps-1][1]);
            string path = Constants.Paths_Images.getImagesRecipesBasePath() + recipe.steps[recipe.totalSteps - 1][0];
            RecipeEndImage.Source = Constants.PathToSource(path);
            Uid = recipe.Uids[recipe.totalSteps];
        }
    }
}
