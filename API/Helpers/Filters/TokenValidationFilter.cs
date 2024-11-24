using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;

namespace API.Helpers.Filters {
  public class TokenValidationFilter : IActionFilter {
    public void OnActionExecuting(ActionExecutingContext context) {
      var request = context.HttpContext.Request;

      if (!request.Headers.TryGetValue("Authorization", out var tokenHeader) || string.IsNullOrEmpty(tokenHeader)) {
        context.Result = new UnauthorizedResult();
        return;
      }

      var token = tokenHeader.ToString();
      if (!token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)) {
        context.Result = new UnauthorizedResult();
        return;
      }

      token = token.Substring("Bearer ".Length).Trim();

      try {
        var jwtHandler = new JwtSecurityTokenHandler();
        if (jwtHandler.CanReadToken(token)) return;
        context.Result = new UnauthorizedResult();
      }
      catch (Exception) {
        context.Result = new UnauthorizedResult();
      }
    }

    public void OnActionExecuted(ActionExecutedContext context) {
    }
  }
}