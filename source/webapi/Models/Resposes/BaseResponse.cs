public class BaseResponse
{
    public BaseResponse(bool isSuccess, string? errorMessage = null, object? data = null)
    {
        Status = isSuccess ? "SUCCESS" : "FAIL";
        Data = data;
        ErrorMessage = errorMessage;
    }


    public string Status { get; set; }
    public object? Data { get; set; }
    public string? ErrorMessage { get; set; }
}