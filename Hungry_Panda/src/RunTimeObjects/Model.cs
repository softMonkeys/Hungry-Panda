using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Hungry_Panda
{
    /// <summary>
    /// used to contan the corrent model of the program.
    /// needs to store all of the currently loaded information, 
    ///     and act as middle man for all the interfaces.
    /// </summary>
    public class Model
    {
        //current user object
        public static UserObj user;
        //current recipe object
        public static RecipeObj recipe;
        //current comment object
        public static CommentObj comment;
        //current ingredientobject
        //users object list
        public static Dictionary<string,UserObj> users;
        //recipes object list
        public static Dictionary<string,RecipeObj> recipes;
        //comment Objects Dictionary
        public static Dictionary<int, CommentObj> comments;
        //search object list
        public static List<RecipeObj> search;
        //home object list
        public static List<RecipeObj> recommend;
        //favorites object list
        public static Dictionary<string, List<string>> favorites;
        //comments object list
        public static int nextCommentId;

        public Model()
        {
            search = new List<RecipeObj>();
            recommend = new List<RecipeObj>();
            favorites = new Dictionary<string, List<string>>();
            recipes = new Dictionary<string, RecipeObj>();
            comments = new Dictionary<int, CommentObj>();
        }
        public void LoadData()
        {
            Trace.WriteLine("loadData entry");
            int sleepDir = 125;//to make it noticable, else its too fast
            Trace.WriteLine("loading account info");
            // load accounts info
            Trace.WriteLine("Load Accounts");
            GetAccountsFromFile();
            Trace.WriteLine("Accounts Loaded");
            MainWindow._viewLoading.loader.ReportProgress(13);
            Thread.Sleep(sleepDir);
            MainWindow._viewLoading.loader.ReportProgress(26);
            Thread.Sleep(sleepDir);

//            Trace.WriteLine("loading recipe info");
            // load recipe info
            Trace.WriteLine("Load Recipes");
            GetRecipesFromFile();
            Trace.WriteLine("Recipes Loaded");
            MainWindow._viewLoading.loader.ReportProgress(39);
            Thread.Sleep(sleepDir);
            MainWindow._viewLoading.loader.ReportProgress(52);
            Thread.Sleep(sleepDir);

            // load and associate comment info
            //      attach to recipes
            //      attach to comment history
//            Trace.WriteLine("loading comments");
            Trace.WriteLine("Load Comments");
            GetCommentsFromFile();
            Trace.WriteLine("Comments Loaded");
            MainWindow._viewLoading.loader.ReportProgress(65);
            Thread.Sleep(sleepDir);
            MainWindow._viewLoading.loader.ReportProgress(78);
            Thread.Sleep(sleepDir);

            //load favorites from file
            //            Trace.WriteLine("loading favorites");
            Trace.WriteLine("Load favorites");
            GetFavoritesFromFile();
            Trace.WriteLine("favorites Loaded");
            MainWindow._viewLoading.loader.ReportProgress(91);
            Thread.Sleep(sleepDir);
            MainWindow._viewLoading.loader.ReportProgress(100);
            Trace.WriteLine("loading completed");
        }

        public void GetRecipesFromFile()
        {
//            String path = Constants.root + Constants.Paths_Data.Recipes;
            String[] directories = Directory.GetDirectories(Constants.Paths_Data.GetDatabaseRecipeFolderPath());
            string recipeFile = "\\recipe";
            Trace.WriteLine("loading recipes from file");
            foreach (string dir in directories)
            {
                Trace.Write(String.Format("loading {0} which splits to ", dir));
                foreach (string piece in dir.Split('\\'))
                    Trace.Write(piece + ", ");
                Trace.WriteLine(String.Format("  with {0} elements", dir.Split('\\').Length));
                Trace.WriteLine(String.Format("    Last element is element {0}", dir.Split('\\').Length - 1));
                string recipeName = dir.Split('\\')[dir.Split('\\').Length-1];
                Trace.Write(String.Format("found {0}, loading file {1}", recipeName, dir + recipeFile));
                StreamReader file = new StreamReader(Constants.Paths_Data.GetDatabaseRecipeFilePath(recipeName));
                RecipeObj r = new RecipeObj(file);
                recipes.Add(r.name, r);
            }
        }

        public void GetCommentsFromFile()
        {
            Trace.WriteLine("opening comments file at " + Constants.Paths_Data.GetCommentsFilePath());
            StreamReader file = new StreamReader(Constants.Paths_Data.GetCommentsFilePath());
            string line;
            file.ReadLine();
            List<string[]> commentStrings = new List<string[]>();
            while ((line = file.ReadLine()) != null)
            {
                //id date_time   userName recipe  content
                String[] comment = line.Split('\t');
                commentStrings.Add(comment);
            }
            foreach (String[] comment in commentStrings)
            {
                CommentObj commentObj = new CommentObj(comment);
                int id = int.Parse(commentObj.id);
                nextCommentId = Math.Max(nextCommentId,id+1);
                comments.Add(id,commentObj);
            }
            //associate with users
            foreach(int key in comments.Keys)
            {
                CommentObj c = comments[key];
                try
                {
                    Model.recipes[c.parentRecipe].AddComment(c);
                    Trace.WriteLine(string.Format("adding comment for user {0} to userObj {1}", c.userName, Model.users[c.userName].userName));
                    Model.users[c.userName].AddComment(c);
                }catch(KeyNotFoundException ex)
                {
                    Trace.WriteLine(ex.Message);
                    Trace.WriteLine(ex.StackTrace);
                }
            }
            //associate with recipes
        }

        public static void GetFavoritesFromFile()
        {
            Trace.WriteLine("opening favorites file at " + Constants.Paths_Data.GetFavoritesFilePath());
            StreamReader file = new StreamReader(Constants.Paths_Data.GetFavoritesFilePath());
            string line;
            file.ReadLine();
            while ((line = file.ReadLine()) != null)
            {
                String[] pair = line.Split('\t');
                Trace.WriteLine(string.Format("loading favorite {0} {1}", pair[0], pair[1]));
                AddFavorite(pair[0], pair[1]);
            }

        }

        public static void AddFavorite(string key, string value)
        {
            if (Model.favorites.ContainsKey(key))
                Model.favorites[key].Add(value);
            else
                Model.favorites.Add(key, new List<string> { value });
            Model.users[key].favorites.Add(value);
        }

        public static void RemoveFavorite(string key, string value)
        {
            if (!favorites.ContainsKey(key)) return;
            favorites[key].Remove(value);
            Model.users[key].favorites.Remove(value);
        }

        public static void WriteFavoritesToFile(Boolean append)
        {
            Trace.WriteLine("write favorites with append " + append);
            System.IO.StreamWriter file = new System.IO.StreamWriter(Constants.Paths_Data.GetFavoritesFilePath(), append);
            if (append)
                file.Write(string.Format("\n{0}\t{1}", Model.user.userName, Model.recipe.name ));
            else
            {
                file.Write("userName\trecipe");
                foreach(string user in Model.favorites.Keys)
                    foreach(string recipe in Model.favorites[user])
                        file.Write(string.Format("\n{0}\t{1}", user, recipe));
            }
            file.Close();

        }



        public static void WriteCommentsToFile(Boolean append)
        {
            Trace.WriteLine("write comments with append " + append);
            System.IO.StreamWriter file = new System.IO.StreamWriter(Constants.Paths_Data.GetCommentsFilePath(), append);
            if (append) {
                Trace.WriteLine(string.Format("looking for comment id {0}", Model.nextCommentId - 1));
                string[] comment = Model.comments[Model.comments.Count-1].ToStringArray();
                file.Write('\n');
                for (int i = 0;i<comment.Length;i++)
                {
                    file.Write(comment[i]);
                    if (i+1 < comment.Length)
                        file.Write('\t');
                }
//                file.Write(string.Format("\n{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", comment[0],comment[1],comment[2],comment[3],comment[4]));
            }
            else
            {
                file.Write("id\tdate_time\tedit_date_time\tdeleted\tuserName\trecipe\tcontent");
                for (int i = 0; i < Model.nextCommentId; i++)
                {
                    string[] comment = Model.comments[i].ToStringArray();
                    file.Write('\n');
                    for (int j = 0; j < comment.Length; j++)
                    {
                        file.Write(comment[j]);
                        if (j + 1 < comment.Length)
                            file.Write('\t');
                    }
                    //                  file.Write(string.Format("\n{0}\t{1}\t{2}\t{3}\t{4}", comment[0], comment[1], comment[2], comment[3], comment[4]));
                }
            }
            file.Close();

        }

        public void ConfigureUser()
        {
            if (user == null)
            {
                MainWindow._viewMain.UserName.Content = "Press to Sign In";
                MainWindow._viewRecipe.SignIn.Visibility = Visibility.Visible;
            }
            else
            {
                MainWindow._viewMain.SetUser(Model.user);
                MainWindow._viewUserAccount.SetUser(Model.user);
            }
        }

        public static void GetAccountsFromFile()
        {
            Trace.WriteLine("opening accounts file at " + Constants.Paths_Data.GetAccountsFilePath());
            StreamReader file = new StreamReader(Constants.Paths_Data.GetAccountsFilePath());
            string line;
            file.ReadLine();
            users = new Dictionary<string,UserObj>();
            while ((line = file.ReadLine()) != null)
            {
                String[] account = line.Split('\t');
                Trace.WriteLine(string.Format("adding account {0} with password {1} and icon {2}",account[0], account[1], account[2]));
//                users.Add(account[0], new UserObj(account[0], account[1], new BitmapImage(new Uri(Constants.Paths_Images.getImagesUserIconsPath() + account[2], UriKind.Absolute))));
                users.Add(account[0], new UserObj(account[0], account[1], account[2]));//delay source creation.
            }
        }
        public string[] GetUserNames()
        {
            return users.Keys.ToArray();
        }
    }
}
