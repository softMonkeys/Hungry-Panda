using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Hungry_Panda
{
    public class UserObj
    {
        public string userName;
//        public ImageSource userIco;  //delay image source creation
        public string userIco;
        public string password;
        public List<CommentObj> comments;
        public List<string> favorites;

//        public UserObj(string name, string pwd, ImageSource ico) delay image source creation
        public UserObj(string name, string pwd, string ico)
        {
            userName = name;
            userIco = ico;
            password = pwd;
            comments = new List<CommentObj>();
            favorites = new List<string>();
        }
        public void AddComment(CommentObj c)
        {
            Trace.WriteLine(string.Format("adding comment {0} {1} to user {2}", c.id, c.commentText, c.userName));
            comments.Add(c);
        }
    }
}
