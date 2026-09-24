namespace RoomReserve.API.Models.Common
{
    public sealed record ResultError(int StatusCode, string Code, string Title, string Message)
    {
        public static ResultError BadRequest(string message, string code = "bad_request", string title = "Bad Request")
            => new(StatusCodes.Status400BadRequest, code, title, message);

        public static ResultError NotFound(string message, string code = "not_found", string title = "Not Found")
            => new(StatusCodes.Status404NotFound, code, title, message);

        public static ResultError Conflict(string message, string code = "conflict", string title = "Conflict")
            => new(StatusCodes.Status409Conflict, code, title, message);

        public static ResultError Forbidden(string message, string code = "forbidden", string title = "Forbidden")
            => new(StatusCodes.Status403Forbidden, code, title, message);
    }

    public sealed record Result
    {
        private Result(bool isSuccess, ResultError? error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public bool IsSuccess { get; }

        public ResultError? Error { get; }

        public static Result Success() => new(true, null);

        public static Result Failure(ResultError error) => new(false, error);
    }

    public sealed record Result<T>
    {
        private Result(bool isSuccess, T? value, ResultError? error)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
        }

        public bool IsSuccess { get; }

        public T? Value { get; }

        public ResultError? Error { get; }

        public static Result<T> Success(T value) => new(true, value, null);

        public static Result<T> Failure(ResultError error) => new(false, default, error);
    }
}