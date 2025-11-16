using ProductApi.Infrastructure.DependencyInjection;
using ProductApi.Presentation.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
//swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructureService(builder.Configuration);

//builder.Services.ConfigureHttpJsonOptions(options =>
//{
//    options.AllowInferredBodyParameters = true;
//});


var app = builder.Build();

app.UseInfrastructurePolicies();
app.MapProductEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{    
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
//app.UseAuthorization();
app.Run();