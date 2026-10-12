using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;
using Vendas.API.Middlewares;
using Vendas.Core;
using Vendas.DB.Migrations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(o => o.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);
builder.Services.AddOpenApi();
builder.Services.ResolveDependencies(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000", "http://127.0.0.1:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var erros = context.ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

        return new BadRequestObjectResult(new { mensagem = "Dados inválidos.", erros });
    };
});

var app = builder.Build();

MigrationRunner.RunMigrations(app.Configuration["BancoConfig:ConnectionString"]!);

app.UseMiddleware<ExceptionMiddleware>();
app.UseCors("AllowFrontend");
app.MapControllers();
app.MapOpenApi();

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference(o => o.WithTitle("Vendas API"));
}

app.MapGet("/", () => Results.Redirect("/scalar/v1")).ExcludeFromDescription();

app.Run();
