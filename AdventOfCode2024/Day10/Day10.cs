using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Day10
{
    internal class Day11 : Day
    {
        const int START_VAL = 0;
        const int END_VAL = 9;

        protected override object SolveA(string input)
        {
            return TraverseHiking(input);
        }

        protected override object SolveB(string input)
        {
            return TraverseHiking(input, START_VAL, END_VAL, false);
        }

        private static int Bfs((int X, int Y) start, string[] rows, int m, int n, int end, bool distinctOnly)
        {

            int ans = 0;
            Queue<(int X, int Y)> nodes = new();
            nodes.Enqueue(start);
            HashSet<(int X, int Y)> endings = new();

            while (nodes.Count > 0)
            {

                int numOfParents = nodes.Count;

                for (int i = 0; i < numOfParents; i++)
                {

                    (int X, int Y) currNode = nodes.Dequeue();

                    int currNodeVal = rows[currNode.X][currNode.Y] - '0';

                    if (currNodeVal == end)
                    {

                        if (distinctOnly)
                        {

                            if (!endings.Contains(currNode))
                            {
                                endings.Add(currNode);
                                ans++;
                            }

                        }
                        else {
                            ans++;
                        }
                        continue;

                    }


                    if (currNode.X > 0)
                    {
                        (int X, int Y) neighbor = (X: currNode.X - 1, Y: currNode.Y);
                        int neighborVal = rows[neighbor.X][neighbor.Y] - '0';
                        if (neighborVal - currNodeVal == 1) nodes.Enqueue(neighbor);
                    }
                    if (currNode.X < m - 1)
                    {
                        (int X, int Y) neighbor = (X: currNode.X + 1, Y: currNode.Y);
                        int neighborVal = rows[neighbor.X][neighbor.Y] - '0';
                        if (neighborVal - currNodeVal == 1) nodes.Enqueue(neighbor);
                    }
                    if (currNode.Y > 0)
                    {
                        (int X, int Y) neighbor = (X: currNode.X, Y: currNode.Y - 1);
                        int neighborVal = rows[neighbor.X][neighbor.Y] - '0';
                        if (neighborVal - currNodeVal == 1) nodes.Enqueue(neighbor);
                    }
                    if (currNode.Y < n - 1)
                    {
                        (int X, int Y) neighbor = (X: currNode.X, Y: currNode.Y + 1);
                        int neighborVal = rows[neighbor.X][neighbor.Y] - '0';
                        if (neighborVal - currNodeVal == 1) nodes.Enqueue(neighbor);
                    }
                }
            }

            return ans;
        }

        internal static int TraverseHiking(string input, int startVal = START_VAL, int endVal = END_VAL, bool distinctOnly = true)
        {
            int ans = 0;

            string[] rows = input.Split("\r\n");
            int m = rows.Length;
            int n = rows[0].Length;


            for (int x = 0; x < m; x++)
            {

                for (int y = 0; y < n; y++)
                {
                    if (rows[x][y] - '0' == startVal)
                    {
                        (int X, int Y) startingPoint = (X: x, Y: y);
                        ans += Bfs(startingPoint, rows, m, n, endVal, distinctOnly);
                    }
                }
            }





            return ans;


        }

        
    }
}
