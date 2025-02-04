using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Exceptions
{
    internal class LoopException(string? message = "Infinite loop.") : Exception(message)
    {

    }


}
