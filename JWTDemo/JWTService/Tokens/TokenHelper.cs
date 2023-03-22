
using Common;
using Common.Models;
using Common.Utils;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JWTService.Tokens
{
    public enum TokenType
    {
        AccessToken = 1,
        RefreshToken = 2
    }
    public class TokenHelper : ITokenHelper
    {
        private IOptions<JWTConfig> _options;
        public TokenHelper(IOptions<JWTConfig> options)
        {
            _options = options;
        }

        public Token CreateAccessToken(User user)
        {
            Claim[] claims = new Claim[] { new Claim(ClaimTypes.NameIdentifier, user.Id), new Claim(ClaimTypes.Name, user.UserName) };

            return CreateToken(claims, TokenType.AccessToken);
        }

        public ComplexToken CreateToken(User user)
        {
            Claim[] claims = new Claim[] { new Claim(ClaimTypes.NameIdentifier, user.Id), new Claim(ClaimTypes.Name, user.UserName) };

            //下面对code为001的张三添加了一个Claim，用于测试在Token中存储用户的角色信息，对应测试在FlyLolo.JWT.API的BookController的Put方法，若用不到可删除
            if (user.Id.Equals("001"))
            {
                claims = claims.Append(new Claim(ClaimTypes.Role, "TestPutBookRole")).ToArray();
            }

            return CreateToken(claims);
        }
        //private string GenerateToken(string userId, string userName)
        //{
        //    string secret = "QWEqwe!@#123QWEqwe!@#123QWEqwe!@#123QWEqwe!@";
        //    string issuer = "xiaozhu";
        //    string audience = "jwtdemo";
        //    var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
        //    var siginsCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        //    var claims = new Claim[] { new Claim("userId", userId), new Claim("userName", userName) };
        //    SecurityToken securityToken = new JwtSecurityToken(
        //        issuer: issuer,
        //        audience: audience,
        //        claims: claims,
        //        signingCredentials: siginsCredentials,
        //        expires: DateTime.Now.AddMinutes(30)
        //        );
        //    return new JwtSecurityTokenHandler().WriteToken(securityToken);
        //}
        public ComplexToken CreateToken(Claim[] claims)
        {
            return new ComplexToken { AccessToken = CreateToken(claims, TokenType.AccessToken), RefreshToken = CreateToken(claims, TokenType.RefreshToken) };
        }

        /// <summary>
        /// 用于创建AccessToken和RefreshToken。
        /// 这里AccessToken和RefreshToken只是过期时间不同，【实际项目】中二者的claims内容可能会不同。
        /// 因为RefreshToken只是用于刷新AccessToken，其内容可以简单一些。
        /// 而AccessToken可能会附加一些其他的Claim。
        /// </summary>
        /// <param name="claims"></param>
        /// <param name="tokenType"></param>
        /// <returns></returns>
        private Token CreateToken(Claim[] claims, TokenType tokenType)
        {
            var now = DateTime.Now;
            var expires = now.Add(TimeSpan.FromMinutes(tokenType.Equals(TokenType.AccessToken) ? 120 : 1440));
            string secret = "QWEqwe!@#123QWEqwe!@#123QWEqwe!@#123QWEqwe!@";
            string issuer = "xiaozhu";
            string audience = "jwtdemo";
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
            var siginsCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            //var claims = new Claim[] { new Claim("userId", userId), new Claim("userName", userName) };
            SecurityToken token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                signingCredentials: siginsCredentials,
                expires: expires
                );
            return new Token { TokenContent = new JwtSecurityTokenHandler().WriteToken(token), Expires = expires };
        }

        public MessageModel<Token> RefreshToken(ClaimsPrincipal claimsPrincipal)
        {
            
            //1.通过用户Id查询出refreshToken(refreshToken存在数据库或者redis中)
            var refreshToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjAwMSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiIxMjM0NTYiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJUZXN0UHV0Qm9va1JvbGUiLCJleHAiOjE2Nzk0NzEzNTEsImlzcyI6InhpYW96aHUiLCJhdWQiOiJqd3RkZW1vIn0.9H_RYKHa6NpSDTR9frNg7y3Z2DDcSpSqRJJwdCA9sQo";
            //2.校验RefreshToken是否过期，如果过期则跳转到登录页面，如果没有过期则重新生成Token
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken JwtSecurityToken = jwtSecurityTokenHandler.ReadJwtToken(refreshToken);
            var exparin = Convert.ToInt32(JwtSecurityToken.Claims.FirstOrDefault(a => a.Type == "exp").Value);
            long now = DateTimeConvert.ToUnixTimestampBySeconds(DateTime.UtcNow);

            var code = Convert.ToString(JwtSecurityToken.Claims.FirstOrDefault(a => a.Type ==ClaimTypes.NameIdentifier).Value);
            if (exparin > now)
            {

                //3.生成Token并返回
                Token token= CreateAccessToken(TemporaryData.GetUser(code));
                return new MessageModel<Token>()
                {
                    success = true,
                    msg = "获取成功",
                    response = token
                };
            }
            else 
            {
                //4.refreshtoken过期跳转到登录页面
                //5.删除Redis中refreshToken数据
                return new MessageModel<Token>()
                {
                    success = false,
                    msg = "获取失败",
                    response = new Token()
                };
            }
        }

        public bool VerifyToken() 
        {

            return true;
        }
    }
}
