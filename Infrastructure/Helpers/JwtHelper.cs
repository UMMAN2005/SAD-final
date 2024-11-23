using System.IdentityModel.Tokens.Jwt;

namespace Infrastructure.Helpers;
public static class JwtHelper {
  public static string? GetClaimFromJwt(string token, string claimType) {
    var handler = new JwtSecurityTokenHandler();
    var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
    return jsonToken?.Claims.First(claim => claim.Type == claimType).Value;
  }
}
