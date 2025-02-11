using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Day19
{
    internal class Day19 : Day
    {
        protected override object SolveA(string input)
        {
            var (availableTowels, toDoCombinations) = HandleInput(input);

            HashSet<string> notMakableDesigns = [];
            int c = 0;
            foreach (string design in toDoCombinations) {
                if (Dp(design, availableTowels, notMakableDesigns)) c++;
            }       
            

            return c;
        }

        protected override object SolveB(string input)
        {
            throw new NotImplementedException();
        }

      
        private static (HashSet<string> availableTowels, string[] toDoCombinations) HandleInput(string input)
        {
            string[] data = input.Split("\r\n", 2);
            string toDoStr = data[1][2..];
            string[] availableTowelsArr = data[0].Split(", ");
            string[] toDoCombinations = toDoStr.Split("\r\n");

            HashSet<string> availableTowels = [];
            foreach (string str in availableTowelsArr) { 
                availableTowels.Add(str);
            }

            return (availableTowels, toDoCombinations);           

        }

        private static bool Dp(string originalDesign, HashSet<string> availableTowels, HashSet<string> notMakableDesigns, int designIndex = 0 ) {

            if (notMakableDesigns.Contains(originalDesign[designIndex..].ToString()) ) return false;
            if (availableTowels.Contains(originalDesign)) return true;
            
            StringBuilder sb = new();
            for (int i = designIndex; i < originalDesign.Length; i++) { 
            
                sb.Append(originalDesign[i]);
                string currWindow = sb.ToString();
                if (!availableTowels.Contains(currWindow)) continue;
                //update seen
                if (designIndex > 0) {
                    string craftableStr = originalDesign[..(i+1)].ToString();
                    availableTowels.Add(craftableStr);
                }
                bool ok = Dp(originalDesign, availableTowels, notMakableDesigns, i+1);
                if (ok) return true;

            }

            notMakableDesigns.Add(originalDesign[designIndex..].ToString());
            return false;        
        
        }





        
       
    }
}
