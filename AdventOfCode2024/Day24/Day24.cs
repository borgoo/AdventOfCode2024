
namespace AdventOfCode2024.Day24
{
    internal class Day24 : Day
    {
        private const char OUTPUTGATE_CHAR = 'z';
        protected override object SolveA(string input)
        {
            int i = 0;
            string[] inputLines = input.Split("\r\n");
            Dictionary<string, bool> gatesOutput = BuildGatesOutput(inputLines, ref i);
            short maxResVal = -1;
            Dictionary<string, Gate> graph = BuildGraph(inputLines, ref i, ref maxResVal, OUTPUTGATE_CHAR);
            if (maxResVal < 0) throw new Exception($"At least one output gate is required ({OUTPUTGATE_CHAR}**)");
            if (maxResVal > 99) throw new Exception($"Too many output gates ({OUTPUTGATE_CHAR}**)");

            double ans = 0;

            for (ushort currOutputGate = 0; currOutputGate <= maxResVal; currOutputGate++) {

                string outputGateName = OUTPUTGATE_CHAR + currOutputGate.ToString("D2");
                bool val = ExploreGraph(graph, gatesOutput, outputGateName);
                if (!val) continue;

                ans += Math.Pow(2, currOutputGate);
            }

            return ans;
        }

        protected override object SolveB(string input)
        {
            throw new NotImplementedException();
        }

        private enum GateType { 
            AND,
            OR,
            XOR
        }

        private class Gate(string inputLeft, GateType gateType, string inputRight) {

            public string InputLeft { get; private set; } = inputLeft;
            public string InputRight { get; private set; } = inputRight;
            public GateType Operation{ get; private set; } = gateType;
            
        }

        private static Dictionary<string, bool> BuildGatesOutput(string[] lines, ref int i) {

            const string TRUE_AS_STRING = "1";

            Dictionary<string, bool> gatesOutput = [];

            while (i < lines.Length && !String.IsNullOrEmpty(lines[i]))
            {
                string line = lines[i];
                string[] tmp = line.Split(": ");
                string key = tmp[0];
                string val = tmp[1];
                gatesOutput.Add(key, val == TRUE_AS_STRING);
                i++;
            }
            i++;

            return gatesOutput;
        }

        private static Dictionary<string, Gate> BuildGraph(string[] lines, ref int i, ref short maxResVal, char resChar)
        {

            Dictionary<string, Gate> gatesRequiredInputs = [];

            while (i < lines.Length)
            {

                string operation = lines[i];
                string[] tmp = operation.Split(' ');
                string inputA = tmp[0];
                if (!Enum.TryParse(tmp[1], out GateType gate)) throw new NotImplementedException(tmp[1]);
                string inputB = tmp[2];
                string output = tmp[4];

                if (output[0] == resChar) {
                    short id = short.Parse(output[1..]);
                    if (id > maxResVal) maxResVal = id;
                }

                gatesRequiredInputs.Add(output, new(inputA, gate, inputB));

                i++;

            }

            return gatesRequiredInputs;



        }

        private static bool ExploreGraph(Dictionary<string, Gate> graph, Dictionary<string, bool>  gatesOuput, string currGate) {

            if (gatesOuput.ContainsKey(currGate)) return gatesOuput[currGate];

            Gate data = graph[currGate];

            bool leftVal;
            if (gatesOuput.ContainsKey(data.InputLeft))
            {
                leftVal = gatesOuput[data.InputLeft];
            }
            else {
                leftVal = ExploreGraph(graph, gatesOuput, data.InputLeft);
            }

            bool rightVal;
            if (gatesOuput.ContainsKey(data.InputRight))
            {
                rightVal = gatesOuput[data.InputRight];
            }
            else
            {
                rightVal = ExploreGraph(graph, gatesOuput, data.InputRight);
            }

            bool currRes = Do(leftVal, data.Operation, rightVal);
            gatesOuput.Add(currGate, currRes);

            return currRes;

        }



        private static bool Do(bool a, GateType gate, bool b) {

            return gate switch
            {
                GateType.OR => a == true || b == true,
                GateType.AND => a != false && b != false,
                GateType.XOR => a != b,
                _ => throw new NotImplementedException(),
            };
        }




    }
}
