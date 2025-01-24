using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Day2
{
    internal class Day2 : Day
    {
        private const int MIN_DELTA = 1;
        private const int MAX_DELTA = 3;

        protected override long SolveA(string input)
        {
            return CountSafeLevels(input);
        }

        protected override long SolveB(string input)
        {
            return CountSafeLevels(input, 1);
        }

        private static int CountSafeLevels(string input, int tollerance = 0) {

            int ans = 0;
            string[] rows = input.Split("\r\n");

            foreach (string row in rows) {

                string[] tmp = row.Split(" ");
                int[] levels = new int[tmp.Length];

                for (int i = 0; i < tmp.Length; i++)
                {
                    levels[i] = Convert.ToInt32(tmp[i]);               
                }

                if (IsSafeLevel(levels, tollerance)) ans++;
            }

            return ans;

          
        }


        private static bool IsSafeLevel(int[] levels, int tollerance = 0) {
            
            int n = levels.Length;
            int minListLength = n - tollerance;
            int mask = (1 << n) - 1;
            int maskMinValue = (1 << n - minListLength) - 1;

            while (mask >= maskMinValue) {

                int currSubArrayLength = CountOnes(mask);

                if (currSubArrayLength < minListLength) {
                    mask--;
                    continue;
                }

                bool res = CheckGeneratedLevel(levels, mask, currSubArrayLength);
                if (res) return true;



                mask--;

            }

            return false;



        }

        private static int CountOnes(int maskVal) {

            int count = 0;
            while (maskVal > 0) {
                count += (maskVal & 1);
                maskVal >>= 1;  
            }
            return count;
        
        }


        internal static bool CheckGeneratedLevel(int[] levels, int mask, int currSubArrayLength)
        {


            int[] vals = new int[currSubArrayLength];
            for (int i = 0, j = 0; i < levels.Length; i++) {

                if ((mask & (1 << i)) != 0){                
                    vals[j] = levels[i];
                    j++;
                }
            }

            int prev = Convert.ToInt32(vals[0]);

            bool shouldBeAsc = Convert.ToInt32(vals[1]) > prev;

            bool ok = true;
            for (int i = 1; i < vals.Length; i++)
            {

                int curr = Convert.ToInt32(vals[i]);

                if (shouldBeAsc)
                {

                    if (curr <= prev || curr - prev <= (MIN_DELTA - 1 ) || curr - prev >= (MAX_DELTA + 1))
                    {
                        ok = false;
                        break;
                    }
                }
                else
                {
                    if (curr >= prev || prev - curr <= (MIN_DELTA - 1) || prev - curr >= (MAX_DELTA + 1))
                    {
                        ok = false;
                        break;
                    }
                }
                prev = curr;
            }
            if (ok) return true;

            return false;


        }
    }
}
