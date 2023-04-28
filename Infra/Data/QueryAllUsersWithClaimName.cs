using Dapper;
using Ecommerce.Endpoints.Employees;
using Microsoft.Data.SqlClient;

namespace Ecommerce.Infra.Data
{
    public class QueryAllUsersWithClaimName
    {
        private readonly IConfiguration configuration;

        public QueryAllUsersWithClaimName(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IEnumerable<EmployeeResponse>> Execute(int page, int rows)
        {
            var db = new SqlConnection(configuration["ConnectionStrings:EcommerceDb"]);
            var query = @"SELECT Email, ClaimValue AS Name FROM ASPNETUSERS U 
                        INNER JOIN ASPNETUSERCLAIMS C ON U.Id = C.UserId AND ClaimType = 'Name' 
                        ORDER BY NAME
                        OFFSET (@page - 1) * @rows ROWS FETCH NEXT @rows ROWS ONLY";
            return await db.QueryAsync<EmployeeResponse>(query, new { page, rows });
        }
    }
}
