namespace SAWA.API.Healper
{
    public class ResponseAPI<T>
    {
        public ResponseAPI(int statusCode, string message = null, T data = default)
        {
            StatusCode = statusCode;
            Message = message ?? GetMessageFromStatusCode(statusCode);
            Data = data;
        }

        private static string GetMessageFromStatusCode(int statusCode) =>
            statusCode switch
            {
                200 => "Success",
                201 => "Created",
                204 => "No Content",
                400 => "Bad Request",
                401 => "Unauthorized",
                403 => "Forbidden",
                404 => "Not Found",
                500 => "Internal Server Error",
                _ => "Unknown Error"
            };

        public int StatusCode { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public static ResponseAPI<T> Success(T data, string message = null, int statusCode = 200)
        {
            return new ResponseAPI<T>(statusCode, message, data);
        }

        public static ResponseAPI<T> Error(string message, int statusCode = 400)
        {
            return new ResponseAPI<T>(statusCode, message, default);
        }
    }
}
