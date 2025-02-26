using System.Text;
namespace AdventOfCode2024.Day21
{

    internal class Day21 : Day
    {

        private static readonly bool _debugActive = false;
        /*
         * 789
         * 456
         * 123
         *  0A  
         */
        private static readonly Dictionary<char, (int X, int Y)> numericKeypad = new() {

            { '7', (0, 0) },
            { '8', (0, 1) },
            { '9', (0, 2) },
            { '4', (1, 0) },
            { '5', (1, 1) },
            { '6', (1, 2) },
            { '1', (2, 0) },
            { '2', (2, 1) },
            { '3', (2, 2) },
            { PANIC_CHAR,   (3,0) },
            { '0', (3, 1) },
            { ENTER_CHAR, (3, 2) }
        };


        /*
         *  ^A
         * <v> 
         */
        private static readonly Dictionary<char, (int X, int Y)> directionalKeypad = new()
        {
            { PANIC_CHAR, (0, 0) },
            { '^', (0, 1) },
            { ENTER_CHAR, (0, 2) },
            { '<', (1, 0) },
            { 'v', (1, 1) },
            { '>', (1, 2) }
        };


        private static readonly Dictionary<char, int> directionalKeypadCosts = new()
        {
            { ENTER_CHAR, 0},
            { '<', 4},
            { '^', 2}, // WARNING: This is the key! Robot1 needs to press < to reach the output ^ (required from Robot2) so, 
            { '>', 1}, // even if ^,> have the same distance from A, ^ of Robot 2 require Robot1 to press <
            { 'v', 3},

        };


        private static readonly Dictionary<char, (int Dx, int Dy)> directions = new()
        {
            { '<', (0,-1)},
            { '^', (-1,0)},
            { '>', (0,1)},
            { 'v', (1,0)}
        };


        const int SOLB_NUM_OF_DIRECTIONALKEYPAD_ROBOTS = 25;
        const int SOLA_NUM_OF_DIRECTIONALKEYPAD_ROBOTS = 2;
        const char PANIC_CHAR = ' ';
        const char ENTER_CHAR = 'A';
        const char DIRECTIONALKEYPAD_STARTING_CHAR = ENTER_CHAR;
        const char NUMERICKEYPAD_STARTING_CHAR = ENTER_CHAR;


        private class SharedCache
        {

            public readonly Dictionary<(string Cmd, int Depth), long> resultStrLengthCache = [];
            public readonly Dictionary<(char, char), string> directionalKeypadMovesCache = BuildMoves(directionalKeypad, PANIC_CHAR);
            public readonly Dictionary<(char, char), string> numericKeypadMovesCache = BuildMoves(numericKeypad, PANIC_CHAR);
        }


        protected override object SolveA(string input)
        {
            SharedCache cache = new();
            return Solve(input, SOLA_NUM_OF_DIRECTIONALKEYPAD_ROBOTS, cache);
        }

        protected override object SolveB(string input)
        {
            SharedCache cache = new();
            return Solve(input, SOLB_NUM_OF_DIRECTIONALKEYPAD_ROBOTS, cache);
        }

        private static long Solve(string input, int numOfDirectionalKeypadRobots, SharedCache cache)
        {

            string[] passwords = input.Split("\r\n");

            long complexity = 0;
            foreach (string password in passwords)
            {

                string outputToMake = GetRequiredInputForNumericKeypad(password, cache);

                long resultStrLength = RecursionGoDeep(outputToMake, 0, numOfDirectionalKeypadRobots, cache);

                complexity += CalcComplexity(password, resultStrLength);
            }

            return complexity;


        }

        private static string CalcCmdInput(string cmd, Dictionary<(char, char), string> ouputKeypadMovesCache)
        {

            StringBuilder sb = new();
            for (int i = 0; i < cmd.Length - 1; i++)
            {
                char from = cmd[i];
                char to = cmd[i + 1];
                sb.Append(ouputKeypadMovesCache[(from, to)]);
            }

            return sb.ToString();
        }

        private static string GetRequiredInputForNumericKeypad(string desiredOutput, SharedCache cache)
        {

            string cmd = NUMERICKEYPAD_STARTING_CHAR + desiredOutput;

            string ans = CalcCmdInput(cmd, cache.numericKeypadMovesCache);
            return ans;

        }



        private static long RecursionGoDeep(string originalCmd, int currDepth, int depthLimit, SharedCache cache)
        {

            if (currDepth == depthLimit)
            {
                return originalCmd.Length;
            }

            var cmdLevelKey = (originalCmd, currDepth);

            if (cache.resultStrLengthCache.ContainsKey(cmdLevelKey))
            {
                return cache.resultStrLengthCache[cmdLevelKey];
            }

            long maxDepthValue = 0;


            string[] cmds = originalCmd[..^1].Split(ENTER_CHAR);

            foreach (string cmd in cmds)
            {

                if (cache.resultStrLengthCache.ContainsKey((cmd + ENTER_CHAR, currDepth)))
                {
                    maxDepthValue += cache.resultStrLengthCache[(cmd + ENTER_CHAR, currDepth)];
                    continue;
                }

                if (cmd == String.Empty)
                {
                    maxDepthValue++;
                    continue;
                }

                string subCmd = DIRECTIONALKEYPAD_STARTING_CHAR + cmd + ENTER_CHAR;
                string partialSol = CalcCmdInput(subCmd, cache.directionalKeypadMovesCache);

                maxDepthValue += RecursionGoDeep(partialSol, currDepth + 1, depthLimit, cache);

            }

            cache.resultStrLengthCache.Add(cmdLevelKey, maxDepthValue);
            return maxDepthValue;

        }





        private static long CalcComplexity(string password, long strLen)
        {
            long num = 0;
            if (!string.IsNullOrEmpty(password))
            {
                num = Convert.ToInt64(password[..^1]);
            }
            if (_debugActive) Console.WriteLine($"{password} | {strLen}*{num}");
            return num * strLen;

        }



        private static Dictionary<(char, char), string> BuildMoves(Dictionary<char, (int X, int Y)> keypad, char panicChar)
        {

            Dictionary<(char, char), string> result = [];

            foreach (char c in keypad.Keys)
            {

                if (c == panicChar) continue;

                foreach (char other in keypad.Keys)
                {

                    if (other == panicChar) continue;

                    (char From, char To) mapKey = (c, other);
                    string ans = CalcMove(c, other, keypad, panicChar);
                    result.Add(mapKey, ans);

                }
            }

            return result;

        }

        protected static string Move((int X, int Y) startingPosition, (int Num, char C)[] pair, Dictionary<char, (int X, int Y)> keypad, char panicChar)
        {

            StringBuilder result = new();

            (int X, int Y) currPosition = startingPosition;
            foreach ((int Num, char C) in pair)
            {
                (int Dx, int Dy) = directions[C];
                for (int i = 0; i < Num; i++)
                {
                    currPosition = (currPosition.X + Dx, currPosition.Y + Dy);
                    if (currPosition == keypad[panicChar])
                        throw new PanicException();

                    result.Append(C);
                }

            }

            return result.ToString();

        }

        private static (int Num, char C)[] SortMovements((int Num, char C) a, (int Num, char C) b)
        {

            (int Num, char C)[] pair = new (int Num, char C)[2];

            if (directionalKeypadCosts[a.C] < directionalKeypadCosts[b.C])
            {
                pair[0] = b;
                pair[1] = a;
            }
            else
            {
                pair[0] = a;
                pair[1] = b;
            }

            return pair;
        }


        private static string CalcMove(char from, char to, Dictionary<char, (int X, int Y)> outputKeypad, char panicChar)
        {

            char prevButton = from;
            char currButton = to;
            (int X, int Y) prevButtonPosition = outputKeypad[prevButton];
            StringBuilder input = new();

            (int X, int Y) currButtonPosition = outputKeypad[currButton];
            int dx = currButtonPosition.X - prevButtonPosition.X;
            int dy = currButtonPosition.Y - prevButtonPosition.Y;

            (int Num, char C) vertical = (Math.Abs(dx), dx > 0 ? 'v' : '^');
            (int Num, char C) horizontal = (Math.Abs(dy), dy > 0 ? '>' : '<');

            (int Num, char C)[] pair = new (int Num, char C)[2];
            string moves;
            try
            {
                pair = SortMovements(vertical, horizontal);
                moves = Move(prevButtonPosition, pair, outputKeypad, panicChar);
            }
            catch (PanicException)
            {
                (int Num, char C) tmp = pair[0];
                pair[0] = pair[1];
                pair[1] = tmp;
                moves = Move(prevButtonPosition, pair, outputKeypad, panicChar);
            }

            string partialInput = moves + ENTER_CHAR;
            input.Append(partialInput);


            return input.ToString();

        }


        private class PanicException() : Exception() { }


    }
}
