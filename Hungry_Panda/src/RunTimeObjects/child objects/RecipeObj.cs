using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hungry_Panda
{
    public class RecipeObj
    {
        public string name;
        public string image;
        public string image_thumb;
        public List <String[]> steps;
        public List <String[]> ingredients;
        public int currentStep;
        public int totalSteps;
        public string ethnicity;
        public string difficulty;
        public string[] times;
        public List<CommentObj> comments;
        public List<string> favoritedUsers;
        public string[] Uids;
        public string description;

        public RecipeObj(StreamReader input)
        {
            comments = new List<CommentObj>();
            this.currentStep = 0;
            this.name = input.ReadLine().Replace("\\t", "\t").Replace("\\n", "\n");
//            input.ReadLine();//skip first line for now;
            image = input.ReadLine();
            image_thumb = input.ReadLine();
            ethnicity = input.ReadLine();
            difficulty = input.ReadLine();
            times = input.ReadLine().Split('\t');
            string line;
            description = input.ReadLine().Replace("\\t", "\t").Replace("\\n", "\n");
            int readSteps = totalSteps = int.Parse(input.ReadLine());
            Uids = new string[readSteps + 1];
            for (int i = 0; i < readSteps + 1; i++)
                Uids[i] = Guid.NewGuid().ToString();
            steps = new List<String[]>();
            string[] step;
            while ((line = input.ReadLine()) != null && readSteps-- > 0)
            {
                Trace.WriteLine(String.Format("{0},[{1},{2},{3}]", line, line.Split('\t')[0], line.Split('\t')[1], line.Split('\t')[2]).Replace("\\t", "\t").Replace("\\n", "\n"));
                step = line.Split('\t');
                step[2] = step[2].Replace("\\t", "\t").Replace("\\n", "\n");
                steps.Add(step);
            }
            ingredients = new List<String[]>();
            while (line != null)
            {
                Trace.WriteLine(String.Format("{0},[{1},{2},{3}]", line, line.Split('\t')[0], line.Split('\t')[1], line.Split('\t')[2]));
                ingredients.Add(line.Split('\t'));
                Trace.WriteLine(String.Format("input is eof? {0}", input.EndOfStream ? Boolean.TrueString : Boolean.FalseString));
                line = input.ReadLine();
            }
        }

        public string[] GetStep()
        {
            return steps[(int)currentStep];
        }

        public string[] GetNextStep()
        {
            if (currentStep + 1 < steps.Count())
                return steps[(int)++currentStep];
            return null;
        }

        public string getUid()
        {
            return Uids[currentStep];
        }

        public void AddComment(CommentObj c)
        {
            comments.Add(c);
        }
    }
}
