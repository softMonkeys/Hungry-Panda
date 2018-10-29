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
    /// Interaction logic for ViewIngredientTemplate.xaml
    /// </summary>
    public partial class ViewIngredientTemplate : UserControl
    {
        public Transform oldTransform;
        public ViewIngredientTemplate()
        {
            InitializeComponent();
            Uid = Guid.NewGuid().ToString();
        }

        public void ConfigIngredient(string[] ingredient)
        {
            Trace.WriteLine(String.Format("path to ingredient is : {0}", Constants.Paths_Images.getImagesIngredientsPath() + "ingredient[2]"));
            IngredientImage.Source = new BitmapImage(new Uri(Constants.Paths_Images.getImagesIngredientsPath() + ingredient[2], UriKind.Absolute));
            IngredientLabel.Text = ingredient[1];
        }
        public void Btn_prev(object sender, RoutedEventArgs e)
        {
            TranslateTransform transform = MainWindow._viewRecipe.ancestor.RenderTransform as TranslateTransform;
            transform.Y = MainWindow._viewRecipeStart.transform_y;
            MainWindow._viewRecipe.ancestor.RenderTransform = transform;
            Switcher.Switch(MainWindow.Views.recipeStart);
        }
    }
}
