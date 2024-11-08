namespace ZonefyDotnet.Helpers
{
    public class Response
    {
        public int Code { get; set; }
        public bool? Success { get; set; }
        public string Message { get; set; }
        public object? ExtraInfo { get; set; }
    }
    public class SuccessResponse<T> : Response
    {
        public SuccessResponse()
        {
            Success = true;
        }
        public T? Data { get; set; }
    }
    public class ErrorResponse<T> : Response
    {
        public T Error { get; set; }
    }

    public class PaginatedResponse<T>
    {
        public IEnumerable<T> Data { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
    }
}
