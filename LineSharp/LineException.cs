using System;
using System.Linq;
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

        private string Describe()
        {
            if (ErrorResponse?.Details != null)
            {
                var details =
                    ErrorResponse.Details
                    .Select(d => $"  {d.Property}: {d.Message}");

                var detailsStr = string.Join("\n", details);

                return string.Join("\n",
                    base.Message,
                    ErrorResponse.Message,
                    detailsStr);
            }
            else if (ErrorResponse != null)
            {
                return string.Join("\n", base.Message, ErrorResponse.Message);
            }
            else
            {
                return base.Message;
            }
        }

        public override string Message => Describe();
    }
}