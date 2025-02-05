using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Utils
{
    internal class WaitingBar(int millisecondsSleepTime = 500) : ProgressBar
    {

        private const string ANIMATION = "|/-\\";
        private readonly int _millisecondsSleepTime = millisecondsSleepTime;
        private CancellationTokenSource? cts = null;

        private void HandleThreadStart() {

            cts = new();
            ThreadPool.QueueUserWorkItem(new WaitCallback(Run), (cts.Token, _millisecondsSleepTime));

        }

        private void HandleThreadStop() {

            _ = cts ?? throw new NullReferenceException();
            cts.Cancel();
            cts.Dispose();
        }

        public static void Run(object? obj)
        {

            _ = obj ?? throw new NullReferenceException();
            var parameters = ((CancellationToken token, int millisecondsSleepTime))obj;

            int sleep = parameters.millisecondsSleepTime;
            CancellationToken token = parameters.token;

            int i = 0;
            while (!token.IsCancellationRequested)
            {
                Console.CursorLeft = 0;
                Console.WriteLine(ANIMATION[i % ANIMATION.Length]);
                Console.CursorTop = Console.CursorTop - 1;
                i++;
                Thread.Sleep(sleep);
            }
        }

        public override void Terminate()
        {
            HandleThreadStop();
        }

        public override void Show()
        {
            try
            {
                _ = cts ?? throw new NullReferenceException();
                CancellationToken token = cts.Token;
            }
            catch (ObjectDisposedException)
            {
                cts = null;
                HandleThreadStart();
            }
            catch (NullReferenceException) {
                HandleThreadStart();
            }
            
            
        }
    }
}
