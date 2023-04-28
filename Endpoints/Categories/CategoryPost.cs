using Ecommerce.Domain.Products;
using Ecommerce.Infra.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Ecommerce.Endpoints.Categories
{
    public class CategoryPost
    {
        public static string Template => "/categories";
        public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
        public static Delegate Handle => Action;

        [Authorize(Policy = "EmployeePolicy")]
        public static async Task<IResult> Action(CategoryRequest request, HttpContext http, ApplicationDbContext context)
        {
            var userId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var category = new Category(request.Name, userId, userId);

            if (!category.IsValid)
            {
                return Results.ValidationProblem(category.Notifications.ConvertToProblemDetails());
            }
            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();

            return Results.Created($"/categories/{category.Id}", category.Id);
        }
    }
}
