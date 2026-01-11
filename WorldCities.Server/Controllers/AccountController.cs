using Microsoft.AspNetCore.Mvc;
using WorldCities.Server.Data.Models;
 using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using WorldCities.Server.Data;
using Microsoft.AspNetCore.Identity.Data;


namespace WorldCities.Server.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        public readonly ApplicationDbContext _context;
        public readonly UserManager<ApplicationUser> _userManager;
        public readonly JwtHandler _jwtHandler;

        public AccountController (
            ApplicationDbContext context, UserManager<ApplicationUser> userManager, JwtHandler jwtHandler
            )
        {
            _context = context;
            _userManager = userManager;
            _jwtHandler = jwtHandler;
         }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(ApiLoginRequest loginRequest)
        {
            var user = await _userManager.FindByNameAsync(loginRequest.Email);
            if(user == null  || !await _userManager.CheckPasswordAsync(user, loginRequest.Password))
                return Unauthorized(new ApiLoginResult() {
                    Success = false,
                    Message= "Invalid mail or password."
            });
            var secToken = await _jwtHandler.GetTokenAsync(user);
            var jwt = new JwtSecurityTokenHandler().WriteToken(secToken);
            return Ok(new ApiLoginResult() { 
                Message="Login successful",
                Success = true,
            Token = jwt
            });

        }

    }
}
