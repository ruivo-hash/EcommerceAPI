using Ecommerce.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Ecommerce.Endpoints.Employees
{
    public class EmployeePost
    {
        public static string Template => "/employees";
        public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
        public static Delegate Handle => Action;

        [Authorize(Policy = "EmployeePolicy")]
        public static async Task<IResult> Action(EmployeeRequest request, HttpContext http, UserCreator userCreator)
        {
            var user = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var userClaims = new List<Claim>
        {
            new Claim("EmployeeCode", request.EmployeeCode),
            new Claim("Name", request.Name),
            new Claim("CreatedBy", user),
        };
            (IdentityResult result, string userId) = await userCreator.Create(request.Email, request.Password, userClaims);

            if (!result.Succeeded)
                return Results.ValidationProblem(result.Errors.ConvertToProblemDetails());

            return Results.Created($"/employees/{userId}", userId);
        }
    }
}
