using Ecommerce.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using static System.Net.WebRequestMethods;

namespace Ecommerce.Endpoints.Clients
{
    public class ClientPost
    {
        public static string Template => "/clients";
        public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
        public static Delegate Handle => Action;

        [AllowAnonymous]
        public static async Task<IResult> Action(ClientRequest request, UserCreator userCreator)
        {
            var userClaims = new List<Claim>
            {
                new Claim("Cpf", request.Cpf),
                new Claim("Name", request.Name)
            };

            (IdentityResult result, string userId) = await userCreator.Create(request.Email, request.Password, userClaims);

            if (!result.Succeeded)
                return Results.ValidationProblem(result.Errors.ConvertToProblemDetails());

            return Results.Created($"/clients/{userId}", userId);
        }
    }
}
