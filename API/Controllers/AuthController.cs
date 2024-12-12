using System.IdentityModel.Tokens.Jwt;
using API.Dtos;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Core.Entities;
using Core.Interfaces.Services;
using Infrastructure.Helpers;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Transactions;
using Infrastructure.Implementations.Services;

namespace API.Controllers;

public class AuthController(AppUserManager userManager, IConfiguration config, IEmailService emailService, IWebHostEnvironment env) : BaseApiController {
  [HttpPost("login")]
  public async Task<IActionResult> Login(LoginDto loginDto) {
    var user = await userManager.FindByEmailAsync(loginDto.Email);

    var response = new BaseResponse(400, "Invalid email or password", null, []);

    if (user == null || !await userManager.CheckPasswordAsync(user, loginDto.Password!))
      return StatusCode(response.StatusCode, response);

    if (user.Provider is AuthProvider.Local) {
      if (!await userManager.CheckPasswordAsync(user, loginDto.Password!))
        return StatusCode(response.StatusCode, response);

      if (!await userManager.IsEmailConfirmedAsync(user)) {
        if (loginDto.Otp == null) {
          return StatusCode(401,
            new BaseResponse(401, $"Email not verified! {user.Email}. Please provide the OTP sent to your email.", null,
              []));
        }

        if (!await userManager.ValidateOtpAsync(user, loginDto.Otp))
          return StatusCode(400, new BaseResponse(400, "Invalid or expired OTP!", null, []));

        user.EmailConfirmed = true;
        await userManager.UpdateAsync(user);
      }
    }
    else {
      user.EmailConfirmed = true;
      await userManager.UpdateAsync(user);
    }

    List<Claim> claims = [
      new Claim(ClaimTypes.NameIdentifier, user!.Id),
      new Claim(ClaimTypes.Name, user.UserName!),
      new Claim(ClaimTypes.Email, user.Email!)
    ];

    var secret = config["JWT:Secret"]!;

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

    var expires = loginDto.RememberMe ? DateTime.Now.AddDays(7) : DateTime.Now.AddDays(1);

    JwtSecurityToken securityToken = new(
      issuer: config["JWT:Issuer"],
      audience: config["JWT:Audience"],
      claims: claims,
      signingCredentials: credentials,
      expires: expires
    );

    var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

    return Ok(new BaseResponse(200, "Logged in successfully", token, []));
  }

  [HttpPost("resend-otp")]
  public async Task<IActionResult> ResendOtp(ResendOtpDto resendOtpDto) {
    var user = await userManager.FindByEmailAsync(resendOtpDto.Email);

    if (user == null)
      return StatusCode(404, new BaseResponse(404, "User not found", null, []));

    if (user.Provider is not AuthProvider.Local)
      return StatusCode(400, new BaseResponse(400, "User is not registered with email", null, []));

    if (user.LastOtpRequestTime.HasValue && user.LastOtpRequestTime.Value.AddMinutes(2) > DateTime.UtcNow) {
      var nextRequestTime = user.LastOtpRequestTime.Value.AddMinutes(2);
      var remainingTime = nextRequestTime.Subtract(DateTime.UtcNow).TotalSeconds;
      return StatusCode(429, new BaseResponse(429, $"You must wait {remainingTime:F1} seconds before requesting a new OTP.", null, []));
    }

    var otp = await userManager.GenerateOtpAsync(user);
    user.LastOtpRequestTime = DateTime.UtcNow;
    await userManager.UpdateAsync(user);
    await emailService.SendAsync(user.Email!, "Email Verification", EmailTemplates.GetVerifyEmailTemplate(otp));

    return Ok(new BaseResponse(200, "OTP has been resent to your email.", null, []));
  }

  [HttpPost("register")]
  public async Task<IActionResult> Register(RegisterDto registerDto) {
    string? uploadedFilePath = null;
    using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
    try {
      var user = new AppUser {
        Email = registerDto.Email,
        UserName = registerDto.UserName ?? Guid.NewGuid().ToString(),
        Birthday = registerDto.Birthday.HasValue
          ? DateTime.SpecifyKind(registerDto.Birthday.Value, DateTimeKind.Utc)
          : DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
        Gender = registerDto.Gender ?? Gender.NotSpecified,
        Provider = AuthProvider.Local
      };

      if (registerDto.Avatar != null)
        uploadedFilePath = FileManager.Save(registerDto.Avatar, env.WebRootPath, "images/users");

      user.AvatarUrl = uploadedFilePath ?? "default.png";

      var result = await userManager.CreateAsync(user, registerDto.Password);

      if (!result.Succeeded) {
        var errors = string.Join(", ", result.Errors.Select(x => x.Description));
        return StatusCode(400, new BaseResponse(400, errors, null, []));
      }

      var otp = await userManager.GenerateOtpAsync(user);
      user.LastOtpRequestTime = DateTime.UtcNow;
      await userManager.UpdateAsync(user);
      await emailService.SendAsync(user.Email!, "Email Verification", EmailTemplates.GetVerifyEmailTemplate(otp));

      scope.Complete();

      return Ok(new BaseResponse(200, "Registered successfully. Please verify your email now.", new { user.Id }, []));
    }
    catch {

      if (!string.IsNullOrEmpty(uploadedFilePath)) {
        FileManager.Delete(env.WebRootPath, "images/users", uploadedFilePath);
      }

      throw;
    }
  }
}
