using Ecommerce.Domain.Products;
using Ecommerce.Infra.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Ecommerce.Endpoints.Products
{
    public class ProductPost
    {
        public static string Template => "/products";
        public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
        public static Delegate Handle => Action;

        [Authorize(Policy = "EmployeePolicy")]
        public static async Task<IResult> Action(ProductRequest request, HttpContext http, ApplicationDbContext context)
        {
            var userId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == request.CategoryId);
            var product =
                new Product(request.Name, category, request.Description, request.HasStock, request.Price, userId);

            if (!product.IsValid)
            {
                return Results.ValidationProblem(product.Notifications.ConvertToProblemDetails());
            }

            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            return Results.Created($"/products/{product.Id}", product.Id);
        }
    }
}
