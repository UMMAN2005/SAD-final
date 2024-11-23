using FluentValidation;

namespace API.Dtos;

public record LoginDto(
  string Email, string? Password, bool RememberMe
);

public record RegisterDto(
   string Email, string UserName, string Password, string ConfirmPassword, IFormFile? Avatar
);


public class LoginDtoValid : AbstractValidator<LoginDto> {
  public LoginDtoValid() {
    RuleFor(x => x.Email).NotEmpty().EmailAddress();
    RuleFor(x => x.Password).NotEmpty();
  }
}

public class RegisterDtoValid : AbstractValidator<RegisterDto> {
  public RegisterDtoValid() {
    RuleFor(x => x.Email).NotEmpty().EmailAddress();
    RuleFor(x => x.UserName).NotEmpty();
    RuleFor(x => x.Password).NotEmpty();
    RuleFor(x => x.ConfirmPassword).Equal(x => x.Password);
  }
}
