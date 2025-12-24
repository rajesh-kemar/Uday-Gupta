using Microsoft.AspNetCore.Mvc;
using Kemar.WHL.Model.Common;

namespace Kemar.WHL.API.Common
{
    public static class CommonHelper
    {
        // For ResultModel based services (Used in some modules)
        public static IActionResult ReturnActionResultByStatus(ResultModel result, ControllerBase controller)
        {
            if (result == null)
                return controller.NotFound(result);

            return result.StatusCode switch
            {
                ResultCode.SuccessfullyCreated => controller.Created("", result),
                ResultCode.SuccessfullyUpdated => controller.Ok(result),
                ResultCode.Success => controller.Ok(result),
                ResultCode.Unauthorized => controller.Unauthorized(result),
                ResultCode.DuplicateRecord => controller.Conflict(result),
                ResultCode.RecordNotFound => controller.NotFound(result),
                ResultCode.NotAllowed => controller.BadRequest(result),
                ResultCode.Invalid => controller.BadRequest(result),
                ResultCode.ExceptionThrown => controller.StatusCode(500, result),
                _ => controller.BadRequest(result)
            };
        }


        // For your existing modules that return:
        // bool, ResponseDTO, List<ResponseDTO>
        public static IActionResult ReturnActionResult(object? result, ControllerBase controller)
        {
            if (result == null)
                return controller.NotFound(new { Message = "Record not found" });

            // Boolean result (Delete)
            if (result is bool booleanResult)
            {
                return booleanResult
                    ? controller.Ok(new { Message = "Deleted successfully" })
                    : controller.BadRequest(new { Message = "Delete failed" });
            }

            // List result (GetAll)
            if (result is IEnumerable<object> listResult)
                return controller.Ok(listResult);

            // Single object result (Add/Update/GetById)
            return controller.Ok(result);
        }
    }
}