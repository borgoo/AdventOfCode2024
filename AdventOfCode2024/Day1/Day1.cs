using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Day1
{
    internal class Day1 : Day
    {
        protected override object SolveA(string input)
        {
            int ans = 0;
            string[] rows = input.Split(new string("\r\n"));
            List<int> left = new();
            List<int> right = new();

            foreach (string row in rows)
            {
                string[] pair = row.Split(new string("   "));
                left.Add(Convert.ToInt32(pair[0]));
                right.Add(Convert.ToInt32(pair[1]));
            }
            left.Sort();
            right.Sort();

            for (int i = 0; i < left.Count; i++)
            {
                ans += Math.Abs(left.ElementAt(i) - right.ElementAt(i));
            }

            return ans;
        }

        protected override object SolveB(string input)
        {
            int ans = 0;
            string[] rows = input.Split(new string("\r\n"));
            List<int> left = new();
            Dictionary<int, int> right = new();


            foreach (string row in rows)
            {
                string[] pair = row.Split(new string("   "));

                left.Add(Convert.ToInt32(pair[0]));
                if (!right.ContainsKey(Convert.ToInt32(pair[1]))) {
                    right.Add(Convert.ToInt32(pair[1]), 0);
                }
                right[Convert.ToInt32(pair[1])]++;
            }

            foreach (int leftVal in left) {
                
                if (!right.ContainsKey(leftVal)) continue;
                ans += right[leftVal] * leftVal;

            }
                       

            return ans;
        }

    }
}
