using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Hungry_Panda
{
    public static class Constants
    {
        static string root;
        public static string GetRootFolder() { return root; }
        public static void SetRootFolder(string root) { Constants.root = root; }
        public static class Paths_Images
        {
            const string Ingredients = "imgs\\Ingredients\\";
            const string Recipes = "imgs\\Recipes\\";
            const string UI = "imgs\\UI\\";
            const string UserIcons = "imgs\\userIcons\\";
            public static string getImagesIngredientsPath() { return root + Ingredients; }
            public static string getImagesRecipesBasePath() {return root + Recipes; }
            public static string getImagesUIPath() { return root + UI; }
            public static string getImagesUserIconsPath() { return root + UserIcons; }
        }
        public static class Paths_Data
        {
            const string Database = "database\\";
            const string Recipes = "Recipes\\";
            const string accountsFile = Database + "accounts";
            const string recipeFile = "\\recipe";
            const string commentsFile = Database+"comments";
            const string favoritesFile = Database + "favorites";
            public static string GetDatabaseFolderPath() { return root + Database; }
            public static string GetDatabaseRecipeFolderPath() { return root + Recipes; }
            public static string GetDatabaseRecipeFilePath(string recipe) { return root + Recipes + recipe + recipeFile; }
            public static string GetAccountsFilePath() { return root + accountsFile; }
            public static string GetCommentsFilePath() { return root + commentsFile; }
            public static string GetFavoritesFilePath() { return root + favoritesFile; }
        }

        public static ImageSource PathToSource(string path)
        {
            Trace.WriteLine(string.Format("converting to path: {0}", path));
            return new BitmapImage(new Uri(path, UriKind.Absolute));
        }
    }
}
