

namespace AdventOfCode2024.Day11
{
    internal class Day11 : Day
    {
        const int MUL_VAL = 2024;
        const int SOLB_BLINKS = 75;
        private static readonly bool _debugActive = false;

      
        protected override long SolveA(string input)
        {
            return Resolve(input);
        }

        protected override long SolveB(string input)
        {
            int forcedNumOfBlinks = SOLB_BLINKS;
            return Resolve(input, forcedNumOfBlinks);
        }

        private static long Resolve(string input, int? forcedNumOfBlinks = null)
        {
            (string[] Values, int BlinkLimits) inputs = ManageInput(input);
            int blinksLimit = forcedNumOfBlinks ?? inputs.BlinkLimits;
            Dictionary<(long Val, int Depth), long> cache = [];

            long ans = inputs.Values.Length;
            foreach (string val in inputs.Values)
            {
                long num = Convert.ToInt64(val);
                ans += Blink(num, cache, blinksLimit, 0);
            }
            if (_debugActive) Console.WriteLine();
            return ans;
        }

        private static (string[] Values, int BlinkLimits) ManageInput(string input) {

            string[] inputs = input.Split("\r\n");
            int blinksLimit = Convert.ToInt32(inputs[0]);

            return (Values: inputs[1].Split(" "), BlinkLimits: blinksLimit);
        }
            
        /// <summary>
        /// Kinda DFS
        /// </summary>
        /// <param name="val"></param>
        /// <param name="cache"></param>
        /// <param name="blinksLimit"></param>
        /// <param name="currentDepth"></param>
        /// <param name="currentNumOfChildren"></param>
        /// <returns></returns>
        private static long Blink(
            long val,
            Dictionary<(long Val, int Depth), long> cache,
            int blinksLimit,
            int currentDepth
        ) {

            if (cache.TryGetValue((val, currentDepth), out long ans)) {
                return ans;
            }

            if (blinksLimit == currentDepth)
            {
                return 0;
            }

            List<long> children = [];
            if (val == 0)
            {
                children.Add(1);
            }
            else { 
                string str = val.ToString();

                if (str.Length % 2 != 0)
                {
                    children.Add(val * MUL_VAL);
                }
                else {

                    children.Add(long.Parse(str.Substring(0, str.Length / 2)));
                    children.Add(long.Parse(str.Substring(str.Length / 2, str.Length - (str.Length / 2))));
                }
            }

            long addedChildrenFromBlinkingUntilLimit = children.Count - 1;

            foreach (long son in children) { 
            
                addedChildrenFromBlinkingUntilLimit += Blink(son, cache, blinksLimit, currentDepth + 1);
            }

            cache.TryAdd((val, currentDepth), addedChildrenFromBlinkingUntilLimit);

            return addedChildrenFromBlinkingUntilLimit;

        }





    }
}
