
using WorldCities.Server.Data.Models;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;

namespace WorldCities.Server.Data;

public class JwtHandler
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;

    public JwtHandler(IConfiguration configuration, UserManager<ApplicationUser> userManager) { 
    
    _configuration = configuration;
    _userManager =  userManager;
            }

    public async Task<JwtSecurityToken> GetTokenAsync(ApplicationUser user)
    {
        var jwt = new JwtSecurityToken (
            issuer: _configuration["JwSettings:Issuer"],
            audience: _configuration["JwSettings:Audience"],
            claims: await GetClaimsAsync(user),
            expires:  DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["JwSettings:ExpirationTimeInMinutes"])),
            
            signingCredentials: GetSigningCredentials());
            
        return jwt;

    }
        
    private SigningCredentials GetSigningCredentials() {
        var key = Encoding.UTF8.GetBytes(_configuration["JwSettings:SecurityKey"]!);
        var secret = new SymmetricSecurityKey(key);
        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    private async Task<List<Claim>> GetClaimsAsync(ApplicationUser user)
    {
        var claims = new List<Claim> {  

            new Claim(ClaimTypes.Name, user.Email!)
        };

        foreach (var role in await _userManager.GetRolesAsync(user))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        return claims;
     }            
}

