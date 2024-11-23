namespace Infrastructure.Helpers;
public class BaseResponse(int statusCode, string? message, object? data, List<RestExceptionError>? errors) {
  public int StatusCode { get; set; } = statusCode;
  public string? Message { get; set; } = message;
  public object? Data { get; set; } = data;
  public List<RestExceptionError>? Errors { get; set; } = errors;
}
