using AdventOfCode2024.Exceptions;
using AdventOfCode2024.Utils;

namespace AdventOfCode2024.Day20
{
    internal class Day20 : Day
    {
        private static readonly bool _debugActiveSolA = false;
        private static readonly bool _debugActiveSolB = false;

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

        private readonly static (int Dx, int Dy)[] solATeleports = [
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
            int[] timesToReachEndCheated = new int[best];
            CheatSomePath(matrix, startingPosition, endPosition, graph, bestPathOnlyNodes, timesToReachEndCheated);

            for (int i = best - 1; _debugActiveSolA && i >= 0; i--)
            {
                if (timesToReachEndCheated[i] == 0) continue;
                Console.WriteLine($"There are {timesToReachEndCheated[i]} cheats that save {best - i} picoseconds.");

            }

            int minNumOfPicosecondsToSave = MIN_NUM_OF_PICOSECONDS_TO_SAVE;
            int ans = 0;
            for (int i = 0;i<best; i++)
            {
                if (timesToReachEndCheated[i] == 0) continue;
                if (best - i < minNumOfPicosecondsToSave) break;
                ans += timesToReachEndCheated[i];

            }


            return ans;

        }

        protected override object SolveB(string input)
        {

            c++;
            if (c != 1) return -1;

            var (matrix, startingPosition, endPosition) = HandleInput(input);
            int cheatMaxLength = SOLB_MAX_CHEAT_LENGTH;
            Dictionary<int, (int Dx, int Dy)[]> cheatLengthTeleports = BuildTeleports(cheatMaxLength);
            Dictionary<(int X, int Y), NodeData> graph = [];
            int best = Bfs(matrix, startingPosition, endPosition, graph);
            HashSet<(int X, int Y)> bestPathOnlyNodes = GetBestPathNodes(graph, endPosition);
            int[] timesToReachEndCheated = new int[best];
            CheatSomePathAdvanced(matrix, startingPosition, endPosition, cheatMaxLength, cheatLengthTeleports, graph, bestPathOnlyNodes, timesToReachEndCheated);

            for (int i = best - 1; _debugActiveSolB && i >= 0; i--)
            {
                if (timesToReachEndCheated[i] == 0) continue;
                Console.WriteLine($"There are {timesToReachEndCheated[i]} cheats that save {best - i} picoseconds.");

            }

            int minNumOfPicosecondsToSave = MIN_NUM_OF_PICOSECONDS_TO_SAVE;
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


                    foreach (var (Dx, Dy) in directions)
                    {

                        (int X, int Y) siblingNode = (currNode.X + Dx, currNode.Y + Dy);
                        if (siblingNode.X < 0 || siblingNode.X >= numOfRows) continue;
                        if (siblingNode.Y < 0 || siblingNode.Y >= numofColumns) continue;
                        if (matrix[siblingNode.X, siblingNode.Y] == WALL_CHAR) continue;
                        if (seen.Contains(siblingNode)) continue;
                        if (graph[siblingNode].Depth < graph[(currNode.X, currNode.Y)].Depth) continue;
                       

                        seen.Add(siblingNode);
                        nodes.Enqueue(siblingNode);
                    }

                    //else cheat from this position
                    foreach (var (Dx, Dy) in solATeleports)
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

        private static void CheatSomePathAdvanced(
        char[,] matrix,
        (int X, int Y) startPosition,
        (int X, int Y) endPosition,
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

                    //else cheat from this position
                    Dictionary<int, int> bestDepthByLength = GetAllDeeperCellsReachableFromCurrentNodeByCheatLength(matrix, cheatMaxLength, numOfRows, numOfColumns, currNode, graph, cheatLengthTeleports, bestPathOnlyNodes);
                    int currDepth = graph[currNode].Depth;
                    foreach (KeyValuePair<int, int> best in bestDepthByLength) {
                        int cheatLength = best.Key;
                        int reachableDepth = best.Value;
                        if (depth + cheatLength >= reachableDepth) continue;
                        int remainingDepthFromReachablePos = solutions.Length - reachableDepth;
                        int fixedDepth = currDepth + cheatLength + remainingDepthFromReachablePos;
                        solutions[fixedDepth]++;
                        
                    }


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


        /// <summary>
        /// REMEMBER: 
        ///     1)"Because this cheat has the same start and end positions as the one above, it's the same cheat" -> moving in suboptimal ways doesn't pay out (es: one position that can be reached by 2 moves can be reached with 4 as well, but 2 is the right way to do it
        ///     2) "Any cheat time not used is lost; it can't be saved for another cheat later." -> AKA can cheat N [0,20] times foreach position in path but only once per path
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="cheatMaxLength"></param>
        /// <param name="numOfRows"></param>
        /// <param name="numOfColumns"></param>
        /// <param name="currNode"></param>
        /// <param name="graph"></param>
        /// <param name="cheatLengthTeleports"></param>
        /// <param name="bestPathOnlyNodes"></param>
        /// <returns></returns>
        private static Dictionary<int, int> GetAllDeeperCellsReachableFromCurrentNodeByCheatLength(
            char[,] matrix,
            int cheatMaxLength,
            int numOfRows,
            int numOfColumns,
            (int X, int Y) currNode,
            Dictionary<(int X, int Y), NodeData> graph,
            Dictionary<int, (int Dx, int Dy)[]> cheatLengthTeleports,
            HashSet<(int X, int Y)> bestPathOnlyNodes
        ) {

            Dictionary<int, int> bestDepthByLength = [];

            for (int cheatLength = 2; cheatLength <= cheatMaxLength; cheatLength++) {

                int? maxDepthReachable = null;
                foreach (var (Dx, Dy) in cheatLengthTeleports[cheatLength]) { 

                    (int X, int Y) siblingNode = (currNode.X + Dx, currNode.Y + Dy);
                    if (siblingNode.X < 0 || siblingNode.X >= numOfRows) continue;
                    if (siblingNode.Y < 0 || siblingNode.Y >= numOfColumns) continue;
                    if (matrix[siblingNode.X, siblingNode.Y] == WALL_CHAR) continue;
                    if (!bestPathOnlyNodes.Contains(siblingNode)) continue;

                    if (!maxDepthReachable.HasValue) maxDepthReachable = graph[siblingNode].Depth;
                    else maxDepthReachable = Math.Max(maxDepthReachable.Value, graph[siblingNode].Depth);
                }

                //reach positions in dumb ways
                for (int k = cheatLength -2; k >= 2; k -= 2) {

                    if (!maxDepthReachable.HasValue) maxDepthReachable = bestDepthByLength[k];
                    else maxDepthReachable = Math.Max(maxDepthReachable.Value, bestDepthByLength[k]);

                }
                if (maxDepthReachable.HasValue) bestDepthByLength.Add(cheatLength, maxDepthReachable.Value);
                else Console.WriteLine($"Strano nessun maxDepthReachable per {currNode} con cheatLength {cheatLength}");
            }


            return bestDepthByLength;


        }


    }
    
}
