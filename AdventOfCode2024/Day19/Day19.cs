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
                if (FeasibilityCheck(design, availableTowels, notMakableDesigns)) c++;
            }       
            

            return c;
        }

        protected override object SolveB(string input)
        {
            var (availableTowels, toDoCombinations) = HandleInput(input);

            HashSet<string> notMakableDesigns = [];
            Dictionary<string, long> cache = [];
            long count = 0;
            foreach (string design in toDoCombinations)
            {
                CombinationsCounter(design, availableTowels, 0, cache);
                count += cache.ContainsKey(design) ? cache[design] : 0;
            }


            return count;
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

        /// <summary>
        /// DP Top-Down with memoization
        /// </summary>
        /// <param name="originalDesign"></param>
        /// <param name="availableTowels"></param>
        /// <param name="notMakableDesigns"></param>
        /// <param name="designIndex"></param>
        /// <returns></returns>
        private static bool FeasibilityCheck(string originalDesign, HashSet<string> availableTowels, HashSet<string> notMakableDesigns, int designIndex = 0 ) {

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
                bool ok = FeasibilityCheck(originalDesign, availableTowels, notMakableDesigns, i+1);
                if (ok) return true;

            }

            notMakableDesigns.Add(originalDesign[designIndex..].ToString());
            return false;        
        
        }

       
        /// <summary>
        /// DP Top-Down with memoization
        /// </summary>
        /// <param name="originalDesign"></param>
        /// <param name="availableTowels"></param>
        /// <param name="designIndex"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        private static long CombinationsCounter(
           string originalDesign,
           HashSet<string> availableTowels,
           int designIndex,
           Dictionary<string, long> cache)
        {

            string currStringToElaborate = originalDesign[designIndex..].ToString();
            if (cache.ContainsKey(currStringToElaborate)) return cache[currStringToElaborate];

            StringBuilder sb = new();
            for (int i = designIndex; i < originalDesign.Length; i++)
            {

                sb.Append(originalDesign[i]);
                string currWindow = sb.ToString();
                if (!availableTowels.Contains(currWindow)) continue;
                if (i == originalDesign.Length - 1)
                {
                    cache.TryAdd(currStringToElaborate, 0);
                    cache[currStringToElaborate] += 1;
                    return cache[currStringToElaborate];
                }
                long res = CombinationsCounter(originalDesign, availableTowels, i + 1, cache);
                cache.TryAdd(currStringToElaborate, 0);
                cache[currStringToElaborate] += res;
               
            }


            return cache.ContainsKey(currStringToElaborate) ? cache[currStringToElaborate] : 0;

        }






    }
}
