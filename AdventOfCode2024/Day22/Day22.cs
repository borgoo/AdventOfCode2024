
namespace AdventOfCode2024.Day22
{
    internal class Day22 : Day
    {

        private const int NUM_OF_ITERATIONS = 2000;

        private const int MUL_64 = 6; // 2^6 = 64 -> *64 is equivalent to shifting left by 6 psx
        private const int PRUNE_FACTOR_MOD_16777216 = 16777215; // 2^24 = 16777216 -> Mod 2^24 is equivalent to AND with (2^24 - 1)
        private const int DIV_32 = 5; // 2^5 = 32 -> /32 is equivalent to shifting right by 5 (PSX)
        private const int MUL_2048 = 11; // 2^11 = 2048 -> *2048 is equivalent to shifting left by 11 psx
        private const int LAST_DIGIT_MOD = 10; //getting last digit is equivalent to Mod 10
        protected override object SolveA(string input)
        {
            int[] startingSecretNumbers = [.. input.Split("\r\n").Select(e => Convert.ToInt32(e))];

            long ans = 0;
            foreach (int start in startingSecretNumbers) {

                long inc = start;
                for (int i = 0; i < NUM_OF_ITERATIONS; i++) {
                    inc = CalcNextSecretNumber(inc);
                }
                ans += inc;
            }

            return ans;
        }

        protected override object SolveB(string input)
        {
            MaxCalculator maxCalculator = new();
            int[] startingSecretNumbers = [.. input.Split("\r\n").Select(e => Convert.ToInt32(e))];

            for (int j = 0; j < startingSecretNumbers.Length; j++) {

                int start = startingSecretNumbers[j];
                int[] currentBuyerOfferPrices = new int[NUM_OF_ITERATIONS + 1];
                int[] diffHistory = new int[NUM_OF_ITERATIONS + 1];
                currentBuyerOfferPrices[0] = GetBuyerOfferPrices(start);
                diffHistory[0] = currentBuyerOfferPrices[0];
                long inc = start;
                HashSet<(int, int, int, int)> seen = [];
                for (int i = 0; i < NUM_OF_ITERATIONS; i++)
                {
                    inc = CalcNextSecretNumber(inc);
                    currentBuyerOfferPrices[i + 1] = GetBuyerOfferPrices(inc);
                    if (i > 0) diffHistory[i] = currentBuyerOfferPrices[i] - currentBuyerOfferPrices[i - 1];
                    if (i > 3) {

                        (int, int, int, int) key = (diffHistory[i-3], diffHistory[i - 2], diffHistory[i - 1], diffHistory[i]);
                        if (seen.Contains(key)) continue;
                        seen.Add(key);
                        int val = currentBuyerOfferPrices[i];
                        maxCalculator.Add(key, val);
                    }
                }
            }

            return maxCalculator.GetMaxSum();



        }

        private static long CalcNextSecretNumber(long currSecretNumber) {

            long firstStep = currSecretNumber << MUL_64;
            firstStep ^= currSecretNumber;
            firstStep &= PRUNE_FACTOR_MOD_16777216;

            long secondStep = firstStep >> DIV_32;
            secondStep ^= firstStep;
            secondStep &= PRUNE_FACTOR_MOD_16777216;

            long thirdStep = secondStep << MUL_2048;
            thirdStep ^= secondStep;
            thirdStep &= PRUNE_FACTOR_MOD_16777216;

            return thirdStep;

        }

        private static int GetBuyerOfferPrices(long currSecretNumber) {

            return Convert.ToInt32(currSecretNumber % LAST_DIGIT_MOD);
        }



        private class MaxCalculator {

            private (int, int, int, int)? _currMaxKey;

            private readonly Dictionary<(int, int, int, int), int> _keySums = [];

            public void Add((int, int, int, int) key, int val)
            {

                if (!_keySums.ContainsKey(key))
                {
                    _keySums.Add(key, 0);
                }
                _keySums[key] += val;

                if (!_currMaxKey.HasValue || _keySums[key] > GetMaxSum()) _currMaxKey = key;

            }

            public int GetMaxSum() {

                if (!_currMaxKey.HasValue) throw new NullReferenceException();

                return _keySums[_currMaxKey.Value];
            }
        } 
    }


}

