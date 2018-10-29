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
using System.Windows.Threading;

namespace Hungry_Panda
{
    /// <summary>
    /// Interaction logic for ViewSingleCommentTemplate.xaml
    /// </summary>
    public partial class ViewSingleCommentTemplate : UserControl
    {
        CommentObj cmt;
        bool delete;
        bool edit;
        string oldComment;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="ico"> expects complete path to icon file</param>
        /// <param name="user"></param>
        public ViewSingleCommentTemplate(CommentObj c)
        {
            InitializeComponent();
            cmt = c;
            delete = edit = false;
            commentText.Text = c.commentText;
            commentDate.Content = c.commentDate;
            commentTime.Content = c.commentTime;
            //            commentUserIco.Source = Model.users[c.userName].userIco;  delayed source creation
            commentUserIco.Source = Constants.PathToSource(Constants.Paths_Images.getImagesUserIconsPath()+Model.users[c.userName].userIco);
            commentUserName.Content = c.userName;
            RecipeLink.Content = c.parentRecipe;
            if (c.editedDate.Length > 0)
                setEditedVisible(c.editedDate, c.editedTime);
            if (cmt.deleted == "1" || Model.user == null || !Model.user.userName.Equals(c.userName))
            {//true
                Edit.Visibility = Visibility.Hidden;
                Delete.Visibility = Visibility.Hidden;
            }
        }

        public void setEditedVisible(string date, string time)
        {
            Edited.Visibility = Visibility.Visible;
            EditedDate.Content = date;
            EditedDate.Visibility = Visibility;
            EditedTime.Content = time;
            EditedTime.Visibility = Visibility;
        }

        public void Btn_delete(object sender, RoutedEventArgs e)
        {
            delete = true;
            Edit.Visibility = Visibility.Hidden;
            Delete.Visibility = Visibility.Hidden;
            Confirm.Visibility = Visibility.Visible;
            Cancel.Visibility = Visibility.Visible;
            return;
        }

        private void Btn_edit(object sender, RoutedEventArgs e)
        {
            edit = true;
            Confirm.Visibility = Visibility.Visible;
            Cancel.Visibility = Visibility.Visible;
            Edit.Visibility = Visibility.Hidden;
            Delete.Visibility = Visibility.Hidden;
            TextBox c = getCommentTextBox(sender);
            oldComment = c.Text;
            c.BorderBrush = System.Windows.Media.Brushes.Black;
            c.IsReadOnly = false;
            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() => c.Focus()));
        }

        private TextBox getCommentTextBox(object sender)
        {
            Label lbl = (sender as Label);
            Grid grid = (Grid)lbl.Parent;
            return (TextBox)grid.FindName("commentText");
        }

        private void Btn_cancel(object sender, RoutedEventArgs e)
        {
            Confirm.Visibility = Visibility.Hidden;
            Cancel.Visibility = Visibility.Hidden;
            Edit.Visibility = Visibility.Visible;
            Delete.Visibility = Visibility.Visible;
            edit = delete = false;
            TextBox c = getCommentTextBox(sender);
            c.BorderBrush = System.Windows.Media.Brushes.Transparent;
            c.Text = oldComment;
            c.IsReadOnly = true;
        }
        private void Btn_confirm(object sender, RoutedEventArgs e)
        {
            Label lbl = (sender as Label);
            Grid grid = (Grid)lbl.Parent;
            CommentObj c = Model.comments[int.Parse(cmt.id)];
            TextBox t = ((TextBox)grid.FindName("commentText"));
            if (delete)
            {
                t.Text = "This comment has been deleted";
                c.deleted = "1";
            }
            else if (edit)
            {
                Edit.Visibility = Visibility.Visible;
                Delete.Visibility = Visibility.Visible;
            }
            c.edit(t.Text);
            setEditedVisible(c.editedDate, c.editedTime);
            Model.WriteCommentsToFile(false);
            Confirm.Visibility = Visibility.Hidden;
            Cancel.Visibility = Visibility.Hidden;
            edit = delete = false;
            t.BorderBrush = System.Windows.Media.Brushes.Transparent;
            t.IsReadOnly = true;
        }

        private void returnToRecipe(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine(string.Format("link back to {0} from comment", (sender as Button).Content.ToString()));
            Model.recipe = Model.recipes[(sender as Button).Content.ToString()];
            MainWindow._viewRecipeStart.ConfigureRecipe();
            Switcher.Switch(MainWindow.Views.recipeStart);
        }
    }
}
