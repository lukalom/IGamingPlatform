
using Asp.Versioning;
using FluentValidation;
using IGamingPlatform.Api;
using IGamingPlatform.Api.Middlewares;
using IGamingPlatform.Api.OpenApi;
using IGamingPlatform.Application.Behaviour;
using IGamingPlatform.Application.Users.Commands.RegisterCommand;
using IGamingPlatform.Infrastructure.Persistence;
using IGamingPlatform.Infrastructure.Repositories;
using IGamingPlatform.Infrastructure.Repositories.Abstractions;
using IGamingPlatform.Infrastructure.UnitOfWork;
using IGamingPlatform.Infrastructure.UnitOfWork.Abstractions;
using IGamingPlatform.Shared.Settings;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder);

var app = builder.Build();

ConfigureApplication(app);

await app.RunAsync();

static void ConfigureServices(WebApplicationBuilder builder)
{
    var applicationAssembly = typeof(RegisterUserCommand).Assembly;

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

    builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
    builder.Services.AddSwaggerGen();

    builder.Services.AddScoped(ApplicationContextFactory.Create);

    AddInfrastructure(builder);

    builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(applicationAssembly));
    builder.Services.AddValidatorsFromAssembly(applicationAssembly, includeInternalTypes: true);

    builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

    builder.Services.AddApiVersioning(options =>
    {
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.ReportApiVersions = true;
    });

    builder.Services
        .AddApiVersioning()
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

    builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(nameof(JwtSettings)));

    var jwtSettings = builder.Configuration.GetRequiredSection(nameof(JwtSettings)).Get<JwtSettings>()!;

    builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };
        });
}

static void AddInfrastructure(WebApplicationBuilder builder)
{
    builder.Services.AddDbContext<GamingPlatformDBContext>((_, options) =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("IGamingPlatform"));
    });

    builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
}

static void ConfigureApplication(WebApplication app)
{
    app.UseMiddleware<ErrorHandlerMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            var descriptions = app.DescribeApiVersions();

            foreach (var description in descriptions)
            {
                var url = $"/swagger/{description.GroupName}/swagger.json";
                var name = description.GroupName.ToUpperInvariant();
                options.SwaggerEndpoint(url, name);
            }
        });
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
}