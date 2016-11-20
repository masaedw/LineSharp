namespace LIneSharp.Messages
{
    public class ErrorResponse
    {
        public string message { get; set; }
        public ErrorDetail[] details { get; set; }
    }

    public class ErrorDetail
    {
        public string message { get; set; }
        public string property { get; set; }
    }
}
