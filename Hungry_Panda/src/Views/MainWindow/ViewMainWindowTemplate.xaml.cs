using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for MainWindowTemplate.xaml
    /// </summary>
    public partial class ViewMainWindowTemplate : UserControl
    {
        //        protected bool isDragging;
        //        private Point clickPosition;
        //        private Point lastPosition;
        //        double yPos;
        //        double yDelta;
        //        StackPanel ancestor;
        //        TranslateTransform stackPanelTransform;
        //        public double maxOffset;
        string originalUid;
        double maxOffset_X = 0;
        public double maxOffset_Y = 630;
        public FrameworkElement ancestor;
        public ViewMainWindowTemplate()
        {
            Trace.WriteLine("entering viewMainWindowTemplate constructor");
            InitializeComponent();
            Constants.SetRootFolder((new System.IO.DirectoryInfo(Environment.CurrentDirectory)).Parent.Parent.FullName + "\\");
            Trace.WriteLine("root is: " + Constants.GetRootFolder());
            Trace.WriteLine("entering viewMainWindowTemplate constructor");
            //            stackPanelTransform = new TranslateTransform();
            originalUid = Uid = Guid.NewGuid().ToString();
            ancestor = MainViewPane;
        }

        public void SetUser(UserObj user)
        {
            Trace.WriteLine(string.Format("user.userName = {0}, user.userIco = {1}", user.userName, user.userIco));
            this.UserName.Content = user.userName;
            //            this.UserIco.Source = user.userIco;  //delay image source creating until last possible moment
            this.UserIco.Source = Constants.PathToSource(Constants.Paths_Images.getImagesUserIconsPath() + user.userIco);
        }

        public void Btn_MainSearch(object sender, RoutedEventArgs e)
        {
            setTransform(MainWindow._viewSearch);
            MainWindow._viewSearch.clearResults();
            Switcher.Switch(MainWindow.Views.search);
        }

        //TODO: tie this to a favorite recipies view template, same view template as popular and search results?
        public void Btn_MainFavorite(object sender, RoutedEventArgs e)
        {
            if (Model.user == null)
                Switcher.Switch(MainWindow.Views.signin);
            else
            {
                setTransform(MainWindow._viewUserAccount);
                Switcher.Switch(MainWindow.Views.user);
            }
        }

        //TODO: tie this to a popular recipies view template, same view template as favorites and search results?
        public void Btn_MainHome(object sender, RoutedEventArgs e)
        {
            setTransform(MainWindow._viewHome);
            Switcher.Switch(MainWindow.Views.popular);
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


        private void Control_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {//https://stackoverflow.com/questions/1495408/how-to-drag-a-usercontrol-inside-a-canvas
            Common.Control_MouseLeftButtonDown(sender, e, this);
            /*
            if (ancestor == null)
            {
                ancestor = Common.TryFindAncestorFromPoint<StackPanel>(sender as UIElement, Mouse.GetPosition(this));
                if (ancestor == null)//do not try to drag if still null
                    return;
            }
            Trace.WriteLine(string.Format("ancestor DesiredSize.Height = {0}, ancestor Actual.Height = {1}, ancestor Extent Height = {2}", ancestor.DesiredSize.Height,ancestor.ActualHeight, ancestor.ExtentHeight));
            if (ancestor.DesiredSize.Height <= ancestor.MinHeight) return;//completely fits, dragging unnecessary
            ancestor.CaptureMouse();
            isDragging = true;
            Trace.WriteLine(string.Format("ancestor size(height) = {0}", ancestor.ActualHeight));
            maxOffset = 411 - ancestor.ActualHeight;
            lastPosition = clickPosition = e.GetPosition(this);
            */
        }

        private void Control_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {//https://stackoverflow.com/questions/1495408/how-to-drag-a-usercontrol-inside-a-canvas
            bool dragging = Common.dragging;
            Common.Control_MouseLeftButtonUp(sender, e, this, ancestor);
            if (dragging) return;//we're done, we got dealt with;

            /*
            if (ancestor == null)//exit early if null, something is wrong.
                return;
            ancestor.ReleaseMouseCapture();
            isDragging = false;
            */
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {//https://stackoverflow.com/questions/1495408/how-to-drag-a-usercontrol-inside-a-canvas
            Common.Control_MouseMove(sender, e, this, ancestor);
            /*
            if (ancestor == null) return;
            Point currentPosition = e.GetPosition(this);
            if (isDragging && lastPosition.Y != currentPosition.Y)
            {
                if (!ancestor.RenderTransform.Equals(stackPanelTransform))
                    ancestor.RenderTransform = stackPanelTransform;
                yDelta = currentPosition.Y - lastPosition.Y;
                stackPanelTransform.Y = Math.Max(maxOffset, Math.Min(0, stackPanelTransform.Y + yDelta));
                Trace.WriteLine(string.Format("DesiredSize of transform = {0}", ancestor.DesiredSize));
                Trace.WriteLine(string.Format("transform.Y = {0}", stackPanelTransform.Y));
                lastPosition = currentPosition;
            }
            */
        }

        private void Control_Leave(object sender, RoutedEventArgs e)
        {
            Common.Control_Leave(sender, e, this, ancestor);
        }

        public void SwapGuid(UserControl control)
        {
            Uid = control.Uid;
        }

        private void Control_MouseEnter(object sender, MouseEventArgs e)
        {
            if (ancestor == null)
                ancestor = Common.TryFindAncestorFromPoint<StackPanel>(sender as UIElement, Mouse.GetPosition(this));
            if (ancestor == null) return;
            maxOffset_Y -= ancestor.ActualHeight;
            Common.registerTransformNamed<StackPanel>(new Point(maxOffset_X, maxOffset_Y), this, "MainViewPane");
        }
    }
}
