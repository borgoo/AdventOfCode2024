using AdventOfCode2024.Exceptions;
using AdventOfCode2024.Utils;

namespace AdventOfCode2024.Day20
{
    internal class Day20 : Day
    {

        private static readonly bool _debugActive = false;

        private const int MIN_NUM_OF_PICOSECONDS_TO_SAVE = 100;
        private const char START_CHAR = 'S';
        private const char WALL_CHAR = '#';
        private const char END_CHAR = 'E';
        private static readonly (int Dx, int Dy)[] directions = [
            
            (-1,0),
            (1,0),
            (0,1),
            (0,-1)
        ];

        private readonly static (int Dx, int Dy)[] teleports = [
            (-2,0),
            (-1,+1),
            (0,+2),
            (+1,+1),
            (+2,0),
            (+1,-1),
            (0,-2),
            (-1,-1)          
          ];


        protected override object SolveA(string input)
        {
            var (matrix, startingPosition, endPosition) = HandleInput(input);
            Dictionary<(int X, int Y), NodeData> graph = [];
            int best = Bfs(matrix, startingPosition, endPosition, graph);
            HashSet<(int X, int Y)> bestPathOnlyNodes = GetBestPathNodes(graph, endPosition);
            int[] results = new int[best];
            CheatSomePath(matrix, startingPosition, endPosition, graph, bestPathOnlyNodes, results);

            for (int i = best - 1; _debugActive && i >= 0; i--)
            {
                if (results[i] == 0) continue;
                Console.WriteLine($"There are {results[i]} cheats that save {best - i} picoseconds.");

            }

            int minNumOfPicosecondsToSave = MIN_NUM_OF_PICOSECONDS_TO_SAVE;
            int ans = 0;
            for (int i = 0;i<best; i++)
            {
                if (results[i] == 0) continue;
                if (best - i < minNumOfPicosecondsToSave) break;
                ans += results[i];

            }


            return ans;

        }

        protected override object SolveB(string input)
        {
            throw new NotImplementedException();
        }

        private static (char[,] matrix, (int X, int Y) startPosition, (int X, int Y) endPosition) HandleInput(string input)
        {
            char[,] matrix = MatrixUtils.StringToCharMatrix(input);
            int col = matrix.GetLength(1);

            int startIndex = input.IndexOf(START_CHAR);
            int startX = (startIndex) / (col + 2);
            int startY = (startIndex) % (col + 2) ;

            int endIndex = input.IndexOf(END_CHAR);
            int endX = (endIndex) / (col + 2);
            int endY = (endIndex) % (col + 2);

            return (matrix, (startX, startY), (endX, endY));


        }


        private static void CheatSomePath(
            char[,] matrix,
            (int X, int Y) startPosition,
            (int X, int Y) endPosition,
            Dictionary<(int X, int Y), NodeData> graph,
            HashSet<(int X, int Y)> bestPathOnlyNodes,
            int[] solutions
            
        )
        {

            int numOfRows = matrix.GetLength(0);
            int numofColumns = matrix.GetLength(1);
            HashSet<(int X, int Y)> seen = [];
            Queue<(int X, int Y)> nodes = [];

            nodes.Enqueue(startPosition);
            seen.Add(startPosition);

            int depth = 1;
            while (nodes.Count > 0)
            {
                if (depth >= solutions.Length) return;

                int num = nodes.Count;
                for (int i = 0; i < num; i++)
                {

                    (int X, int Y) currNode = nodes.Dequeue();

                    foreach (var (Dx, Dy) in directions)
                    {

                        (int X, int Y) siblingNode = (currNode.X + Dx, currNode.Y + Dy);
                        if (siblingNode.X < 0 || siblingNode.X >= numOfRows) continue;
                        if (siblingNode.Y < 0 || siblingNode.Y >= numofColumns) continue;
                        if (matrix[siblingNode.X, siblingNode.Y] == WALL_CHAR) continue;
                        if (seen.Contains(siblingNode)) continue;
                        if (graph[siblingNode].Depth < graph[(currNode.X, currNode.Y)].Depth) continue;
                        if (siblingNode == endPosition)
                        {
                            solutions[depth]++;  
                            continue;
                        }

                        seen.Add(siblingNode);
                        nodes.Enqueue(siblingNode);
                    }

                    //else cheat from this position
                    foreach (var (Dx, Dy) in teleports)
                    {

                        (int X, int Y) siblingNode = (currNode.X + Dx, currNode.Y + Dy);
                        if (siblingNode.X < 0 || siblingNode.X >= numOfRows) continue;
                        if (siblingNode.Y < 0 || siblingNode.Y >= numofColumns) continue;
                        if (matrix[siblingNode.X, siblingNode.Y] == WALL_CHAR) continue;
                        if (graph[siblingNode].Depth <= depth +1) continue;
                        if (!bestPathOnlyNodes.Contains(siblingNode)) continue;
                        if (siblingNode == endPosition)
                        {
                            solutions[depth + 1]++;
                            continue;
                        }

                        int currDone = depth + 1;
                        int toBeDone = solutions.Length - graph[siblingNode].Depth;
                        int fixedDepth = currDone + toBeDone;
                        solutions[fixedDepth]++;
                    }




                }
                depth++;
            }

            throw new ImpossibleException();


        }

        private class NodeData((int X, int Y)? parent, int depth) {

            public (int X, int Y)? Parent = parent;
            public int Depth = depth;
        }

        private static int Bfs(char[,] matrix, (int X, int Y) startPosition, (int X, int Y) endPosition, Dictionary<(int X, int Y), NodeData> graph) {

            int numOfRows = matrix.GetLength(0);
            int numofColumns = matrix.GetLength(1);
            Queue<(int X, int Y)> nodes = [];

            nodes.Enqueue(startPosition);
            graph.Add(startPosition, new NodeData(null, 0));

            int depth = 1;
            while (nodes.Count > 0) {

                int num = nodes.Count;
                for (int i = 0; i < num; i++) {

                    (int X, int Y) currNode = nodes.Dequeue();
                    foreach (var (Dx, Dy) in directions) {

                        (int X, int Y) siblingNode = (currNode.X + Dx, currNode.Y + Dy);
                        if (siblingNode.X < 0 || siblingNode.X >= numOfRows) continue;
                        if (siblingNode.Y < 0 || siblingNode.Y >= numofColumns) continue;
                        if (matrix[siblingNode.X, siblingNode.Y] == WALL_CHAR) continue;
                        if (graph.ContainsKey(siblingNode)) continue;
                        graph.Add(siblingNode, new NodeData(currNode, depth));
                        if (siblingNode == endPosition) {
                            return depth;
                        }

                        nodes.Enqueue(siblingNode);
                    }
                    

                }
                depth++;
            }

            throw new ImpossibleException();
            

        }

        private static HashSet<(int X, int Y)> GetBestPathNodes(Dictionary<(int X, int Y), NodeData> graph, (int X, int Y) endPosition) {
            HashSet<(int X, int Y)> bestPathOnlyNodes = [];

            (int X, int Y)? currNode = endPosition;
            while (currNode != null) {

                bestPathOnlyNodes.Add(currNode.Value);
                currNode = graph[currNode.Value].Parent;

            }

            return bestPathOnlyNodes;

        }





    }
    
}
