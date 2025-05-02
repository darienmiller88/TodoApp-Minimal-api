namespace api.v1.Models;

public class ServiceResult<T>{
    public string ErrorMessage { get; set; }
    public int StatusCode { get; set; }
    public T? Data { get; set; }

    public ServiceResult(string errorMessage, int statusCode, T? data){
        ErrorMessage = errorMessage;
        StatusCode = statusCode;
        Data = data;
    }

    public bool IsSucess(){
        return StatusCode >= 200 && StatusCode <= 226;
    }

    public bool IsFailure(){
        return StatusCode >= 400 && StatusCode <= 451;
    }
}