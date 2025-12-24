namespace Kemar.WHL.Model.Common
{
    public enum ResultCode
    {
        Success = 200,
        SuccessfullyCreated = 201,
        SuccessfullyUpdated = 202,
        Invalid = 400,
        NotAllowed = 401,
        Unauthorized = 403,
        DuplicateRecord = 409,
        RecordNotFound = 404,   
        ExceptionThrown = 500
    }
}      