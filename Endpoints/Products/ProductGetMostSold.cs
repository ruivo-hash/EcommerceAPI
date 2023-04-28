using Ecommerce.Infra.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Endpoints.Products
{
    public class ProductGetMostSold
    {
        public static string Template => "/products/mostsold";
        public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
        public static Delegate Handle => Action;

        [Authorize(Policy ="EmployeePolicy")]
        public static async Task<IResult> Action(QueryMostProductsSold query)
        {
            var result = await query.Execute();
            return Results.Ok(result);
        }
    }
}
