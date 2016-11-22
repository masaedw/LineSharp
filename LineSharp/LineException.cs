using System;
using LineSharp.Messages;

namespace LineSharp
{
    public class LineException : Exception
    {
        public ErrorResponse ErrorResponse { get; }

        public LineException(string message)
            : base(message)
        {
        }

        public LineException(string message, ErrorResponse response)
            : base(message)
        {
            ErrorResponse = response;
        }
    }
}
