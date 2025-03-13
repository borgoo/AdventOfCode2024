
using AdventOfCode2024.Exceptions;

namespace AdventOfCode2024.Day25
{
    internal class Day25 : Day
    {
        const int LOCKS_AND_KEYS_HEIGHT = 7;
        const int LOCKS_AND_KEYS_WIDTH = 5;
        const char BUSY_CHAR = '#';
        const char EMPTY_CHAR = '.';
        private static readonly string LOCK_FIRSTLINE_CHAR = new(BUSY_CHAR, LOCKS_AND_KEYS_WIDTH);
        private static readonly string KEY_FIRSTLINE_CHAR = new(EMPTY_CHAR, LOCKS_AND_KEYS_WIDTH);

        protected override object SolveA(string input)
        {
            int ans = 0;
            var (Locks, Keys) = HandleInput(input);

            foreach (int[] currLock in Locks)
            {

                foreach (int[] key in Keys)
                {
                    ans += CheckCompatibility(currLock, key);
                }
            }

            return ans;

        }

        protected override object SolveB(string input)
        {
            throw new IsChristmasTimeException();
        }

        private static int CheckCompatibility(int[] currLockAvailableSpaces, int[] currKeyBusySpaces)
        {

            for (int i = 0; i < LOCKS_AND_KEYS_WIDTH; i++)
            {
                if (currKeyBusySpaces[i] > currLockAvailableSpaces[i]) return 0;
            }
            return 1;
        }

        private static int[] ReadObj(string[] lines, int cols, int i, char charToCheck)
        {

            int[] curr = new int[LOCKS_AND_KEYS_WIDTH];
            for (int j = 0; j < cols; j++)
            {

                curr[j] = LOCKS_AND_KEYS_HEIGHT;
                for (int currR = 0; currR < LOCKS_AND_KEYS_HEIGHT; currR++)
                {
                    if (lines[i + currR][j] == charToCheck) break;
                    curr[j]--;
                }
            }

            return curr;

        }

        private static (List<int[]> LocksAvailableSpaces, List<int[]> KeysBusySpaces) HandleInput(string input)
        {

            List<int[]> locks = [];
            List<int[]> keys = [];

            string[] lines = input.Split("\r\n");
            int cols = LOCKS_AND_KEYS_WIDTH;
            int rows = lines.Length;

            int skipRows = LOCKS_AND_KEYS_HEIGHT + 1;

            for (int i = 0; i < rows; i += skipRows)
            {

                if (lines[i] == LOCK_FIRSTLINE_CHAR)
                {

                    int[] currLock = ReadObj(lines, cols, i, EMPTY_CHAR);
                    locks.Add(currLock);


                }
                else if (lines[i] == KEY_FIRSTLINE_CHAR)
                {

                    int[] currKey = ReadObj(lines, cols, i, BUSY_CHAR);
                    keys.Add(currKey);

                }
                else
                {
                    throw new NotHandledException();
                }

            }

            return (locks, keys);


        }


    }
}
