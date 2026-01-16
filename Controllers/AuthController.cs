using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json;
using WebApi_Angular.Common;
using WebApi_Angular.Models;
using LoginRequest = WebApi_Angular.Models.LoginRequest;

namespace WebApi_Angular.Controllers
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "Auth API")]
    public class AuthController : ControllerBase
    {
        private readonly JWTTokenGenerator _tokenGenerator;

        public AuthController(JWTTokenGenerator tokenGenerator)
        {
            _tokenGenerator = tokenGenerator;
        }

        [HttpPost("login")]
        [Route("api/web/client/{domainModelName}/{parameter?}/login")]
        public IActionResult Login(string domainModelName, [FromBody] JsonElement parameter)
        {
            string UserName = "";
            dynamic paramObject = JsonConvert.DeserializeObject(parameter.ToString());

            if (paramObject.Username == "admin" && paramObject.Password == "Admin022")
            {
                if(paramObject.Username == "admin")
                {
                     UserName = "admin";
                }
                var token = _tokenGenerator.GenerateToken(UserName, "Admin");
                return Ok(new { Token = token });
            }

            return Unauthorized();
        }

    }
}
