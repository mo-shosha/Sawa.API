using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SAWA.core.IServices;
using SAWA.core.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SAWA.infrastructure.Services
{
    public class GenerateTokenServices: IGenerateTokenServices
    {
  
        private readonly IConfiguration configuration;
        private readonly UserManager<AppUser> _userManager;

        public GenerateTokenServices(IConfiguration configuration, UserManager<AppUser> userManager)
        {
            this.configuration = configuration;
            _userManager = userManager;
        }
        public async Task<string> GetAndCreateToken(AppUser user)
        {

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            string secret = configuration["Token:Secret"];
            if (string.IsNullOrEmpty(secret))
            {
                throw new ArgumentNullException("Token secret is not configured");
            }

            byte[] key = Encoding.ASCII.GetBytes(secret);
            SigningCredentials credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(30), 
                Issuer = configuration["Token:Issuer"],
                SigningCredentials = credentials,
                NotBefore = DateTime.UtcNow
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            string tokenString = tokenHandler.WriteToken(token);


            return tokenString;
        }
    }
}
