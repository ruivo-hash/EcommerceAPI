﻿using Ecommerce.Domain.Products;
using Ecommerce.Infra.Data;
using Microsoft.AspNetCore.Mvc;
using static System.Net.WebRequestMethods;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Ecommerce.Endpoints.Categories
{
    public class CategoryPut
    {
        public static string Template => "/categories/{id:guid}";
        public static string[] Methods => new string[] { HttpMethod.Put.ToString() };
        public static Delegate Handle => Action;

        [Authorize(Policy = "EmployeePolicy")]
        public static IResult Action([FromRoute] Guid id, HttpContext http, CategoryRequest request, ApplicationDbContext context)
        {
            var userId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var category = context.Categories.Where(c => c.Id == id).FirstOrDefault();

            if(category == null)
                return Results.NotFound();

            category.EditInfo(request.Name, request.Active, userId);

            if (!category.IsValid)
                return Results.ValidationProblem(category.Notifications.ConvertToProblemDetails());
            
            context.SaveChanges();

            return Results.Ok();
        }
    }
}
