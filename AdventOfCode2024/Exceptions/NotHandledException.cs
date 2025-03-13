
namespace AdventOfCode2024.Exceptions
{
    internal class NotHandledException : Exception
    {
        public NotHandledException(string msg):base(msg) { }
        public NotHandledException():base() { }

    }

}
