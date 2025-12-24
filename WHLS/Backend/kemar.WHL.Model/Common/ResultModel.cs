namespace Kemar.WHL.Model.Common
{
    public class ResultModel
    {
        public bool IsSuccess { get; set; }
        public ResultCode StatusCode { get; set; }  
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }

        public static ResultModel Success(string message, object? data = null)
        {
            return new ResultModel
            {
                IsSuccess = true,
                StatusCode = ResultCode.Success,
                Message = message,
                Data = data
            };
        }

        public static ResultModel Created(string message, object? data = null)
        {
            return new ResultModel
            {
                IsSuccess = true,
                StatusCode = ResultCode.SuccessfullyCreated,
                Message = message,
                Data = data
            };
        }

        public static ResultModel Updated(string message, object? data = null)
        {
            return new ResultModel
            {
                IsSuccess = true,
                StatusCode = ResultCode.SuccessfullyUpdated,
                Message = message,
                Data = data
            };
        }

        public static ResultModel Invalid(string message)
        {
            return new ResultModel
            {
                IsSuccess = false,
                StatusCode = ResultCode.Invalid,
                Message = message
            };
        }

        public static ResultModel NotFound(string message)
        {
            return new ResultModel
            {
                IsSuccess = false,
                StatusCode = ResultCode.RecordNotFound,
                Message = message
            };
        }

        public static ResultModel Error(string message)
        {
            return new ResultModel
            {
                IsSuccess = false,
                StatusCode = ResultCode.ExceptionThrown,
                Message = message
            };
        }
    }
}