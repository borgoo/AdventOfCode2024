
namespace AdventOfCode2024.Day22
{
    internal class Day22 : Day
    {

        private const int SOLA_NUM_OF_ITERATIONS = 2000;

        private const int MUL_64 = 6; // 2^6 = 64 -> *64 is equivalent to shifting left by 6 psx
        private const int PRUNE_FACTOR_MOD_16777216 = 16777215; // 2^24 = 16777216 -> Modulo 2^24 is equivalent to AND with (2^24 - 1)
        private const int DIV_32 = 5; // 2^5 = 32 -> /32 is equivalent to shifting right by 5 (PSX)
        private const int MUL_2048 = 11; // 2^11 = 2048 -> *2048 is equivalent to shifting left by 11 psx
        protected override object SolveA(string input)
        {
            int[] startingSecretNumbers = [.. input.Split("\r\n").Select(e => Convert.ToInt32(e))];

            long ans = 0;
            foreach (int start in startingSecretNumbers) {

                long inc = start;
                for (int i = 0; i < SOLA_NUM_OF_ITERATIONS; i++) {
                    inc = CalcNextSecretNumber(inc);
                }
                ans += inc;
            }

            return ans;
        }

        protected override object SolveB(string input)
        {
            throw new NotImplementedException();
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

        

    }
}
