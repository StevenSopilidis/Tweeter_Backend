namespace Application.Core
{
    public class AppException
    {
        public string _error { get; set; }
        public int _statusCode { get; set; }
        public string _stackTrace { get; set; }
        public AppException(int StatusCode, string Error, string StackTrace = null)
        {
            _error = Error;
            _statusCode = StatusCode;
            _stackTrace = StackTrace;
        }
    }
}