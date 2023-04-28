using Ecommerce.Domain.Users;
using Ecommerce.Endpoints.Categories;
using Ecommerce.Endpoints.Clients;
using Ecommerce.Endpoints.Employees;
using Ecommerce.Endpoints.Orders;
using Ecommerce.Endpoints.Products;
using Ecommerce.Endpoints.Security;
using Ecommerce.Infra.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseSerilog((context, configuration) => 
{
    configuration
        .WriteTo.Console()
        .WriteTo.MSSqlServer(
            context.Configuration["ConnectionStrings:EcommerceDb"],
              sinkOptions: new MSSqlServerSinkOptions()
              {
                  AutoCreateSqlTable = true,
                  TableName = "LogAPI"
              });
});

builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["ConnectionStrings:EcommerceDb"]);
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddScoped<QueryAllUsersWithClaimName>();
builder.Services.AddScoped<QueryMostProductsSold>();
builder.Services.AddScoped<UserCreator>();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
      .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
      .RequireAuthenticatedUser()
      .Build();
    options.AddPolicy("EmployeePolicy", p =>
        p.RequireAuthenticatedUser().RequireClaim("EmployeeCode"));
});
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateActor = true,
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = builder.Configuration["JwtBearerTokenSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtBearerTokenSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JwtBearerTokenSettings:SecretKey"]))
    };
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapMethods(CategoryPost.Template, CategoryPost.Methods, CategoryPost.Handle);
app.MapMethods(CategoryGetAll.Template, CategoryGetAll.Methods, CategoryGetAll.Handle);
app.MapMethods(CategoryPut.Template, CategoryPut.Methods, CategoryPut.Handle);
app.MapMethods(CategoryDelete.Template, CategoryDelete.Methods, CategoryDelete.Handle);

app.MapMethods(EmployeePost.Template, EmployeePost.Methods, EmployeePost.Handle);
app.MapMethods(EmployeeGetAll.Template, EmployeeGetAll.Methods, EmployeeGetAll.Handle);

app.MapMethods(TokenPost.Template, TokenPost.Methods, TokenPost.Handle);

app.MapMethods(ProductPost.Template, ProductPost.Methods, ProductPost.Handle);
app.MapMethods(ProductGetAll.Template, ProductGetAll.Methods, ProductGetAll.Handle);
app.MapMethods(ProductGetShowcase.Template, ProductGetShowcase.Methods, ProductGetShowcase.Handle);
app.MapMethods(ProductGetMostSold.Template, ProductGetMostSold.Methods, ProductGetMostSold.Handle);

app.MapMethods(ClientPost.Template, ClientPost.Methods, ClientPost.Handle);
app.MapMethods(ClientGet.Template, ClientGet.Methods, ClientGet.Handle);

app.MapMethods(OrderGet.Template, OrderGet.Methods, OrderGet.Handle);
app.MapMethods(OrderPost.Template, OrderPost.Methods, OrderPost.Handle);

app.UseExceptionHandler("/error");
app.Map("/error", (HttpContext http) =>
{
    var error = http.Features?.Get<IExceptionHandlerFeature>()?.Error;
    if(error != null)
    {
        if(error is SqlException)
            return Results.Problem(title: "Database out", statusCode: 500);
        else if(error is FormatException)
            return Results.Problem(title: "Error to convert data to other type", statusCode: 500);
    }
        
    return Results.Problem(title: "An error ocurred", statusCode: 500);
});

app.Run();