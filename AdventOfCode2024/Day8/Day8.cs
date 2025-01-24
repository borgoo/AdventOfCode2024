using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AdventOfCode2024.Day;

namespace AdventOfCode2024.Day8
{
    internal class Day8 : Day
    {
        protected override long SolveA(string input)
        {
            var antinodes = GetAntinodes(input, 1);
            return antinodes.Count;
        }

        protected override long SolveB(string input)
        {
            var antinodes = GetAntinodes(input);            
            return antinodes.Count;
        }

        private static void DEBUG(string input, HashSet<(int X, int Y)> antinodes) {

            string[] rows = input.Split("\r\n");
            for (int i = 0; i < rows.Length; i++)
            {

                for (int j = 0; j < rows[i].Length; j++)
                {
                    char val = rows[i][j];
                    if (antinodes.Contains((X: i, Y: j))) Console.Write('#');
                    else Console.Write(val);
                }
                Console.WriteLine();
            }

        }

        private static HashSet<(int X, int Y)> GetAntinodes(string input, int? limitOfAntinodesPerAntenna = null) {

            string[] rows = input.Split("\r\n");
            Dictionary<char, List<(int X, int Y)>> hashMap = new();
            HashSet<(int X, int Y)> antinodes = new();

            int m = rows.Length;
            int n = rows[0].Length;

            for (int rowNum = 0; rowNum < m; rowNum++)
            {

                string row = rows[rowNum];
                for (int col = 0; col < row.Length; col++)
                {
                    char val = row[col];

                    if (val == '.') continue;

                    (int X, int Y) frequency = (X: rowNum, Y: col);

                    if (!hashMap.ContainsKey(val))
                    {
                        hashMap.Add(val, new());
                    }
                    AddAntinode(antinodes, hashMap, val, frequency, m, n, limitOfAntinodesPerAntenna);
                }
            }

            return antinodes;

        }

        private static void AddAntinode(HashSet<(int X, int Y)> antinodes, Dictionary<char, List<(int X, int Y)>> hashMap, char key, (int X, int Y) frequency, int m, int n, int? limitOfAntinodesPerAntenna = null)
        {
            int minIValue = limitOfAntinodesPerAntenna is null ? 0 : 1;

            foreach ((int X, int Y) other in hashMap[key])
            {

                int dY = frequency.Y - other.Y;
                int dX = frequency.X - other.X;

                for (int i = minIValue; limitOfAntinodesPerAntenna is null || i <= limitOfAntinodesPerAntenna; i++) {

                    (int X, int Y) antinodeA = (X: frequency.X + (dX * i), Y: frequency.Y + (dY * i));

                    if (antinodeA.X < 0 || antinodeA.X >= m || antinodeA.Y < 0 || antinodeA.Y >= n)
                        break;

                    antinodes.Add(antinodeA);
                }


                for (int i = minIValue; limitOfAntinodesPerAntenna is null || i <= limitOfAntinodesPerAntenna; i++)
                {

                    (int X, int Y) antinodeB = (X: other.X - (dX * i), Y: other.Y - (dY * i));

                    if (antinodeB.X < 0 || antinodeB.X >= m || antinodeB.Y < 0 || antinodeB.Y >= n)
                        break;

                    antinodes.Add(antinodeB);
                }

            }

            hashMap[key].Add(frequency);

        }


    }
}
