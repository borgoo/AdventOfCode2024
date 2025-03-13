using AdventOfCode2024.Exceptions;
using AdventOfCode2024.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Day18
{
    internal class Day18 : Day
    {

        private static readonly (int Dx, int Dy)[] directions = {
            (-1,0),
            (0,1),
            (1,0),
            (0,-1)
        };

        private class NotFoundExitException(string msg ="Exit is unreachable.") : Exception(msg)
        { 
        }

        protected override object SolveA(string input)
        {
            (int X, int Y) startingPosition = (0, 0);
            var (m, n, wallHistory) = HandleInput(input);
            (int X, int Y) exitPosition = (m-1, n-1);

            return Bfs(m, n, wallHistory, startingPosition, exitPosition);


        }

        private static class BruteForce {

            private static readonly LoadingBar loadingBar = new();
            private static (int m, int n, Queue<(int X, int Y)> walls) HandleInput(string input)
            {

                string[] data = input.Split("\r\n");

                int maxX = 0;
                int maxY = 0;
                Queue<(int X, int Y)> walls = [];
                int skipByteLimits = 1;
                for (int i = skipByteLimits; i < data.Length; i++)
                {
                    string[] pair = data[i].Split(',');
                    int x = Convert.ToInt32(pair[0]);
                    int y = Convert.ToInt32(pair[1]);
                    if (x == 0 && y == 0) throw new NotHandledException("(0,0) cannot be inaccessible.");
                    if (x > maxX) maxX = x;
                    if (y > maxY) maxY = y;
                    walls.Enqueue((x, y));
                }

                return (maxX + 1, maxY + 1, walls);

            }

            public static string SolveB(string input)
            {
                var (m, n, walls) = HandleInput(input);
                (int X, int Y) startingPosition = (0, 0);
                (int X, int Y) exitPosition = (m - 1, n - 1);

                int numOfWalls = walls.Count;
                bool showLoadingBar = numOfWalls > 100;
                loadingBar.Enabled = showLoadingBar;
                HashSet <(int X, int Y)> currentWalls = [];

                while (walls.Count > 0) {

                    loadingBar.Show(currentWalls.Count, numOfWalls);
                    var addedWall = walls.Dequeue();
                    currentWalls.Add(addedWall);                   

                    try
                    {
                        Bfs(m, n, currentWalls, startingPosition, exitPosition);
                        continue;
                    }
                    catch (NotFoundExitException)
                    {
                        loadingBar.Terminate();
                        return $"{addedWall.X},{addedWall.Y}";
                    }


                }

                loadingBar.Terminate();
                throw new Exception("Exit always reachable.");
               

            }


        }

        protected override object SolveB(string input)
        {
            return BruteForce.SolveB(input);

        }
        private static (int m, int n, HashSet<(int X, int Y)> walls) HandleInput(string input) {

            string[] data = input.Split("\r\n");

            int maxX = 0;
            int maxY = 0;
            HashSet<(int X, int Y)> walls = [];
            int bytesNum = Convert.ToInt32(data[0].Replace(" bytes", String.Empty));
            for (int i = 1; i < data.Length; i++) { 
                string[] pair = data[i].Split(',');
                int x = Convert.ToInt32(pair[0]);
                int y = Convert.ToInt32(pair[1]);
                if (x == 0 && y == 0) throw new NotHandledException("(0,0) cannot be inaccessible.");
                if (x > maxX) maxX = x;
                if (y > maxY) maxY = y;
                walls.Add((x, y));
                if (walls.Count >= bytesNum) return (maxX+1, maxY+1, walls);
            }

            return (maxX+1, maxY+1, walls);
           
            
        }


        private static int Bfs(int m, int n, HashSet<(int X, int Y)> walls, (int X, int Y) startingPosition, (int X, int Y) exitPosition) {

            HashSet<(int X, int Y)> seen = [];

            Queue<(int X, int Y)> nodes = [];
            nodes.Enqueue(startingPosition);
            seen.Add(startingPosition);

            int depth = 0;
            while (nodes.Count != 0)
            {
                depth++;
                int numOfNodes = nodes.Count;
                for (int i = 0; i < numOfNodes; i++) {

                    (int X, int Y) currentNode = nodes.Dequeue();

                    foreach (var (Dx, Dy) in directions) { 
                    
                        (int X , int Y) siblingNode = (currentNode.X + Dx, currentNode.Y + Dy);
                        if (siblingNode.X < 0 || siblingNode.X >= m) continue;
                        if (siblingNode.Y < 0 || siblingNode.Y >= n) continue;
                        if (walls.Contains(siblingNode)) continue;
                        if (seen.Contains(siblingNode)) continue;
                        if (siblingNode == exitPosition) return depth;

                        nodes.Enqueue(siblingNode);
                        seen.Add(siblingNode);
                    }


                }

            }

            throw new NotFoundExitException();


        
        }

    }
}
