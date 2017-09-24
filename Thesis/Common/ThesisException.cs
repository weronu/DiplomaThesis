using System;

namespace Common
{
    public class ThesisException : Exception
    {
        public ThesisException()
        {
        }

        public ThesisException(string message) : base(message)
        {
        }

        public ThesisException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}