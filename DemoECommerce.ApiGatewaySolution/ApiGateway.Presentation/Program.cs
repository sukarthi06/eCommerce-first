using ApiGateway.Presentation.Middlewares;
using ApiGateway.Presentation.Misc;
using eCommerce.SharedLibrary.DependencyInjection;
using Microsoft.OpenApi.Models;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Runtime.InteropServices;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

//if (builder.Environment.IsDevelopment())
//{
//    builder.WebHost.ConfigureKestrel(options =>
//    {
//        options.ListenLocalhost(5041);
//    });
//}

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

// Add Ocelot services

builder.Services.AddOcelot();
builder.Services.AddSwaggerForOcelot(builder.Configuration);
//builder.Services.AddOcelot().AddCacheManager(x => x.WithDictionaryHandle());

JWTAuthenticationScheme.AddJWTAuthenticationScheme(builder.Services, builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

app.MapWhen(
    ctx => ctx.Request.Path.Value!
        .StartsWith("/swagger/docs", StringComparison.OrdinalIgnoreCase),
    appBranch =>
    {
        appBranch.UseSwaggerForOcelotUI(opt =>
        {
            opt.PathToSwaggerGenerator = "/swagger/docs";
        });
    });


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseSwaggerForOcelotUI(opt =>
    {
        opt.PathToSwaggerGenerator = "/swagger/docs";
    });

}
else
{
    app.UseHttpsRedirection();
}

app.UseCors();
app.UseMiddleware<AttachSignatureToRequest>();


app.MapSwagger();
await app.UseOcelot();

app.Run();
