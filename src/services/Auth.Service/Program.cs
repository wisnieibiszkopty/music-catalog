using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseInMemoryDatabase("AuthDb")
);

builder.Services
    .AddIdentityApiEndpoints<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.SignIn.RequireConfirmedEmail = false;
    })
    .AddEntityFrameworkStores<AppDbContext>();
    
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapGroup("/auth").MapIdentityApi<IdentityUser>();

app.Run();

class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<IdentityUser>(options) { }