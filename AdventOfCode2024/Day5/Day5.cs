using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Day5
{
    internal class Day5 : Day
    {

        private static (string[] Rules, string[] Inputs) HandleInput(string input) {

            List<string> rules = new();
            List<string> inputs = new();

            string[] fileLines = input.Split("\r\n");
            int i = 0;
            while (fileLines[i] != String.Empty)
            {
                rules.Add(fileLines[i]);
                i++;
            }
            i++;
            while (i < fileLines.Length)
            {
                inputs.Add(fileLines[i]);
                i++;
            }

            return (Rules: [.. rules], Inputs: [.. inputs]);


        }
        
        protected override long SolveA(string input)
        {
            var (Rules, Inputs) = HandleInput(input);
            return ApplyRulesToInputs(Rules, Inputs).Valid;

        }

        protected override long SolveB(string input)
        {
            var (Rules, Inputs) = HandleInput(input);
            return ApplyRulesToInputs(Rules, Inputs).Fixed;

        }

        private static int ApplyRulesToInputs2(string[] rules, string[] inputs) {

            int ans = 0;
            Dictionary<int, HashSet<int>> mustBeBefore = new();

            foreach (string rule in rules)
            {
                string[] beforeThan = rule.Split('|');
                int key = Int32.Parse(beforeThan[0]);
                int val = Int32.Parse(beforeThan[1]);

                if (!mustBeBefore.ContainsKey(key)) mustBeBefore.Add(key, new HashSet<int>());
                mustBeBefore[key].Add(val);

            }

            foreach (string input in inputs)
            {

                string[] vals = input.Split(',');
                bool ok = true;
                for (int i = 0; i < vals.Length; i++)
                {

                    int key = Int32.Parse(vals[i]);
                    for (int j = 0; j < i; j++)
                    {

                        if (!mustBeBefore.ContainsKey(key)) continue;
                        if (mustBeBefore[key].Contains(Int32.Parse(vals[j])))
                        {
                            ok = false;
                            break;
                        }
                    }

                    if (!ok) break;

                }

                if (ok) ans += Int32.Parse(vals[vals.Length / 2]);



            }

            return ans;
        }


        private static (int Valid, int Fixed) ApplyRulesToInputs(string[] rules, string[] inputs)
        {

            int ansValid = 0;
            int ansFixed = 0;

            Dictionary<int, HashSet<int>> mustBeBefore = new();

            foreach (string rule in rules)
            {
                string[] beforeThan = rule.Split('|');
                int key = Int32.Parse(beforeThan[0]);
                int val = Int32.Parse(beforeThan[1]);

                if (!mustBeBefore.ContainsKey(key)) mustBeBefore.Add(key, new HashSet<int>());
                mustBeBefore[key].Add(val);

            }

            foreach (string input in inputs)
            {

                string[] vals = input.Split(',');
                LinkedList<string> flexibleVals = new();
                foreach (string val in vals) { 
                    flexibleVals.AddLast(val);
                }

                bool allOk = true;
                for (int i = 0; i < flexibleVals.Count; i++)
                {
                    string keyAsString = flexibleVals.ElementAt(i);
                    int key = Int32.Parse(keyAsString);
                    for (int j = 0; j < i; j++)
                    {

                        if (!mustBeBefore.ContainsKey(key)) continue;
                        string currentConsideredVal = flexibleVals.ElementAt(j);
                        if (mustBeBefore[key].Contains(Int32.Parse(currentConsideredVal)))
                        {
                            allOk = false;
                            flexibleVals.Remove(keyAsString);
                            LinkedListNode<string> toBeMoved = new(keyAsString);
                            LinkedListNode<string> moveHere = flexibleVals.Find(currentConsideredVal) ?? throw new NullReferenceException();
                            flexibleVals.AddBefore(moveHere, keyAsString);
                            i--;
                            break;
                        }
                    }

                }

                int middleVal = Int32.Parse(flexibleVals.ElementAt(flexibleVals.Count / 2));
                if (allOk) ansValid += middleVal;
                else ansFixed += middleVal;

            }

            return (Valid: ansValid, Fixed: ansFixed);
        }
         

    }
}
