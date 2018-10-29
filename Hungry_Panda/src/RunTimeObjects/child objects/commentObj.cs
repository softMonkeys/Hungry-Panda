using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hungry_Panda
{
    public class CommentObj
    {
        public string commentText { get; private set; }
        public string commentDate { get; private set; }
        public string commentTime { get; private set; }
        public string editedDate { get; private set; }
        public string editedTime { get; private set; }
        public string parentRecipe { get; private set; }
        public string userName { get; private set; }
        public string id { get; private set; }
        public string deleted { get; set; }
        public CommentObj(string[] comment)
        {
            //id date_time   userName recipe  content
            id = comment[0];
            commentDate = comment[1].Split('_')[0];
            commentTime = comment[1].Split('_')[1];
            editedDate = comment[2].Split('_')[0];
            editedTime = comment[2].Split('_')[1];
            deleted = comment[3];
            userName = comment[4];
            parentRecipe = comment[5];
            commentText = comment[6];
            //replace \t and \n with legit characters
            string parsedCommentText = commentText.Replace("\n", "\\n").Replace("\t", "\\t");
        }

        public string[] ToStringArray()
        {
            //id date_time   userName recipe  content
            string[] comment = new string[7];
            comment[0] = id;
            comment[1] = commentDate + '_' + commentTime;
            comment[2] = editedDate + '_' + editedTime;
            comment[3] = deleted;
            comment[4] = userName;
            comment[5] = parentRecipe;
            comment[6] = commentText.Replace("\n", "\\n").Replace("\t", "\\t");
            return comment;
        }
        public void edit(string comment)
        {
            commentText = comment;
            string[] localTime = MainWindow._viewComment.GetDateTime().Split('_');
            editedDate = localTime[0];
            editedTime = localTime[1];
        }
    }
}
