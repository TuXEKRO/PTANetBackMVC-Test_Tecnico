namespace interviewProject.Utils
{
    public class Result
    {
        public string Error { get; }
        public bool IsSuccess => Error == null;

        protected Result(string error)
        {
            Error = error;
        }

        public static Result Success() => new Result(null);
        public static Result Failure(string error) => new Result(error);
    }

    public class Result<T> : Result
    {
        public T Value { get; }

        protected Result(T value, string error) : base(error)
        {
            Value = value;
        }

        public static new Result<T> Success(T value) => new Result<T>(value, null);
        public static new Result<T> Failure(string error) => new Result<T>(default, error);
    }

    public static class ResultExtensions
    {
        public static async Task<Result> TryAsync(Func<Task> func)
        {
            try
            {
                await func();
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public static async Task<Result<T>> TryAsync<T>(Func<Task<T>> func)
        {
            try
            {
                var result = await func();
                return Result<T>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<T>.Failure(ex.Message);
            }
        }

        public static async Task<Result<U>> Bind<T, U>(this Task<Result<T>> resultTask, Func<T, Task<Result<U>>> func)
        {
            var result = await resultTask;
            return result.IsSuccess ? await func(result.Value) : Result<U>.Failure(result.Error);
        }

        public static Result<U> Bind<T, U>(this Result<T> result, Func<T, Result<U>> func)
        {
            return result.IsSuccess ? func(result.Value) : Result<U>.Failure(result.Error);
        }

    }
}
