using System;

namespace WIMCore.Exceptions
{
    public class WimIntegrityException : Exception
    {
        public WimIntegrityExceptionType Type { get; }

        public WimIntegrityException(WimIntegrityExceptionType type)
        {
            Type = type;
        }
    }
}