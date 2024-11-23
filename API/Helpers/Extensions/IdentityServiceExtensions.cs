using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace API.Helpers.Extensions;

public static class IdentityServiceExtensions {
  public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config) {

    services.AddAuthentication(
        options => {
          options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
          options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
          options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }
      )
      .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Secret"] ?? string.Empty)),
          ValidIssuer = config["JWT:Issuer"],
          ValidAudience = config["JWT:Audience"],
          ValidateIssuerSigningKey = true,
          ValidateIssuer = false,
          ValidateAudience = false
        };
      })
      .AddCookie(opt => {
        opt.Cookie.SecurePolicy = CookieSecurePolicy.None;
      })
      .AddGoogle(opt => {
        opt.ClientId = config["Google:ClientId"]!;
        opt.ClientSecret = config["Google:ClientSecret"]!;
        opt.SaveTokens = true;
        opt.Scope.Add("profile");
        opt.Events.OnCreatingTicket = (context) => {
          var picture = context.User.GetProperty("picture").GetString();
          if (picture != null) context.Identity?.AddClaim(new Claim("picture", picture));
          return Task.CompletedTask;
        };
        opt.Events.OnRedirectToAuthorizationEndpoint = context => {
          context.HttpContext.Response.Redirect(context.RedirectUri);
          return Task.CompletedTask;
        };
      });

    services.AddAuthorization();

    return services;
  }
}