using System;

namespace WIMCore
{
    public class WimNotValidException : Exception
    {
        public ErrorType Type { get; }

        public WimNotValidException(ErrorType type)
        {
            Type = type;
        }

        public enum ErrorType
        {
            InvalidSize,
            InvalidMagic
        }
    }
}