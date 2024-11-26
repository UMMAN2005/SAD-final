using Core.Entities;
using FluentValidation;

namespace API.Dtos;

public record LoginDto(
  string Email, string? Password, bool RememberMe, string? Otp
);

public record RegisterDto(
   string Email, Gender Gender, DateTime Birthday, string UserName, string Password, string ConfirmPassword, IFormFile? Avatar
);

public record ResendOtpDto(
  string Email
);


public class LoginDtoValid : AbstractValidator<LoginDto> {
  public LoginDtoValid() {
    RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(50);
    RuleFor(x => x.Password).NotEmpty();
    RuleFor(x => x.Otp).MaximumLength(6);
  }
}

public class RegisterDtoValid : AbstractValidator<RegisterDto> {
  public RegisterDtoValid() {
    RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(50);
    RuleFor(x => x.UserName).NotEmpty().MaximumLength(50);
    RuleFor(x => x.Password).NotEmpty();
    RuleFor(x => x.ConfirmPassword).Equal(x => x.Password);
  }
}

public class ResendOtpDtoValid : AbstractValidator<ResendOtpDto> {
  public ResendOtpDtoValid() {
    RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(50);
  }
}
