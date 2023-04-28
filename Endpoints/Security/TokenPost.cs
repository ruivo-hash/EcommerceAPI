using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ecommerce.Endpoints.Security
{
    public class TokenPost
    {
        public static string Template => "/token";
        public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
        public static Delegate Handle => Action;

        [AllowAnonymous]
        public static IResult Action(LoginRequest request, IConfiguration configuration, UserManager<IdentityUser> userManager, ILogger<TokenPost> log, IWebHostEnvironment env)
        {
            log.LogInformation("Getting token");

            var user = userManager.FindByEmailAsync(request.Email).Result;
            if (user == null)
                Results.BadRequest();

            if (!userManager.CheckPasswordAsync(user, request.Password).Result)
                Results.BadRequest();

            var claims = userManager.GetClaimsAsync(user).Result;
            var subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Email, request.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            });
            subject.AddClaims(claims);

            var key = Encoding.ASCII.GetBytes(configuration["JwtBearerTokenSettings:SecretKey"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = subject,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = configuration["JwtBearerTokenSettings:Audience"],
                Issuer = configuration["JwtBearerTokenSettings:Issuer"],
                Expires = env.IsDevelopment() || env.IsStaging() ? DateTime.UtcNow.AddHours(8) : DateTime.UtcNow.AddMinutes(5)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Results.Ok(new
            {
                token = tokenHandler.WriteToken(token)
            });
        }
    }
}
