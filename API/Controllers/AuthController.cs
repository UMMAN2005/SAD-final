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

namespace API.Controllers;

public partial class AuthController(
  UserManager<AppUser> userManager,
  IConfiguration config,
  IWebHostEnvironment env
  )
  : BaseApiController {

  [HttpPost("login")]
  public async Task<IActionResult> Login(LoginDto loginDto) {
    var user = await userManager.FindByEmailAsync(loginDto.Email);

    if (user == null || !await userManager.CheckPasswordAsync(user, loginDto.Password!))
      return Unauthorized(new BaseResponse(401, "Invalid email or password", null, []));

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

  [HttpPost("register")]
  public async Task<IActionResult> Register(RegisterDto registerDto) {
    string? uploadedFilePath = null;
    using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
    try {
      var user = new AppUser {
        Email = registerDto.Email,
        UserName = registerDto.UserName
      };

      if (registerDto.Avatar != null)
        uploadedFilePath = FileManager.Save(registerDto.Avatar, env.WebRootPath, "images/users");

      user.AvatarUrl = uploadedFilePath ?? "default.png";

      var result = await userManager.CreateAsync(user, registerDto.Password);

      if (!result.Succeeded) {
        var errors = string.Join(", ", result.Errors.Select(x => x.Description));
        return BadRequest(new BaseResponse(400, errors, null, []));
      }

      scope.Complete();

      return Ok(new BaseResponse(200, "Registered successfully!", new { user.Id }, []));
    }
    catch {

      if (!string.IsNullOrEmpty(uploadedFilePath)) {
        FileManager.Delete(env.WebRootPath, "images/users", uploadedFilePath);
      }

      throw;
    }
  }
}
