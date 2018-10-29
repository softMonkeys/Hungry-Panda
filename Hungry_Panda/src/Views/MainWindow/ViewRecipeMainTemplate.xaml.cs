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
    /// Interaction logic for ViewRecipeMain.xaml
    /// </summary>
    public partial class ViewRecipeMainTemplate : UserControl
    {
        string originalUid;
        const double Offset = 670;
        double maxOffset_X = 0;
        double maxOffset_Y = Offset;
        public FrameworkElement ancestor { private set; get; }

        public ViewRecipeMainTemplate()
        {
            Trace.WriteLine("entering viewRecipeMainTemplate constructor");
            InitializeComponent();
            Trace.WriteLine("    leaving viewRecipeMainTemplate constructor");
//            translate = new TranslateTransform();
            originalUid = Uid = Guid.NewGuid().ToString();
            ancestor = MainViewPane;
        }

        public void Btn_MainSearch(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(MainWindow.Views.search);
        }

        //TODO: tie this to a favorite recipies view template, same view template as popular and search results?
        public void Btn_MainFavorite(object sender, RoutedEventArgs e)
        {
            SetFavorite(favoritesStarGrey.IsVisible);//if grey, we tagged to favorite and switch to yellow, otherwise.
            Model.WriteFavoritesToFile(favoritesStarYellow.IsVisible);   //append if true, re-write if false
                                                                         //! to get opposite result to get original result from before toggle
        }

        public void setTransform(FrameworkElement view)
        {
            Trace.WriteLine("    setTransform");
            if (!Common.TransformIsRegistered(view))
                Common.registerTransformNamed<StackPanel>(new Point(0, 0), view as UserControl, "MainViewPane");
            if (ancestor != null)
            {
                Trace.WriteLine(string.Format("replacing recipeMain transform in setTransform with {0}", view.Uid));
                ancestor.RenderTransform = Common.getTransform(view.Uid);
            }
            else
                Trace.WriteLine("ancestor is currently null");
        }


        public void ConfigFavorite(bool fav)
        {
            Trace.WriteLine(String.Format("configure favorite {0} in viewRecipeMainTemplate to {1}", favoritesStarGrey.IsVisible, fav));
            if (fav)
            {
                string path = Constants.Paths_Images.getImagesUIPath() + "yellow-star-md.png";
                Trace.WriteLine("setting favorite "+path);
                favoritesStarGrey.Visibility = Visibility.Hidden;
                favoritesStarYellow.Visibility = Visibility.Visible;
            }
            else
            {
                string path = Constants.Paths_Images.getImagesUIPath() + "grey-star-md.png";
                Trace.WriteLine("setting favorite "+path);
                favoritesStarGrey.Visibility = Visibility.Visible;
                favoritesStarYellow.Visibility = Visibility.Hidden;
            }
        }

        public void SetFavorite(bool fav)
        {
            Trace.WriteLine(String.Format("setting favorite {0} in viewRecipeMainTemplate to {1}", favoritesStarGrey.IsVisible, fav));
            ConfigFavorite(fav);
            if (fav)
                Model.AddFavorite(Model.user.userName, Model.recipe.name);
            else
                Model.RemoveFavorite(Model.user.userName,Model.recipe.name);
        }

        public void Btn_MainShare(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(MainWindow.Views.share);
        }

        public void Btn_MainComment(object sender, RoutedEventArgs e)
        {
            setTransform(MainWindow._viewComment);
            Switcher.Switch(MainWindow.Views.comment);
        }
        public void Btn_Prev(object sender, RoutedEventArgs e)
        {
            if (Model.recipe.currentStep == 0)
            {
                Switcher.Switch(Switcher.viewPrevEnum);
                return;
            }
            if (--Model.recipe.currentStep == 0)
                Switcher.Switch(MainWindow.Views.recipeStart);
            else
            {
                Switcher.Switch(MainWindow.Views.recipeStep);
                RefreshStep();
            }
            SetNavButtons();
            if (!Common.TransformIsRegistered(this))
                Common.registerTransformNamed<StackPanel>(new Point(maxOffset_X, maxOffset_Y), this, "MainViewPane");
            if (ancestor != null)
            {
                Trace.WriteLine("replaceing recipeMain transform in Prev");
                ancestor.RenderTransform = Common.getTransform(Uid);
            }
            else
                ancestor.RenderTransform = new TranslateTransform();
        }
        public void Btn_Next(object sender, RoutedEventArgs e)
        {
            if (++Model.recipe.currentStep == Model.recipe.totalSteps)
                Switcher.Switch(MainWindow.Views.recipeEnd);
            else
            {
                Switcher.Switch(MainWindow.Views.recipeStep);
                RefreshStep();
            }
            SetNavButtons();
            if(!Common.TransformIsRegistered(this))
                Common.registerTransformNamed<StackPanel>(new Point(maxOffset_X, maxOffset_Y), this, "MainViewPane");
            if (ancestor != null)
            {
                Trace.WriteLine("replaceing recipeMain transform in Next");
                ancestor.RenderTransform = Common.getTransform(Uid);
            }
        }

        public void RefreshStep()
        {
            ViewRecipeStepTemplate v = MainWindow._viewRecipeStep;
            double stepNum = Model.recipe.currentStep;
            Trace.WriteLine(string.Format("configure recipeStep {0}", Model.recipe.currentStep));
            string[] step = Model.recipe.steps[(int)stepNum-1];
            v.RecipeStepTitle.Text = step[1];
            string path = Constants.Paths_Images.getImagesRecipesBasePath() + step[0];
            v.RecipeStepImage.Source = Constants.PathToSource(path);
            v.RecipeStepText.Text = step[2];
            Trace.Write(String.Format("recipeMain changing Uid from {0}",Uid));
            Uid = Model.recipe.Uids[Model.recipe.currentStep];
            Trace.WriteLine(String.Format(" to {0}", Uid));
            v.progress.Value = stepNum;
            SwapGuidStep();
            maxOffset_Y = Offset - ancestor.ActualHeight;
            Common.updateOffset(new Point(maxOffset_X, maxOffset_Y), this);
        }

        public void SetNavButtons()
        {
            int s = (int)Model.recipe.currentStep;
            int f = Model.recipe.totalSteps;
            Next.IsEnabled = true;
            Prev.IsEnabled = true;

            if (Switcher.viewCurrentEnum == MainWindow.Views.recipeStart)
            {//at start
                Next.Content = "Start Cooking";
                //                Prev.IsEnabled = false;
                Prev.Content = "Back";
            }
            else if (Switcher.viewCurrentEnum == MainWindow.Views.recipeEnd)
            {//at start
                Next.IsEnabled = false;
                Next.Content = "Recipe Complete";
            }
            else if (s == 0)//step 1, prev->start
                Prev.Content = "Back to Start";
            else if (s == f)//at end
                Next.Content = "Finish";
            else
            {
                Next.Content = "Next Step";
                Prev.Content = "Prev Step";
            }
        }
        
        //TODO: tie this to a popular recipies view template, same view template as favorites and search results?
        public void Btn_MainHome(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            Trace.WriteLine(string.Format("button Tag={0}",b.Tag.ToString()));
            switch (b.Tag.ToString())
            {
                case "Go":
                    confirmation.Visibility = Visibility.Hidden;
                    Switcher.Switch(MainWindow.Views.home);
                    break;
                case "Confirm":
                    confirmation.Visibility = Visibility.Visible;
                    break;
                case "Back":
                    confirmation.Visibility = Visibility.Hidden;
                    break;
            }
        }

        private void Control_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {//https://stackoverflow.com/questions/1495408/how-to-drag-a-usercontrol-inside-a-canvas
            Common.Control_MouseLeftButtonDown(sender, e, this);
        }

        private void Control_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {//https://stackoverflow.com/questions/1495408/how-to-drag-a-usercontrol-inside-a-canvas
            bool dragging = Common.dragging;
            Common.Control_MouseLeftButtonUp(sender, e, this, ancestor);
            if (dragging) return;//we're done, we got dealt with;
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {//https://stackoverflow.com/questions/1495408/how-to-drag-a-usercontrol-inside-a-canvas
            Common.Control_MouseMove(sender, e, this, ancestor);
        }
        private void Btn_SignIn(object sender, MouseButtonEventArgs e)
        {
            Switcher.Switch(MainWindow.Views.signin);
        }

        private void Control_Leave(object sender, RoutedEventArgs e)
        {//mouse out of elment, drop capture as a minimum!
            Common.Control_Leave(sender, e, this, ancestor);
        }

        public void SwapGuid(UserControl control)
        {
            Trace.WriteLine(string.Format("RecipeMain swap Uid from {0} to {1}",Uid, control.Uid));
            Uid = control.Uid;
        }

        public void SwapGuidStep()
        {
            Model.recipe.getUid();
        }

        private void Control_MouseEnter(object sender, MouseEventArgs e)
        {
            maxOffset_Y = Offset - ancestor.ActualHeight;
            Trace.WriteLine(string.Format("Recipe MouseDown with Uid {0}", Uid));
            Common.registerTransformNamed<StackPanel>(new Point(maxOffset_X, maxOffset_Y), this, "MainViewPane");
            Common.registerTransformNamed<StackPanel>(new Point(0, 0), MainWindow._viewIngredient, "MainViewPane");
        }
    }
}
