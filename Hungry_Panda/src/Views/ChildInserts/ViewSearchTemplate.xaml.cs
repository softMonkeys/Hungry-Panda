using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for ViewSearchTemplate.xaml
    /// </summary>
    public partial class ViewSearchTemplate : UserControl
    {
        //TODO: determine a more flexible means of setting the search combo boxes
        ObservableCollection<string> ethnicities = new ObservableCollection<string>();
        ObservableCollection<string> difficuties = new ObservableCollection<string>();
        ObservableCollection<string> allergies = new ObservableCollection<string>();

        string chosenEthnicity;
        string chosenDifficulty;
        string chosenAllergy;

        public double transform_y = 0;

        public ViewSearchTemplate()
        {
            InitializeComponent();
            ConfigComboBoxes();
            Uid = Guid.NewGuid().ToString();
        }
        void ConfigComboBoxes()
        {
            string[] eth = new string[] { "All", "Chinese", "Korean", "Japanese", "Thai" };
            string[] dif = new string[] { "All", "Easy", "Medium", "Hard" };
            string[] alrg = new string[] { "None", "Egg", "Milk", "Nut", "Peanut","Soy", "Wheat" };
            foreach (string e in eth)
                ethnicities.Add(e);
            foreach (string d in dif)
                difficuties.Add(d);
            foreach (string a in alrg)
                allergies.Add(a);
            ComboEthnicityBox.ItemsSource = ethnicities;
            ComboDifficultyBox.ItemsSource = difficuties;
            ComboAllergyBox.ItemsSource = allergies;
        }
        public void FakedSearchResults()
        {
            string[] ethnicities = { "Chinese", "Thai", "Japanese", "Korean" };
            string[] difficulties = { "easy", "medium", "hard" };
            MainWindow._viewHome.recipeList.Children.Clear();
            for (int i = 40; i < 45; i++)
            {
                string name = string.Format("recipe {0}", i);
                string thumbnail = Constants.Paths_Images.getImagesRecipesBasePath()+"KungPao_Chicken\\kung-pao-chicken-thumb.jpg";
                string difficulty = difficulties[i % 3];
                string ethnicity = ethnicities[i % 4];
                MainWindow._viewHome.recipeList.Children.Add(new ViewRecipeListElement(name, thumbnail, difficulty, ethnicity, i));
            }
            MainWindow._viewHome.paneLabel.Content = "Search Results";
        }

        public void clearResults()
        {
            chosenEthnicity = "";
            chosenDifficulty = "";
            chosenAllergy = "";
            ComboAllergyBox.SelectedIndex = 0;
            ComboDifficultyBox.SelectedIndex = 0;
            ComboEthnicityBox.SelectedIndex = 0;
        }

        List<String>[] ParseKeywords(){
            List<string> pro = new List<string>();
            List<string> must = new List<string>();
            List<string> con = new List<string>();
            string keywords = keywordBox.Text;
            Trace.WriteLine(string.Format("search keyword text is >{0}< and is null={1}", keywords, keywords == null));
            if (keywords.Length > 0)
            {
                foreach (string word in keywords.Split(' '))
                {
                    Trace.WriteLine(string.Format("parsing kwyword {0} with first char {1} == '-' {2}, .equals('-') {3}, .equals(\"-\") {4}", word, word[0], word[0] == '-', word[0].Equals('-'), word[0].Equals("-")));
                    if (word[0] == '-')
                        con.Add(word.Remove(0, 1));
                    else if (word[0] == '+')
                    {
                        must.Add(word.Remove(0, 1));
                        pro.Add(word.Remove(0, 1));//to get them to filter in so I can cross check later
                    }
                    else
                        pro.Add(word);
                }
            }
            return new List<string>[3] { pro, must, con };
        }

        void KeywordProInclude(List<string> pro, List<RecipeObj> results)
        {
            bool added = false;
            if (pro.Count > 0)
            {
                foreach (string recipeName in Model.recipes.Keys)
                {
                    RecipeObj r = Model.recipes[recipeName];
                    foreach (string word in pro)
                    {//need to match all pro to pass recipe to results
                        if (WordInRecipe(r, word) && !results.Contains(r) && !added)
                        {
                            results.Add(r);
                            Trace.WriteLine(string.Format("adding recipe {0} because it contains {1} and results.contains() is {2}", r.name,word,results.Contains(r)));
                            added = true;
                        }
                        added = false;
                    }
                }
            }
            else
                foreach (string name in Model.recipes.Keys)
                    results.Add(Model.recipes[name]);
        }
        void KeywordConRemove(List<string> con, List<RecipeObj> results)
        {
            //eliminate those that are being excluded
            foreach (RecipeObj r in results.ToArray())
                foreach (string word in con)
                    if (WordInRecipe(r, word))
                    {
                        results.Remove(r);
                        break;
                    }
        }
        void KeywordMustRemove(List<string> must, List<RecipeObj> results)
        {
            foreach (RecipeObj r in results.ToArray())
                foreach (string word in must)
                    if (!WordInRecipe(r, word))
                    {
                        Trace.WriteLine(string.Format("removing element {0} because must value {1} not found", r.name, word));
                        results.Remove(r);
                        break;
                    }
        }

        void RemoveEthnicity(List<RecipeObj> results)
        {
            if (chosenEthnicity.Length == 0) return;
            foreach (RecipeObj r in results.ToArray())
                if (!r.ethnicity.ToUpper().Equals(chosenEthnicity))
                {
                    Trace.WriteLine(string.Format("removing element {0} because Ethnicity value {1} is not {2}", r.name, r.ethnicity.ToUpper(), chosenEthnicity));
                    Trace.WriteLine(string.Format("removing element {0} because ethnicity value {1} not found", r.name, chosenEthnicity));
                    results.Remove(r);
                }
        }
        void RemoveDifficulty(List<RecipeObj> results)
        {
            if (chosenDifficulty.Length == 0) return;
            foreach (RecipeObj r in results.ToArray())
                if (!r.difficulty.ToUpper().Equals(chosenDifficulty))
                {
                    Trace.WriteLine(string.Format("removing element {0} because Difficulty value {1} is not {2}", r.name, r.difficulty.ToUpper(),chosenDifficulty));
                    results.Remove(r);
                }
        }
        void RemoveAllergy(List<RecipeObj> results)
        {
            Trace.WriteLine(string.Format("trimming results for allergy {0}", chosenAllergy));
            if (chosenAllergy.Length == 0) return;
            foreach (RecipeObj r in results.ToArray()) {
                Trace.WriteLine(string.Format("testing recipe {0} for contains allergen {1}", r.name, chosenAllergy));
                if (WordInRecipe(r, chosenAllergy))
                {
                    Trace.WriteLine(string.Format("removing element {0} because Allergy value {1} found", r.name, chosenAllergy));
                    results.Remove(r);
                }
            }
        }

        void SearchResults()
        {
            List<RecipeObj> results = new List<RecipeObj>();
            List<string>[] keywords = ParseKeywords();
            List<string> pro = keywords[0],
                         must = keywords[1],
                         con = keywords[2];

            KeywordProInclude(pro, results);
            KeywordConRemove(con, results);
            KeywordMustRemove(must,results);
            RemoveEthnicity(results);
            RemoveDifficulty(results);
            RemoveAllergy(results);
            //eliminate those results that do not match ALL fields in must
            MainWindow._viewHome.recipeList.Children.Clear();
            MainWindow._viewHome.noRecipe("", false);
            if (results.Count > 0)
                foreach (RecipeObj r in results)
                    MainWindow._viewHome.recipeList.Children.Add(new ViewRecipeListElement(r));
            else
                MainWindow._viewHome.noRecipe("No Recipes Match Given Query", true);
        }

        Boolean WordInRecipe(RecipeObj r, string word)
        {
            //get fully upercase variants of all relevant text fields
            string NAME = r.name.ToUpper();
            string ETH = r.ethnicity.ToUpper();
            string DIFF = r.difficulty.ToUpper();
            List<string> INGRED = new List<string>();
            foreach (string[] i in r.ingredients)
                INGRED.Add(i[1].ToUpper());
            string WORD = word.ToUpper();

            //test keyword matching now;
            if (NAME.Contains(WORD))
            {
                Trace.WriteLine(string.Format("recipe {0} contains {1} in name", r.name, WORD));
                return true;
            }
            if (ETH.Contains(WORD))
            {
                Trace.WriteLine(string.Format("recipe {0} contains {1} in ethnicity", r.name, WORD));
                return true;
            }
            if (DIFF.Contains(WORD))
            {
                Trace.WriteLine(string.Format("recipe {0} contains {1} in difficulty", r.name, WORD));
                return true;
            }
            foreach (string I in INGRED)
            {
                Trace.WriteLine(string.Format("testing ingredient {0} or .contains() {1}", I, WORD));
                if (I.Contains(WORD))
                {
                    Trace.WriteLine(string.Format("recipe {0} contains {1} in ingredients", r.name, WORD));
                    return true;
                }
            }
            return false;
        }


        public void Btn_search(object sender, RoutedEventArgs e)
        {
            SearchResults();
            MainWindow._viewHome.paneLabel.Content = "Search Results";
            Switcher.Switch(MainWindow.Views.results);
        }
        public void SearchEnterHit(object sender, RoutedEventArgs e)
        {
        }

        private void ComboAllergies_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ControlTemplate ct = this.ComboAllergyBox.Template;
            Popup pop = ct.FindName("PART_Popup", this.ComboAllergyBox) as Popup;
            pop.Placement = PlacementMode.Top;
        }

        private void setEthn(object sender, RoutedEventArgs e)
        {
            ComboBox selected = sender as ComboBox;
            if (selected.SelectedIndex > -0)
                chosenEthnicity = selected.SelectedItem.ToString().ToUpper();
            else if (selected.SelectedIndex > -1)
                chosenEthnicity = "";
        }
        private void setDiff(object sender, RoutedEventArgs e)
        {
            ComboBox selected = sender as ComboBox;
            if (selected.SelectedIndex > -0)
                chosenDifficulty= selected.SelectedItem.ToString().ToUpper();
            else if (selected.SelectedIndex > -1)
                chosenDifficulty = "";
        }
        private void setAlrg(object sender, RoutedEventArgs e)
        {
            ComboBox selected = sender as ComboBox;
            if (selected.SelectedIndex > -0)
                chosenAllergy = selected.SelectedItem.ToString().ToUpper();
            else if (selected.SelectedIndex > -1)
                chosenAllergy = "";
        }

    }
}
