using Microsoft.EntityFrameworkCore;
using Youxel.Check.LicensesGenerator.Infrastructure.DependencyInjection;
using Youxel.Check.LicensesGenerator.Utilities.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddContext(builder.Configuration);
builder.Services.AddServices();

var app = builder.Build();

// Automatically apply migrations
await app.MigrateAsync();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.UseMiddleware<BasicAuthenticationMiddleware>();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
