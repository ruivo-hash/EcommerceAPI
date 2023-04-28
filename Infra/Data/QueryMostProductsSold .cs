using Dapper;
using Ecommerce.Endpoints.Employees;
using Ecommerce.Endpoints.Products;
using Microsoft.Data.SqlClient;

namespace Ecommerce.Infra.Data
{
    public class QueryMostProductsSold
    {
        private readonly IConfiguration configuration;

        public QueryMostProductsSold(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IEnumerable<ProductSoldResponse>> Execute()
        {
            var db = new SqlConnection(configuration["ConnectionStrings:EcommerceDb"]);
            var query = @"SELECT p.Id, p.Name, count(*) amount FROM Orders o 
                        INNER JOIN OrderProducts op ON o.Id = op.OrdersId
                        INNER JOIN Products p on p.Id = op.ProductsId 
                        GROUP BY p.Id, p.Name ORDER BY amount";
            return await db.QueryAsync<ProductSoldResponse>(query);
        }
    }
}
