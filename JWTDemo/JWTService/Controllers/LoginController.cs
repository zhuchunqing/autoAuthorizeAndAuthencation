using Common.Models;
using Common.Authorize;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using Newtonsoft.Json.Linq;
using JWTService.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace Common.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private PermissionRequirement _permissionRequirement;
        private ITokenHelper _tokenHelper;
        public LoginController(ITokenHelper tokenHelper, PermissionRequirement permissionRequirement) 
        {
            _tokenHelper= tokenHelper;
            _permissionRequirement = permissionRequirement;
        }
        [HttpPost("Login")]
        public IActionResult Login(User user)
        {
            if (user.UserName == "123456" && user.Password == "123456")
            {
                _permissionRequirement.Permissions = new List<PermissionData>()
                {
                    new PermissionData() {UserId="123456",Url="api/Login/Login"},
                    new PermissionData() {UserId="123456",Url="api/Home/GetWeatherForecast"}
                };
                ComplexToken token = _tokenHelper.CreateToken(user);
                return Ok(token);
            }
            else 
            {
                return BadRequest();
            }
        }
        [HttpPost("GetTokenToRefreshToken")]
        //[Authorize]
        public IActionResult GetTokenToRefreshToken()
        {
            return Ok(_tokenHelper.RefreshToken(HttpContext.User));
        }

    }
}
