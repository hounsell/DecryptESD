using System;

namespace WIMCore.Exceptions
{
    public class WimInvalidException : Exception
    {
        public WimInvalidExceptionType Type { get; }

        public WimInvalidException(WimInvalidExceptionType type)
        {
            Type = type;
        }
    }
}