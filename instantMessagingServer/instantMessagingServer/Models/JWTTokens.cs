using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace instantMessagingServer.Models
{
    public static class JWTTokens
    {
        /// <summary>
        /// JWToken Generation
        /// </summary>
        /// <param name="username">the username owner</param>
        /// <param name="IDToken">the unique IDToken</param>
        /// <param name="key">the symetrical key for encryption</param>
        /// <param name="issuer">the token origin</param>
        /// <param name="duration">the token duration valitidy</param>
        /// <returns></returns>
        public static string Generate(string username, string IDToken, string key, string issuer, int duration)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, username));
            claims.Add(new Claim("IDToken", IDToken));

            var token = new JwtSecurityToken(
                issuer,
                issuer,
                claims,
                expires: DateTime.Now.AddSeconds(5).AddMinutes(duration),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
