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

        private class NotFoundExitException((int X, int Y)? node = null) : Exception()
        {
            public (int X, int Y)? BlockingBytePosition { get; private set; } = node;

            public override string Message
            {
                get
                {
                    if(BlockingBytePosition is null) return base.Message;
                    return $"{BlockingBytePosition.Value.X},{BlockingBytePosition.Value.Y}";
                }
            }
        }

        protected override object SolveA(string input)
        {
            (int X, int Y) startingPosition = (0, 0);
            var (m, n, bytesPositions) = HandleInput(input);
            (int X, int Y) exitPosition = (m-1, n-1);

            return Bfs(m, n, bytesPositions, startingPosition, exitPosition);


        }

        protected override object SolveB(string input)
        {
            (int X, int Y) startingPosition = (0, 0);
            var (m, n, bytesPositions) = HandleInput(input, true);
            (int X, int Y) exitPosition = (m - 1, n - 1);

            try
            {
                return Bfs(m, n, bytesPositions, startingPosition, exitPosition); ;
            }
            catch (NotFoundExitException nfeEx) {
                return nfeEx.Message;
            }

        }
        private static (int m, int n, HashSet<(int X, int Y)> bytesPositions) HandleInput(string input, bool ignoreBytesLimit = false) {

            string[] data = input.Split("\r\n");

            int maxX = 0;
            int maxY = 0;
            HashSet<(int X, int Y)> bytesPositions = [];
            int bytesNum = Convert.ToInt32(data[0].Replace(" bytes", String.Empty));
            for (int i = 1; i < data.Length; i++) { 
                string[] pair = data[i].Split(',');
                int x = Convert.ToInt32(pair[0]);
                int y = Convert.ToInt32(pair[1]);
                if (x == 0 && y == 0) throw new NotImplementedException("(0,0) cannot be inaccessible.");
                if (x > maxX) maxX = x;
                if (y > maxY) maxY = y;
                bytesPositions.Add((x, y));
                if (!ignoreBytesLimit && bytesPositions.Count >= bytesNum) return (maxX+1, maxY+1, bytesPositions);
            }

            return (maxX+1, maxY+1, bytesPositions);
           
            
        }


        private static int Bfs(int m, int n, HashSet<(int X, int Y)> bytesPositions, (int X, int Y) startingPosition, (int X, int Y) exitPosition) {

            HashSet<(int X, int Y)> seen = [];

            Queue<(int X, int Y)> nodes = [];
            nodes.Enqueue(startingPosition);
            seen.Add(startingPosition);

            (int X, int Y)? lastUnreachableNode = null;

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
                        if (bytesPositions.Contains(siblingNode)) {
                            lastUnreachableNode = siblingNode;
                            continue;
                        };
                        if(seen.Contains(siblingNode)) continue;
                        if (siblingNode == exitPosition) return depth;

                        nodes.Enqueue(siblingNode);
                        seen.Add(siblingNode);
                    }


                }

            }

            throw new NotFoundExitException(lastUnreachableNode);


        
        }

    }
}
