using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Employee.Infrastructure.Persistence;
using Employee.Infrastructure.Repositories;
using Employee.Domain.Repositories;
using Employee.Application.Mappings;

var builder = WebApplication.CreateBuilder(args);

// ===== Configuration =====
var configuration = builder.Configuration;

// ===== Add services to the container =====

// Controllers
builder.Services.AddControllers();

// Database Context
builder.Services.AddDbContext<EmployeeDbContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), sqlOptions =>
    {
        sqlOptions.MigrationsAssembly(typeof(EmployeeDbContext).Assembly.FullName);
        sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
    });
});

// Repositories
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();

// MediatR (CQRS)
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Employee.Application.AssemblyReference).Assembly);
});

// AutoMapper
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(Employee.Application.AssemblyReference).Assembly);

// Localization (MultiLanguage support)
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en"),
        new CultureInfo("pt-BR")
    };

    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    options.RequestCultureProviders.Insert(0, new AcceptLanguageHeaderRequestCultureProvider());
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<EmployeeDbContext>("database");

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Employee Management API",
        Version = "v1",
        Description = "API para gerenciamento de funcionários com DDD, CQRS e Clean Architecture",
        Contact = new()
        {
            Name = "Employee Management Team",
            Email = "contact@employeemanagement.com"
        }
    });

    options.EnableAnnotations();
});

var app = builder.Build();

// ===== Configure the HTTP request pipeline =====

// Swagger (Development and Production)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee Management API v1");
        options.RoutePrefix = string.Empty; // Swagger at root (http://localhost:5000/)
    });
}
else
{
    // Production: Swagger disponível em /swagger
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee Management API v1");
        options.RoutePrefix = "swagger";
    });
}

// Localization
app.UseRequestLocalization();

// HTTPS Redirection
app.UseHttpsRedirection();

// CORS
app.UseCors("AllowSpecificOrigins");

// Authentication & Authorization (preparado para futuro)
app.UseAuthentication();
app.UseAuthorization();

// Controllers
app.MapControllers();

// Health Checks
app.MapHealthChecks("/health");

app.Run();
