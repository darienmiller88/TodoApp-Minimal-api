namespace api.v1.Models;

public class ServiceResult<T>{
    public string Message { get; set; }
    public int StatusCode { get; set; }
    public T? Data { get; set; }

    public ServiceResult(string message, int statusCode, T? data){
        Message = message;
        StatusCode = statusCode;
        Data = data;
    }
}