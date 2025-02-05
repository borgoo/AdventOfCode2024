using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Utils
{
    internal abstract class ProgressBar
    {
        public bool Enabled = true;

        public abstract void Show();
        public abstract void Terminate();
    }
}
