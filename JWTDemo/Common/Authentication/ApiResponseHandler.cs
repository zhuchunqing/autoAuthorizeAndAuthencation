using Common.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace Common.Authentication
{
    public class ApiResponseHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IUser _user;

        public ApiResponseHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IUser user) : base(options, logger, encoder, clock)
        {
            _user = user;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return Task.FromResult(AuthenticateResult.Fail("Authorization header is not specified."));

            }
            var authHeader = Request.Headers["Authorization"].ToString();

            if (!authHeader.StartsWith("Bearer "))
            {
                return Task.FromResult(

                    AuthenticateResult.Fail("Authorization header value is not in a correct format"));
            }

            var base64EncodedValue = authHeader["Bearer ".Length..];
            //var userNamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedValue));
            //var userName = userNamePassword.Split(':')[0];
            //var password = userNamePassword.Split(':')[1];
            //var user = User.FirstOrDefault(u => u.UserName == userName && u.Password == password);

            //if (user == null)

            //{

            //    return Task.FromResult(AuthenticateResult.Fail("Invalid username or password."));

            //}
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,"1213"),
                //new Claim(ClaimTypes.Role, string.Join(',', user.Roles)),
                //new Claim(ClaimTypes.UserData, user.Age.ToString())
            };

            var claimsPrincipal =

                new ClaimsPrincipal(new ClaimsIdentity(
                    claims,
                    "Bearer",
                    ClaimTypes.NameIdentifier, ClaimTypes.Role));

            var ticket = new AuthenticationTicket(claimsPrincipal, new AuthenticationProperties
            {
                IsPersistent = false
            }, "Bearer");
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
        

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.ContentType = "application/json";
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            await Response.WriteAsync(JsonConvert.SerializeObject(new ApiResponse(StatusCode.CODE401).MessageModel));
        }

        protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            Response.ContentType = "application/json";
            if (_user.MessageModel != null)
            {
                Response.StatusCode = _user.MessageModel.status;
                await Response.WriteAsync(JsonConvert.SerializeObject(_user.MessageModel));
            }
            else
            {
                Response.StatusCode = StatusCodes.Status403Forbidden;
                await Response.WriteAsync(JsonConvert.SerializeObject(new ApiResponse(StatusCode.CODE403).MessageModel));
            }
        }
    }
}
