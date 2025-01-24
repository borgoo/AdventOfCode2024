using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Exceptions
{
    internal class DayNotFoundException : FileNotFoundException
    {
        public DayNotFoundException():base() { }
        public DayNotFoundException(string message) : base(message) { }
    }
}
