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
using System.Globalization;

namespace Hungry_Panda
{
    /// <summary>
    /// Interaction logic for ViewCommentTemplate.xaml
    /// </summary>
    public partial class ViewCommentTemplate : UserControl
    {
        public ViewCommentTemplate()
        {
            InitializeComponent();
            Uid = Guid.NewGuid().ToString();
        }

        public void ConfigComments()
        {
            commentsList.Children.Clear();
            foreach (CommentObj c in Model.recipe.comments)
                commentsList.Children.Add(new ViewSingleCommentTemplate(c));
        }

        public string GetDateTime()
        {
            return DateTime.Now.ToString(new CultureInfo("en-GB")).Replace(' ', '_');
        }

        public void Btn_Send(object sender, RoutedEventArgs e)
        {

            Trace.WriteLine("comment send");
            //id date_time   userName recipe  content
            int key = Model.nextCommentId++;
            string localDate = GetDateTime();
            string user = Model.user.userName;
            string recipe = recipeName.Text;
            string comment = commentText.Text;
            CommentObj c = new CommentObj(new string[] { ""+key, localDate, "_", "0", user, recipe, comment });
            Model.comments.Add(key,c);
            Model.recipe.AddComment(c);
            Model.user.AddComment(c);
            MainWindow._viewCommentHistory.ConfigComments();
            Model.WriteCommentsToFile(true);
            commentsList.Children.Add(new ViewSingleCommentTemplate(c));
            commentText.Text = "";
            MainWindow._viewMain.ancestor.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            MainWindow._viewMain.maxOffset_Y -= MainWindow._viewMain.ancestor.ActualHeight;
            Common.updateOffset(new Point(0, MainWindow._viewMain.maxOffset_Y), MainWindow._viewComment);
            //            Switcher.Switch(MainWindow.Views.favorites);
        }
        public void Btn_back(object sender, RoutedEventArgs e)
        {
            
            Switcher.Switch(Switcher.viewPrevEnum);
        }
    }
}
