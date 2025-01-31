using AdventOfCode2024.Exceptions;

namespace AdventOfCode2024.Day9
{
    internal class Day9 : Day
    {
        private static readonly bool _debugActive = false;

       
        protected override object SolveA(string input)
        {
            return CompactMemory(input);
        }

        protected override object SolveB(string input)
        {
            return CompactMemoryMovingEntireFiles(input);
        }

        private static long CompactMemory(string input)
        {

            long checkSum = 0;
            int n = input.Length;

            int currCheckSumI = 0;
            int left = 0;
            int right = (n - 1) % 2 == 0 ? n - 1 : n - 2;
            int remainingToBeRefactored = 0;
            int rightId = 0;
            while (left < n && left < right)
            {

                bool isFile = left % 2 == 0;

                if (isFile)
                {
                    int fileSize = input[left] - '0';
                    int id = left / 2;
                    for (int i = 0; i < fileSize; i++)
                    {
                        checkSum += (id * currCheckSumI);
                        currCheckSumI++;
                    }

                }
                else
                {
                    int emptySize = input[left] - '0';

                    for (int i = 0; i < emptySize && left < right; i++)
                    {

                        if (remainingToBeRefactored < 1)
                        {
                            remainingToBeRefactored = input[right] - '0';
                            rightId = right / 2;

                        }

                        checkSum += (rightId * currCheckSumI);
                        currCheckSumI++;
                        remainingToBeRefactored--;
                        if (remainingToBeRefactored == 0) right -= 2;


                    }

                }

                left++;

            }

            for (int i = 0; i < remainingToBeRefactored; i++)
            {
                checkSum += (rightId * currCheckSumI);
                currCheckSumI++;
            }

            return checkSum;

        }



        private static long CheckSumIncrement(int fileId, int fileDim, int idx)
        {

            long gaussSumEnd = idx + fileDim - 1;
            long gaussSumToRemoveUntil = idx - 1;

            long gaussSum = (gaussSumEnd * (gaussSumEnd + 1)) / 2;
            long gaussSumToRemove = (gaussSumToRemoveUntil * (gaussSumToRemoveUntil + 1)) / 2;
            long gaussDifference = gaussSum - gaussSumToRemove;

            long checkSumContributeForCurrentFile = fileId * (gaussDifference);

            return checkSumContributeForCurrentFile;

        }

        private static void Debug(string fileId, int fileDim)
        {
            for (int i = 0; i < fileDim; i++) Console.Write(fileId);
        }

        private static long CompactMemoryMovingEntireFiles(string input)
        {
            long checkSum = 0;
            int n = input.Length;
            int lasFileIdx = n % 2 == 0 ? n - 2 : n - 1;
            int numOfFiles = lasFileIdx + 1 / 2;

            LinkedList<int> availableFilesIds = new();

            for (int i = 0; i < n; i+=2) {
                
                if (i % 2 == 0) {
                    availableFilesIds.AddLast(i);
                }
            }

            int currentResultIdx = 0;

            for (int i = 0; i < n && availableFilesIds.Count > 0; i++)
            {

                int elemId = i / 2;
                bool isFile = i % 2 == 0 && availableFilesIds.Contains(elemId*2);

                if (isFile)
                {

                    int fileId = elemId;
                    int fileDim = input[i] - '0';

                    if (_debugActive) Debug(fileId.ToString(), fileDim);
                    checkSum += CheckSumIncrement(fileId, fileDim, currentResultIdx);

                    LinkedListNode<int>? currFile = availableFilesIds.Find(fileId*2);
                    _ = currFile ?? throw new NotFoundException();
                    availableFilesIds.Remove(currFile);
                    currentResultIdx += fileDim;

                }
                else
                {
                    int spaceDim = input[i] - '0';

                    int notFilledSpace = FillAvailableSpace(lasFileIdx, input, numOfFiles, spaceDim, availableFilesIds, ref currentResultIdx, ref checkSum);
                    currentResultIdx += notFilledSpace;
                    if (_debugActive) Debug(".", notFilledSpace);

                }

            }



            return checkSum;




        }

        private static int FillAvailableSpace(int lastFileIdx, string input, int numOfFiles, int availableSpace, LinkedList<int> availableFilesIds, ref int currentResultIdx, ref long checkSum)
        {

            if (availableSpace == 0 || availableFilesIds.Count < 1) return availableSpace;

            LinkedListNode<int>? currNode = availableFilesIds.Last;
            while (currNode is not null && availableSpace > 0)
            {
                int fileId = currNode.Value / 2;
                int fileDim = input[currNode.Value] - '0';
                LinkedListNode<int>? prevNode = currNode.Previous;
                if (fileDim <= availableSpace)
                {

                    if (_debugActive) Debug(fileId.ToString(), fileDim);
                    checkSum += CheckSumIncrement(fileId, fileDim, currentResultIdx);
                    availableFilesIds.Remove(currNode);
                    currentResultIdx += fileDim;
                    availableSpace -= fileDim;
                    currNode = availableFilesIds.Last;
                }
                else { 
                    currNode = prevNode;                
                }




            }
           

            return availableSpace;
        }

    }
}
