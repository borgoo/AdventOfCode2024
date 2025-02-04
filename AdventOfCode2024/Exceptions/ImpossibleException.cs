using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Exceptions
{
    internal class ImpossibleException(string? message = "Not solvable.") : Exception(message)
    {

    }
}
