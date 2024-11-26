using System.Security.Cryptography;
using System.Text;
using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OtpNet;

namespace Infrastructure.Implementations.Services;

public class AppUserManager(
  IUserStore<AppUser> store,
  IOptions<IdentityOptions> optionsAccessor,
  IPasswordHasher<AppUser> passwordHasher,
  IEnumerable<IUserValidator<AppUser>> userValidators,
  IEnumerable<IPasswordValidator<AppUser>> passwordValidators,
  ILookupNormalizer keyNormalizer,
  IdentityErrorDescriber errors,
  IServiceProvider services,
  ILogger<UserManager<AppUser>> logger)
  : UserManager<AppUser>(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer,
    errors, services, logger) {
  // Generate OTP
  public async Task<string> GenerateOtpAsync(AppUser user) {
    var otp = GenerateOtp();
    user.OtpCode = otp;
    user.OtpExpirationTime = DateTime.UtcNow.AddMinutes(2);
    await UpdateAsync(user);
    return otp;
  }

  // Validate OTP
  public Task<bool> ValidateOtpAsync(AppUser user, string otp) {
    return Task.FromResult(user.OtpCode == otp && !IsOtpExpired(user));
  }

  // Generate TOTP Secret
  private static string GenerateTotpSecret(int length = 20) {
    var key = new byte[length];
    using (var rng = RandomNumberGenerator.Create()) {
      rng.GetBytes(key);
    }
    return Base32Encoding.ToString(key);
  }

  // Generate TOTP
  public static string GenerateTotpAsync(AppUser user) {
    return GenerateTotp(user.TotpSecret);
  }

  // Validate TOTP
  public static bool ValidateTotpAsync(AppUser user, string totpCode) {
    return VerifyTotp(user.TotpSecret, totpCode);
  }

  // Helper methods to generate OTP and TOTP
  private static string GenerateOtp(int length = 6) {
    var otpBytes = new byte[length];
    using var rng = RandomNumberGenerator.Create();
    rng.GetBytes(otpBytes);

    var otp = new StringBuilder(length);
    foreach (var byteValue in otpBytes) {
      // Convert to 0-9
      otp.Append(byteValue % 10);
    }

    return otp.ToString();
  }

  private static string GenerateTotp(string secret, int step = 30, int totpSize = 6) {
    var key = Base32Encoding.ToBytes(secret);
    var totp = new Totp(key, step, OtpHashMode.Sha1, totpSize);

    return totp.ComputeTotp();
  }

  private static bool VerifyTotp(string secret, string totpCode, int step = 30, int totpSize = 6) {
    var key = Base32Encoding.ToBytes(secret);
    var totp = new Totp(key, step, OtpHashMode.Sha1, totpSize);

    return totp.VerifyTotp(totpCode, out _, new VerificationWindow(previous: 1, future: 1));
  }

  private static bool IsOtpExpired(AppUser user) {
    var now = DateTime.UtcNow;
    return user.OtpExpirationTime < now;
  }
}