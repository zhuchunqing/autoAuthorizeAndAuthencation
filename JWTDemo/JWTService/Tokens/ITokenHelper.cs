using Common;
using Common.Models;
using Newtonsoft.Json.Linq;
using System.Security.Claims;

namespace JWTService.Tokens
{
    public interface ITokenHelper
    {
        ComplexToken CreateToken(User user);
        ComplexToken CreateToken(Claim[] claims);
        MessageModel<Token> RefreshToken(ClaimsPrincipal claimsPrincipal);
    }
}
