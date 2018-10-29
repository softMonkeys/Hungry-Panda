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
    //TODO: kill all local data, none of it needs exist, should all come from a system model object
    /// <summary>
    /// Interaction logic for ViewRecipeStepTemplate.xaml
    /// </summary>
    public partial class ViewRecipeStepTemplate : UserControl
    {
        public ViewRecipeStepTemplate()
        {
            InitializeComponent();
            Uid = Guid.NewGuid().ToString();
        }

        public void ConfigRecipe()
        {
            RecipeObj recipe = Model.recipe;
            progress.Value = 0;
            progress.Maximum = recipe.totalSteps;
            string path = Constants.Paths_Images.getImagesRecipesBasePath() + recipe.steps[0][0];
            RecipeStepImage.Source = Constants.PathToSource(path);
            RecipeStepTitle.Text = recipe.steps[0][1];
            RecipeStepText.Text = recipe.steps[0][2];
        }
        public void RefreshStep()
        {
            RecipeObj recipe = Model.recipe;
            double stepNum = recipe.currentStep;
            string[] step = recipe.steps[(int)stepNum];
            Trace.WriteLine(string.Format("configure recipeStep {0} for stepName {1} and stepText {2}", Model.recipe.currentStep, step[1], step[2]));
            RecipeStepTitle.Text = step[1];
            Trace.Write(string.Format("refresh Uid from {0}", Uid));
            Trace.WriteLine(string.Format("to {0}", Uid));
            Uid = Model.recipe.Uids[Model.recipe.currentStep];
            MainWindow._viewRecipe.SwapGuid(this);
            string path = Constants.Paths_Images.getImagesRecipesBasePath() + step[0];
            RecipeStepImage.Source = Constants.PathToSource(path);
            RecipeStepText.Text = step[2];
            progress.Value = stepNum+1;
        }
    }
}
