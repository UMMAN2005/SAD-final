namespace Infrastructure.Helpers;

public class RestException : Exception {
  public int Code { get; set; }
  public new string? Message { get; set; }
  public List<RestExceptionError> Errors { get; set; } = [];

  public RestException() { }
  public RestException(int code, string? message) {
    Code = code;
    Message = message;
  }
  public RestException(int code, string errorKey, string errorMessage, string? message = null) {
    Code = code;
    Message = message;
    Errors = [new RestExceptionError(errorKey, errorMessage)];
  }
}

public class RestExceptionError {
  public string? Key { get; set; }
  public string? Message { get; set; }

  public RestExceptionError() { }
  public RestExceptionError(string key, string message) {
    Key = key;
    Message = message;
  }
}