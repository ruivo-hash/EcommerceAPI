﻿using Ecommerce.Domain.Orders;
using Ecommerce.Domain.Products;
using Ecommerce.Infra.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Ecommerce.Endpoints.Orders

{
    public class OrderPost
    {
        public static string Template => "/orders";
        public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
        public static Delegate Handle => Action;

        public static async Task<IResult> Action(OrderRequest request, HttpContext http, ApplicationDbContext context)
        {
            var clientId = http.User.Claims
                .First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var clientName = http.User.Claims
                .First(c => c.Type == "Name").Value;

            List<Product> productsFound = null;
            if (request.ProductsId != null && request.ProductsId.Any())
                productsFound = context.Products.Where(p => request.ProductsId.Contains(p.Id)).ToList();

            var order = new Order(clientId, clientName, productsFound, request.DeliveryAddress);
            if (!order.IsValid)
            {
                return Results.ValidationProblem(order.Notifications.ConvertToProblemDetails());
            }
            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync();

            return Results.Created($"/orders/{order.Id}", order.Id);
        }
    }
}
