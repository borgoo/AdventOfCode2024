using AdventOfCode2024.Utils;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Day7
{
    internal class Day7 : Day
    {
        private abstract class Operation {
            public abstract long Run(long a, long b);
        }

        private class Plus() : Operation()
        {
            public override long Run(long a, long b)
            {
                return a + b;
            }
        }

        private class Mul() : Operation()
        {
            public override long Run(long a, long b)
            {
                return a * b;
            }
        }
        private class Concat() : Operation()
        {
            public override long Run(long a, long b)
            {
                return Convert.ToInt64(a.ToString() + b.ToString());
            }
        }

        protected override object SolveA(string input)
        {
            Operation[] operations = [
                    new Plus(),
                    new Mul()
            ];
            long ans = 0;
            foreach (string row in input.Split("\r\n")) {
                ans += CanBeCalibrated(row, operations);
            }

            return ans;
        }

        protected override object SolveB(string input)
        {
            Operation[] operations = [
                   new Plus(),
                    new Mul(),
                    new Concat()
           ];
            long ans = 0;

            string[] rows = input.Split("\r\n");
            _loadingBar.Enabled = rows.Length > 200;

            int i = 0;
            foreach (string row in rows)
            {
                _loadingBar.Show(i, rows.Length);
                ans += CanBeCalibrated(row, operations);
                i++;
            }
            _loadingBar.Terminate();

            return ans;
        }

        private static long CanBeCalibrated(string row, Operation[] allowedOperations) {

            string[] content = row.Split(": ");
            long key = long.Parse(content[0]);
            string[] values = content[1].Split(' ');
            int numOfOp = values.Length - 1;
            int numOfAllowedOperations = allowedOperations.Length;


            int[] currVal = new int[numOfOp];
            double numOfCombinations = Math.Pow(numOfAllowedOperations, numOfOp);
            for (int i = 0; i <  numOfCombinations; i++)
            {

                long res = long.Parse(values[0]);
                for (int j = 0; j < currVal.Length; j++)
                {

                    int opIdx = currVal[j];
                    Operation op = allowedOperations[opIdx];
                    res = op.Run(res, long.Parse(values[j + 1]));
                }

                if (res == key)
                {
                    return key;
                }

                currVal = Increment(currVal, numOfAllowedOperations);

            }

            return 0;

        }


        private static int[] Increment(int[] arr, int numOfAllowedOperations)
        {

            int i = arr.Length - 1;
            int carry = 1;
            while (carry == 1 && i >= 0)
            {

                int sum = arr[i] + carry;
                if (sum > numOfAllowedOperations - 1)
                {
                    arr[i] = 0;
                    carry = 1;
                }
                else
                {
                    arr[i] = arr[i] + carry;
                    carry = 0;
                }
                i--;

            }


            return arr;
        }


    }
}
