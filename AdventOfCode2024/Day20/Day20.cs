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
            _waitingBar.Show();
            var (matrix, startingPosition, endPosition) = HandleInput(input);
            Dictionary<(int X, int Y), int> seen = [];
            int best = Bfs(matrix, startingPosition, endPosition, seen);
            int[] results = new int[best];
            CheatSomePath(matrix, startingPosition, endPosition, seen, results);
            _waitingBar.Terminate();

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
            Dictionary<(int X, int Y), int> bestPathHistory,
            int[] solutions
            
        )
        {

            int numOfRows = matrix.GetLength(0);
            int numofColumns = matrix.GetLength(1);
            HashSet<(int X, int Y)> seen = [];
            Queue<(int X, int Y, bool Cheated)> nodes = [];

            nodes.Enqueue((startPosition.X, startPosition.Y, false));
            seen.Add(startPosition);

            int depth = 1;
            while (nodes.Count > 0)
            {
                if (depth >= solutions.Length) return;

                int num = nodes.Count;
                for (int i = 0; i < num; i++)
                {

                    (int X, int Y, bool Cheated) currNode = nodes.Dequeue();
                    bool alreadyCheated = currNode.Cheated;

                    foreach (var (Dx, Dy) in directions)
                    {

                        (int X, int Y) siblingNode = (currNode.X + Dx, currNode.Y + Dy);
                        if (siblingNode.X < 0 || siblingNode.X >= numOfRows) continue;
                        if (siblingNode.Y < 0 || siblingNode.Y >= numofColumns) continue;
                        if (matrix[siblingNode.X, siblingNode.Y] == WALL_CHAR) continue;
                        if (!alreadyCheated && seen.Contains(siblingNode)) continue;
                        if (bestPathHistory[siblingNode] < bestPathHistory[(currNode.X, currNode.Y)]) continue;
                        if (siblingNode == endPosition)
                        {
                            int fixedDepth = alreadyCheated ? depth +1 : depth;
                            solutions[fixedDepth]++;  
                            continue;
                        }

                        if(!alreadyCheated) seen.Add(siblingNode);
                        nodes.Enqueue((siblingNode.X, siblingNode.Y, alreadyCheated));
                    }

                    if (alreadyCheated) continue;

                    bool cheated = true;
                    //else cheat from this position
                    foreach (var (Dx, Dy) in teleports)
                    {

                        (int X, int Y) siblingNode = (currNode.X + Dx, currNode.Y + Dy);
                        if (siblingNode.X < 0 || siblingNode.X >= numOfRows) continue;
                        if (siblingNode.Y < 0 || siblingNode.Y >= numofColumns) continue;
                        if (matrix[siblingNode.X, siblingNode.Y] == WALL_CHAR) continue;
                        if (bestPathHistory[siblingNode] <= depth +1) continue;
                        if (siblingNode == endPosition)
                        {
                            solutions[depth + 1]++;
                            continue;
                        }

                        nodes.Enqueue((siblingNode.X, siblingNode.Y, cheated));
                    }




                }
                depth++;
            }

            throw new ImpossibleException();


        }

        private static int Bfs(char[,] matrix, (int X, int Y) startPosition, (int X, int Y) endPosition, Dictionary<(int X, int Y), int> history) {

            int numOfRows = matrix.GetLength(0);
            int numofColumns = matrix.GetLength(1);
            Queue<(int X, int Y)> nodes = [];

            nodes.Enqueue(startPosition);
            history.Add(startPosition, 0);

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
                        if (history.ContainsKey(siblingNode)) continue;
                        history.Add(siblingNode, depth);
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
      

     
     

    }
    
}
