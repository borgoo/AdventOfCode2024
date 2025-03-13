
using AdventOfCode2024.Exceptions;
using System.Diagnostics;
using System.Text;


namespace AdventOfCode2024.Day24
{
    internal class Day24 : Day
    {
        private const string BROWSER_EXEC_PATH = ""; // C:\\..\\chrome.exe;

        private const char OUTPUTGATE_CHAR = 'z';
        private const char TRUE_AS_CHAR = '1';
        private const ushort SOLB_ONLY_SOLUTION = 3;

        protected override object SolveA(string input)
        {
            var (ZMaxIdx, Graph, GatesOutput) = HandleInput(input);
            var (Value, _) = SolveTheCircuit(ZMaxIdx, Graph, GatesOutput);
            return Value;

        }


        /// <summary>
        /// TIP USED FROM REDDIT: THE CIRUCUIT IS A FULLADDER!
        /// </summary>
        private static int c = 0;
        protected override object SolveB(string input)
        {
            c++;
            if (c == SOLB_ONLY_SOLUTION)
            {
                return TaylorMadeSolution.Solve(input);
            }

            return 0;
        }

        private static class TaylorMadeSolution
        {
            private const int MAX_NUM_OF_SWITCHES = 4;
            private const string REDIRECT_HTML = @"
            <!DOCTYPE html>
            <html>
                <head>
                    <meta http-equiv='refresh' content='0;url={0}' />
                </head>
                <body>
                    <p>Redirecting...</p>
                </body>
            </html>";

            private static Dictionary<string, bool> BuildYAllTrueInput(short zMaxIdx)
            {

                Dictionary<string, bool> inputs = [];
                for (int i = 0; i <= zMaxIdx; i++)
                {
                    inputs.Add('x' + i.ToString("D2"), false);
                }
                for (int i = 0; i <= zMaxIdx; i++)
                {
                    inputs.Add('y' + i.ToString("D2"), true);
                }
                return inputs;

            }

            private static string CheckResult(string result, string expected)
            {

                for (int i = result.Length -1; i >= 0; i--)
                {

                    if (result[i] != expected[i])
                    {
                        string zName = OUTPUTGATE_CHAR + (result.Length - 1 -i).ToString("D2");
                        return zName;
                    }
                }

                return String.Empty;
            }

            private static string GetGraphVizURL(Dictionary<string, Gate> graph, short zMaxIdx, string? firstWrongZxx)
            {
                string header = "https://dreampuf.github.io/GraphvizOnline/?engine=dot#";
                string encodedURL = header + Uri.EscapeDataString(ParseGraphIntoGraphVizCode(graph, zMaxIdx, firstWrongZxx));
                return encodedURL;
            }

            private static string ParseGraphIntoGraphVizCode(Dictionary<string, Gate> graph, short zMaxIdx, string? firstWrongZxx)
            {

                StringBuilder sb = new();
                HashSet<string> seenWires = [];
                HashSet<int> seenGates = [];


                sb.AppendLine("digraph FullAdder {\n\trankdir=LR;");
                if (!String.IsNullOrEmpty(firstWrongZxx)) sb.AppendLine($"label=\"WRONG {firstWrongZxx}\"; fontsize=500;");
                for (int i = (zMaxIdx - 1); i >= 0; i--)
                {
                    string zxx = OUTPUTGATE_CHAR + i.ToString("D2");
                    BuildCode(graph, firstWrongZxx, zxx, seenWires, seenGates, sb);
                }
                sb.AppendLine("}");

                string res = sb.ToString();

                return res;

            }

            private static void BuildCode(Dictionary<string, Gate> graph, string? firstWrongZxx, string currWire, HashSet<string> seenWires, HashSet<int> seenGates, StringBuilder sb)
            {

                if (!graph.ContainsKey(currWire)) return;

                Gate gate = graph[currWire];
                BuildCode(graph, firstWrongZxx, gate.InputLeft, seenWires, seenGates, sb);
                BuildCode(graph, firstWrongZxx, gate.InputRight, seenWires, seenGates, sb);

                if (seenGates.Contains(gate.GateID)) return;
                seenGates.Add(gate.GateID);
                sb.AppendLine($"{gate.Operation}{gate.GateID} [shape=box, label=\"{gate.Operation}\"];");


                if (!seenWires.Contains(currWire))
                {
                    string fill = String.Empty;
                    if (currWire.StartsWith(OUTPUTGATE_CHAR))
                    {
                        string color = !string.IsNullOrEmpty(firstWrongZxx) && currWire == firstWrongZxx ? "red" : "lightgreen";
                        fill = $"style=filled, fillcolor=\"{color}\"";
                    }
                    sb.AppendLine($"{currWire}[shape = circle, label = \"{currWire}\" {fill}];");
                    seenWires.Add(currWire);
                }

                if (!seenWires.Contains(gate.InputLeft))
                {
                    sb.AppendLine($"{gate.InputLeft}[shape = circle, label = \"{gate.InputLeft}\"];");
                    seenWires.Add(gate.InputLeft);
                }

                if (!seenWires.Contains(gate.InputLeft))
                {
                    sb.AppendLine($"{gate.InputRight}[shape = circle, label = \"{gate.InputRight}\"];");
                    seenWires.Add(gate.InputRight);
                }


                sb.AppendLine($"{gate.Operation}{gate.GateID} -> {currWire};");
                sb.AppendLine($"{gate.InputLeft} -> {gate.Operation}{gate.GateID};");
                sb.AppendLine($"{gate.InputRight} -> {gate.Operation}{gate.GateID};");

            }

            private static string SortAns(List<string> answer)
            {

                var tmp = answer.ToArray();
                Array.Sort(tmp);
                return string.Join(',', tmp);
            }

            private static void DisplayGraph(Dictionary<string, Gate> graph,short zMaxIdx, string firstWrongZxx) {

                if (string.IsNullOrEmpty(BROWSER_EXEC_PATH)) throw new Exception("Missing BROWSER_EXEC_PATH value.");

                Console.Write($"[Wrong {firstWrongZxx}] See your browser to see the circuit preview. Press ENTER to continue... ");

                string url = GetGraphVizURL(graph, zMaxIdx, firstWrongZxx);

                //URL IS TOO LONG TO BE CALLED DIRECTLY FROM CMD
                string tempHtmlFile = Path.GetTempFileName() + ".html";
                string htmlContent = String.Format(REDIRECT_HTML, url);
                File.WriteAllText(tempHtmlFile, htmlContent);
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = BROWSER_EXEC_PATH,
                        Arguments = tempHtmlFile,
                        UseShellExecute = false
                    });
                    _ = Console.ReadLine();
                }
                catch(Exception)
                {
                    Console.WriteLine("Failed to open the browser to show the circuit.");
                    throw;
                }
                finally { 
                   if(File.Exists(tempHtmlFile)) File.Delete(tempHtmlFile);
                }

            }

            private static (string gateA, string gateB) ProceedSwitching(Dictionary<string, Gate> graph) {

                string gateA = String.Empty;
                string gateB = String.Empty;
                
                bool valid = false;
                while (!valid) {
                    Console.Write("First wire to switch: ");
                    gateA = Console.ReadLine() ?? String.Empty;
                    valid = !string.IsNullOrEmpty(gateA) && graph.ContainsKey(gateA);                 
                }
                valid = false;
                while (!valid)
                {
                    Console.Write("Second wire to switch: ");
                    gateB = Console.ReadLine() ?? String.Empty;
                    valid = !string.IsNullOrEmpty(gateB) && graph.ContainsKey(gateB);
                }

                (graph[gateB], graph[gateA]) = (graph[gateA], graph[gateB]);

                return (gateA, gateB);
            }
            public static string Solve(string input)
            {

                List<string> ans = [];

                string[] inputLines = input.Split("\r\n");
                int i = 0;
                while (!string.IsNullOrEmpty(inputLines[i])) i++;
                i++;
                var (graph, zMaxIdx) = ExtractGraphFromInput(inputLines, i);


                Dictionary<string, bool> yAllTrueInput = BuildYAllTrueInput(zMaxIdx);
                string expectedSumXAllFalseAndYAllTrue = String.Concat( '0', new String(TRUE_AS_CHAR, zMaxIdx));

                int remainingSwitches = MAX_NUM_OF_SWITCHES;
                while (remainingSwitches > 0 ) {

                    string res = SolveTheCircuit(zMaxIdx, graph, new(yAllTrueInput)).StringValue;
                    string firstWrongZxx = CheckResult(res, expectedSumXAllFalseAndYAllTrue);
                    if (String.IsNullOrEmpty(firstWrongZxx) && remainingSwitches > 0) throw new ImpossibleException();
                    DisplayGraph(graph, zMaxIdx, firstWrongZxx);

                    var (gateA, gateB) = ProceedSwitching(graph);
                    ans.Add(gateA);
                    ans.Add(gateB);
                    remainingSwitches--;
                    Console.WriteLine(remainingSwitches + " switches remaining.");

                }

                return SortAns(ans);


            }
        }

       
        private static (Dictionary<string, Gate> graph, short zMaxIdx) ExtractGraphFromInput(string[] inputLines, int i)
        {

            (Dictionary<string, Gate> graph, short zMaxIdx) ans = BuildGraph(inputLines, i, OUTPUTGATE_CHAR);
            if (ans.zMaxIdx < 0) throw new Exception($"At least one output gate is required ({OUTPUTGATE_CHAR}**)");
            if (ans.zMaxIdx > 99) throw new Exception($"Too many output gates ({OUTPUTGATE_CHAR}**)");

            return ans;

        }

        private static (short ZMaxIdx, Dictionary<string, Gate> Graph, Dictionary<string, bool> GatesOutput) HandleInput(string input)
        {

            int i = 0;
            string[] inputLines = input.Split("\r\n");
            Dictionary<string, bool> gatesOutput = BuildGatesOutput(inputLines, ref i);

            var (graph, zMaxIdx) = ExtractGraphFromInput(inputLines, i);

            return (zMaxIdx, graph, gatesOutput);
        }

        private static (ulong Value, string StringValue) SolveTheCircuit(short zMaxIdx, Dictionary<string, Gate> graph, Dictionary<string, bool> gatesOutput)
        {

            ulong ans = 0;
            char[] arr = new char[zMaxIdx + 1];

            for (ushort currOutputGate = 0, arrId = (ushort)(arr.Length - 1); currOutputGate <= zMaxIdx; currOutputGate++, arrId--)
            {
                string outputGateName = OUTPUTGATE_CHAR + currOutputGate.ToString("D2");
                bool val = ExploreGraph(graph, gatesOutput, outputGateName);
                arr[arrId] = val ? TRUE_AS_CHAR : '0';
                if (!val) continue;

                ans += (1uL << currOutputGate);
            }


            return (ans, new String(arr));

        }

        private enum GateType
        {
            AND,
            OR,
            XOR
        }

        private static class Gen
        {

            private static int _gateID;
            public static int GateID
            {
                get
                {
                    _gateID++;
                    return _gateID;
                }
                private set { }
            }
        }

        private class Gate(string inputLeft, GateType gateType, string inputRight)
        {

            public int GateID { get; private set; } = Gen.GateID;
            public string InputLeft { get; private set; } = inputLeft;
            public string InputRight { get; private set; } = inputRight;
            public GateType Operation { get; private set; } = gateType;

            public Gate(Gate other) : this(other.InputLeft, other.Operation, other.InputRight) { }

        }

        private static Dictionary<string, bool> BuildGatesOutput(string[] lines, ref int i)
        {

            Dictionary<string, bool> gatesOutput = [];

            while (i < lines.Length && !String.IsNullOrEmpty(lines[i]))
            {
                string line = lines[i];
                string[] tmp = line.Split(": ");
                string key = tmp[0];
                string val = tmp[1];
                gatesOutput.Add(key, val == TRUE_AS_CHAR.ToString());
                i++;
            }
            i++;

            return gatesOutput;
        }

        private static (Dictionary<string, Gate> graph, short zMaxIdx) BuildGraph(string[] lines, int i, char resChar)
        {

            Dictionary<string, Gate> gatesRequiredInputs = [];
            short zMaxIdx = 0;

            while (i < lines.Length)
            {

                string operation = lines[i];
                string[] tmp = operation.Split(' ');
                string inputA = tmp[0];
                if (!Enum.TryParse(tmp[1], out GateType gate)) throw new NotHandledException(tmp[1]);
                string inputB = tmp[2];
                string output = tmp[4];

                if (output[0] == resChar)
                {
                    short id = short.Parse(output[1..]);
                    if (id > zMaxIdx) zMaxIdx = id;
                }

                gatesRequiredInputs.Add(output, new(inputA, gate, inputB));

                i++;

            }

            return (gatesRequiredInputs, zMaxIdx);



        }

        private static bool ExploreGraph(Dictionary<string, Gate> graph, Dictionary<string, bool> gatesOuput, string currGate, HashSet<string>? seen = null)
        {

            seen ??= [];

            if (gatesOuput.ContainsKey(currGate))
                return gatesOuput[currGate];

            if (seen.Contains(currGate))
            {
                throw new ImpossibleException();
            }

            seen.Add(currGate);

            Gate data = graph[currGate];

            bool leftVal;
            if (gatesOuput.ContainsKey(data.InputLeft))
            {
                leftVal = gatesOuput[data.InputLeft];
            }
            else
            {
                leftVal = ExploreGraph(graph, gatesOuput, data.InputLeft, seen);
            }

            bool rightVal;
            if (gatesOuput.ContainsKey(data.InputRight))
            {
                rightVal = gatesOuput[data.InputRight];
            }
            else
            {
                rightVal = ExploreGraph(graph, gatesOuput, data.InputRight, seen);
            }

            bool currRes = Do(leftVal, data.Operation, rightVal);
            gatesOuput.Add(currGate, currRes);

            return currRes;

        }



        private static bool Do(bool a, GateType gate, bool b)
        {

            return gate switch
            {
                GateType.OR => a == true || b == true,
                GateType.AND => a != false && b != false,
                GateType.XOR => a != b,
                _ => throw new NotHandledException(),
            };
        }





    }
}
