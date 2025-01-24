using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Exceptions
{
    internal class NotFoundException: Exception
    {
        public NotFoundException():base() { }
        public NotFoundException(string msg):base(msg) { }
    }
}
