using Ecommerce.Infra.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.Endpoints.Categories
{
    public class CategoryDelete
    {
        public static string Template => "/categories/{id:guid}";
        public static string[] Methods => new string[] { HttpMethod.Delete.ToString() };
        public static Delegate Handle => Action;

        [Authorize(Policy = "EmployeePolicy")]
        public static IResult Action([FromRoute] Guid id, HttpContext http, ApplicationDbContext context)
        {
            //var userId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var category = context.Categories.Where(c => c.Id == id).FirstOrDefault();

            if (category == null)
                return Results.NotFound();

            context.Categories.Remove(category);
            context.SaveChanges();

            return Results.Ok();
        }
    }
}
