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
    /// Interaction logic for ViewRecipeListElement.xaml
    /// should tie this to a dedicated recipe class with all the recipe info, faking by providing elements directly
    /// </summary>
    public partial class ViewRecipeListElement : UserControl
    {
        public string Difficulty {
            get { return this.difficultyString.Content.ToString(); }
            set { this.difficultyString.Content = value; }
        }
        public string Ethnicity
        {
            get { return this.ethnicityString.Content.ToString(); }
            set { this.ethnicityString.Content = value; }
        }
        public string RecipeName
        {
            get { return this.nameString.Text.ToString(); }
            set { this.nameString.Text = value; }
        }
        /// <summary>
        /// assumes complete path is provided from caller.
        /// </summary>
        public string ImgPath
        {
            get { return thumbnailPathString.Source.ToString(); }
            set {
                Trace.WriteLine(String.Format("incoming path is {0}",value));
                string path = Constants.Paths_Images.getImagesRecipesBasePath();
                if (value.Contains(path))
                    thumbnailPathString.Source = Constants.PathToSource(value);
                else
                    thumbnailPathString.Source = Constants.PathToSource(path+value);
            }
}

// TODO: kill this once we tie object to a recipe object, using to carry temporary info forwards
public int Steps { get; set; }
        // TODO: tie this to a class object for a recipe
        public ViewRecipeListElement(string name, string thumbnail, string difficulty, string ethnicity, int steps)
        {
            InitializeComponent();
            RecipeName = name;
            ImgPath = thumbnail;
            Difficulty = difficulty;
            Ethnicity = ethnicity;
            Steps = steps;
        }
        public ViewRecipeListElement(RecipeObj r)
        {
            InitializeComponent();
            RecipeName = r.name;
            ImgPath = r.image;
            Difficulty = r.difficulty;
            Ethnicity = r.ethnicity;
            Steps = r.totalSteps;
        }
    }
}
