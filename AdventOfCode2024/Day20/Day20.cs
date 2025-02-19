using AdventOfCode2024.Exceptions;
using AdventOfCode2024.Utils;

namespace AdventOfCode2024.Day20
{
    internal class Day20 : Day
    {
        private static readonly bool _debugActiveSolA = false;
        private static readonly bool _debugActiveSolB = false;

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

       
        protected override object SolveA(string input)
        {
            return Solve(input, 2);

        }

        protected override object SolveB(string input)
        {
            return Solve(input, 20);
        }


        private int Solve(string input, int cheatMaxLength, int minNumOfPicosecondsToSave = MIN_NUM_OF_PICOSECONDS_TO_SAVE) {

            var (matrix, startingPosition, endPosition) = HandleInput(input);
            Dictionary<int, (int Dx, int Dy)[]> cheatLengthTeleports = BuildTeleports(cheatMaxLength);
            Dictionary<(int X, int Y), NodeData> graph = [];
            int best = Bfs(matrix, startingPosition, endPosition, graph);
            HashSet<(int X, int Y)> bestPathOnlyNodes = GetBestPathNodes(graph, endPosition);
            int[] timesToReachEndCheated = new int[best];
            MoveThrowExplorablePositionsAndCheat(matrix, startingPosition, cheatMaxLength, cheatLengthTeleports, graph, bestPathOnlyNodes, timesToReachEndCheated);

            bool debugActive = (!SolBIsRunning && _debugActiveSolA) || (SolBIsRunning && _debugActiveSolB);
            for (int i = best - 1; debugActive && i >= 0; i--)
            {
                if (timesToReachEndCheated[i] == 0) continue;
                Console.WriteLine($"There are {timesToReachEndCheated[i]} cheats that save {best - i} picoseconds.");

            }

            int ans = 0;
            for (int i = 0; i < best; i++)
            {
                if (timesToReachEndCheated[i] == 0) continue;
                if (best - i < minNumOfPicosecondsToSave) break;
                ans += timesToReachEndCheated[i];

            }


            return ans;

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

        private static void MoveThrowExplorablePositionsAndCheat(
        char[,] matrix,
        (int X, int Y) startPosition,
        int cheatMaxLength,
        Dictionary<int, (int Dx, int Dy)[]> cheatLengthTeleports,
        Dictionary<(int X, int Y), NodeData> graph,
        HashSet<(int X, int Y)> bestPathOnlyNodes,
        int[] solutions

    )
        {
            int numOfRows = matrix.GetLength(0);
            int numOfColumns = matrix.GetLength(1);
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

                    //move through explorable positions
                    foreach (var (Dx, Dy) in directions)
                    {

                        (int X, int Y) siblingNode = (currNode.X + Dx, currNode.Y + Dy);
                        if (siblingNode.X < 0 || siblingNode.X >= numOfRows) continue;
                        if (siblingNode.Y < 0 || siblingNode.Y >= numOfColumns) continue;
                        if (matrix[siblingNode.X, siblingNode.Y] == WALL_CHAR) continue;
                        if (seen.Contains(siblingNode)) continue;
                        if (graph[siblingNode].Depth < graph[(currNode.X, currNode.Y)].Depth) continue;
                      

                        seen.Add(siblingNode);
                        nodes.Enqueue(siblingNode);
                    }

                    //cheat from the considered position once per path
                    CheatFromThisPositon(matrix, cheatMaxLength, numOfRows, numOfColumns, currNode, graph, cheatLengthTeleports, bestPathOnlyNodes, solutions);


                }
                depth++;
            }

            throw new ImpossibleException();


        }


        private static Dictionary<int, (int Dx, int Dy)[]> BuildTeleports(int maxLength) {

            Dictionary<int, (int Dx, int Dy)[]> cheatLengthTeleports = [];
            for (int length = 1; length <= maxLength; length++) {

                HashSet < (int X, int Y) > teleports = BuildOptimizedTeleportsByCheatLength(length);
                cheatLengthTeleports.Add(length, [.. teleports]);
            }

            return cheatLengthTeleports;
        
        }

        private static HashSet<(int X, int Y)> BuildOptimizedTeleportsByCheatLength(int cheatLength)
        {

            HashSet<(int X, int Y)> teleports = new();
            for (int i = 0; i <= cheatLength; i++)
            {

                int x = i;
                int y = cheatLength - i;
                teleports.Add((x, y));
                teleports.Add((x, -y));
                teleports.Add((-x, -y));
                teleports.Add((-x, +y));
            }

            return teleports;


        }


        private static void CheatFromThisPositon(
            char[,] matrix,
            int cheatMaxLength,
            int numOfRows,
            int numOfColumns,
            (int X, int Y) currNode,
            Dictionary<(int X, int Y), NodeData> graph,
            Dictionary<int, (int Dx, int Dy)[]> cheatLengthTeleports,
            HashSet<(int X, int Y)> bestPathOnlyNodes,
            int[] solutions
        ) {

            int currDepth = graph[currNode].Depth;

            for (int cheatLength = 2; cheatLength <= cheatMaxLength; cheatLength++) {

                (int Dx, int Dy)[] teleports = cheatLengthTeleports[cheatLength];
                foreach (var (Dx, Dy) in teleports)
                {
                    (int X, int Y) siblingNode = (currNode.X + Dx, currNode.Y + Dy);
                    if(siblingNode.X < 0 || siblingNode.X >= numOfRows) continue;
                    if(siblingNode.Y < 0 || siblingNode.Y >= numOfColumns) continue;
                    if (matrix[siblingNode.X, siblingNode.Y] == WALL_CHAR) continue;
                    if(!bestPathOnlyNodes.Contains(siblingNode)) continue;
                    int destDepthOnNormalPath = graph[siblingNode].Depth;
                    if (currDepth + cheatLength >= destDepthOnNormalPath) continue;

                    int remainingMovesAfterTeleport = solutions.Length - destDepthOnNormalPath;
                    int fixedDepth = currDepth + cheatLength + remainingMovesAfterTeleport;
                    solutions[fixedDepth]++;

                }


            }
        }
    
    }
    
}
