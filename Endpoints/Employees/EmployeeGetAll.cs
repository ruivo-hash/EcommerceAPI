using Dapper;
using Ecommerce.Infra.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using System.Security.Claims;

namespace Ecommerce.Endpoints.Employees
{
    public class EmployeeGetAll
    {
        public static string Template => "/employees";
        public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
        public static Delegate Handle => Action;

        [Authorize(Policy = "EmployeePolicy")]
        public static async Task<IResult> Action(int? page, int? rows, QueryAllUsersWithClaimName query)
        {
            var result = await query.Execute(page.Value, rows.Value);
            return Results.Ok(result);
        }
    }
}
