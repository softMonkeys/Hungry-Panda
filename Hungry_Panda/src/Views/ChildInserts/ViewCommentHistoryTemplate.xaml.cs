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
    /// Interaction logic for ViewCommentTemplate.xaml
    /// </summary>
    public partial class ViewCommentHistoryTemplate : UserControl
    {
        public ViewCommentHistoryTemplate()
        {
            InitializeComponent();
            Uid = Guid.NewGuid().ToString();
        }

        public void ConfigComments()
        {
            Trace.WriteLine(string.Format("config comments for user {0} with {1} comments",Model.user.userName,Model.user.comments.Count));
            commentsList.Children.Clear();
            foreach (CommentObj c in Model.user.comments)
            {
                Trace.WriteLine(string.Format("prepping user {0} with comment {1}", c.userName, c.commentText));
                commentsList.Children.Add(new ViewSingleCommentTemplate(c));
            }
        }

    }
}
