using Azure.Identity;
using PreconIQAPI.Configuration;
using PreconIQAPI.Data;
using PreconIQAPI.Data.DAL;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;


var builder = WebApplication.CreateBuilder(args);

var isDevelopment = builder.Environment.IsDevelopment();

// CONFIGURATION ORDER IS CRITICAL - DO NOT REARRANGE WITHOUT UNDERSTANDING DEPENDENCIES
// 1. Logging must be first - other services may log during configuration
ConfigureLogging(builder, isDevelopment);

// 2. Key Vault must be early - adds configuration providers that other services may need
ConfigureKeyVault(builder);

// 3. Core services can be configured in any order (no dependencies between them)
ConfigureServices(builder, isDevelopment);

// 4. Authentication must be before Swagger - Swagger references auth configuration
ConfigureAuthentication(builder);

// 5. Read path base configuration once for use in multiple places
var pathBase = builder.Configuration.GetValue<string>("Swagger:BasePath");

// 6. Swagger depends on authentication being configured first and path base
ConfigureSwagger(builder, pathBase);

// 7. CORS has no dependencies and can be anywhere in service configuration
ConfigureCors(builder);

var app = builder.Build();

// MIDDLEWARE PIPELINE ORDER IS CRITICAL - ORDER MATTERS FOR REQUEST PROCESSING
// Middleware executes in the order registered here

// 1. Database initialization must happen BEFORE middleware configuration
//    (ensures DB is ready before any requests are processed)
InitializeDatabase(app, isDevelopment);

// 2. Configure middleware pipeline (order within this method is also critical)
ConfigureMiddleware(app, builder, isDevelopment, pathBase);

app.Run();


/// <summary>
/// Configure logging for the application.
/// </summary>
static void ConfigureLogging(WebApplicationBuilder builder, bool isDevelopment)
{
    // Make NLog the only log provider
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    if (!isDevelopment)
    {
        var loggingEventHubConnectionString = builder.Configuration.GetValue<string>("LogEventHub:EventHubConnectionString");
        var loggingEventHubName = builder.Configuration.GetValue<string>("LogEventHub:EventHubName");

        GlobalDiagnosticsContext.Set("EventHubConnectionString", loggingEventHubConnectionString);
        GlobalDiagnosticsContext.Set("EventHubName", loggingEventHubName);
    }
}

/// <summary>
/// Configure Azure Key Vault integration for the application.
/// TIMING: Should be called early in configuration as it adds configuration providers 
/// that other services may depend on.
/// </summary>
static void ConfigureKeyVault(WebApplicationBuilder builder)
{
    var keyVaultUrl = builder.Configuration.GetValue<string>("KeyVault:KeyVaultUrl");
    if (!string.IsNullOrEmpty(keyVaultUrl))
    {
        var keyVaultEndpoint = new Uri(keyVaultUrl);
        builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());
    }
}

/// <summary>
/// Configure application services.
/// </summary>
static void ConfigureServices(WebApplicationBuilder builder, bool isDevelopment)
{
    // Add services to the container.

    builder.Services.AddControllers();

    // Database configuration
    ConfigureDatabase(builder, isDevelopment);

    // Unit of Work Injection
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

    // Add Mapper Configurations
    builder.Services.AddMapster();

    builder.Services.AddEndpointsApiExplorer();
}

/// <summary>
/// Configure database services.
/// </summary>
static void ConfigureDatabase(WebApplicationBuilder builder, bool isDevelopment)
{
    var dbConfig = GetDatabaseConfig(builder.Configuration);

    var connectionString = dbConfig.GetConnectionString(isDevelopment);

    // TODO: Uncomment when database is ready
     builder.Services.AddDbContext<PreconDbContext>(options => 
         options.UseSqlServer(connectionString, sql => 
             sql.MigrationsHistoryTable(dbConfig.MigrationsHistoryTable, dbConfig.Schema)));

    // Database Setup
    //builder.Services.AddDbContext<PreconDbContext>(
     //   options => options.UseSqlServer(builder.Environment.IsDevelopment()
     //   ? builder.Configuration.GetConnectionString("LocalConnection") // Need to use LocalConnection for local dev since Key Vault (DefaultConnection) gets applied last in Configuration.
      //  : builder.Configuration.GetConnectionString("DefaultConnection"))
     //   .UseSqlServer(x => x.MigrationsHistoryTable("__EFMigrationsHistory", builder.Configuration.GetValue<string>("BLAAppsPreconDatabaseSchema"))) // Need to specify schema for Migration History table to not use dbo
     //   );
}

/// <summary>
/// Configure authentication and authorization services.
/// </summary>
static void ConfigureAuthentication(WebApplicationBuilder builder)
{
    var entraConfig = GetEntraIdConfig(builder.Configuration);

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = entraConfig.Authority;
            options.Audience = entraConfig.ApiClientId;
        });

    builder.Services.AddAuthorization();
}

/// <summary>
/// Configure Swagger for API documentation.
/// DEPENDENCY: Must be called AFTER ConfigureAuthentication() as it references auth configuration.
/// </summary>
static void ConfigureSwagger(WebApplicationBuilder builder, string? pathBase)
{
    var entraConfig = GetEntraIdConfig(builder.Configuration);
    var swaggerConfig = GetSwaggerConfig(builder.Configuration);

    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = swaggerConfig.Title,
            Version = swaggerConfig.Version
        });

        // Configure server URLs for proper API base path handling
        if (!string.IsNullOrEmpty(pathBase))
        {
            c.AddServer(new OpenApiServer
            {
                Url = pathBase
            });
        }

        // OAuth2 definition using Authorization Code + PKCE
        var oAuthScheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = entraConfig.AuthorizationUrl,
                    TokenUrl = entraConfig.TokenUrl,
                    Scopes = new Dictionary<string, string>
                    {
                        [entraConfig.ScopeFullName] = swaggerConfig.ScopeDescription
                    }
                }
            }
        };

        c.AddSecurityDefinition(swaggerConfig.SecuritySchemeId, oAuthScheme);

        // Apply security requirement to all endpoints
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = swaggerConfig.SecuritySchemeId
                    }
                },
                new[] { entraConfig.ScopeFullName }
            }
        });
    });
}

/// <summary>
/// Configure CORS policies.
/// </summary>
static void ConfigureCors(WebApplicationBuilder builder)
{
    // ---- CORS ----
    // Define allowed SPA origins for local development
    var spaOrigins = new[]
{
    "http://localhost:3000"  // React
    // add/remove as needed
};

    // CORS policy for local SPA development
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("SpaDev", policy =>
        {
            policy.WithOrigins(spaOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });
}

/// <summary>
/// Configure the middleware pipeline for the application.
/// WARNING: MIDDLEWARE ORDER IS CRITICAL - DO NOT REARRANGE WITHOUT UNDERSTANDING REQUEST FLOW
/// </summary>
/// <remarks>
/// Middleware executes in the order registered. Each middleware can:
/// - Process the request, then call next middleware
/// - Process the response on the way back
/// - Short-circuit the pipeline by not calling next
/// 
/// Current order rationale:
/// 1. Exception handling first - catches all unhandled exceptions
/// 2. Path base configuration - for reverse proxy scenarios
/// 3. Swagger UI - for development/documentation (before security)
/// 4. HTTPS redirection - security requirement
/// 5. CORS - handle cross-origin requests before auth
/// 6. Authentication - identify the user
/// 7. Authorization - verify permissions (MUST be after Authentication)
/// 8. Controller routing - handle the actual request
/// </remarks>
static void ConfigureMiddleware(WebApplication app, WebApplicationBuilder builder, bool isDevelopment, string? pathBase)
{
    // 1. EXCEPTION HANDLING - Must be first to catch all downstream exceptions
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync("An error occurred.");
        });
    });

    // 2. PATH BASE CONFIGURATION - Configure for reverse proxy scenarios
    if (!string.IsNullOrEmpty(pathBase))
    {
        app.UsePathBase(pathBase);
    }

    // 3. SWAGGER - Early in pipeline for documentation access
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        var entraConfig = GetEntraIdConfig(builder.Configuration);
        var swaggerConfig = GetSwaggerConfig(builder.Configuration);

        // For deployed environments behind APIM, include the full path
        var swaggerEndpoint = !string.IsNullOrEmpty(pathBase)
            ? $"{pathBase}{swaggerConfig.SwaggerEndpoint}"
            : swaggerConfig.SwaggerEndpoint;

        c.SwaggerEndpoint(swaggerEndpoint, $"{swaggerConfig.Title} {swaggerConfig.Version}");
        c.OAuthClientId(entraConfig.UiClientId);
        c.OAuthUsePkce();

        var allScopes = swaggerConfig.AdditionalScopes.Concat(new[] { entraConfig.ScopeFullName }).ToArray();
        c.OAuthScopes(allScopes);
        c.OAuth2RedirectUrl(swaggerConfig.OAuth2RedirectUrl);
    });

    // 4. HTTPS REDIRECTION - Security requirement, before CORS and Auth
    app.UseHttpsRedirection();

    // 5. CORS - Must be after HTTPS redirection but before Authentication
    if (isDevelopment)
    {
        app.UseCors("SpaDev");
    }

    // 6. AUTHENTICATION - Identify the user (MUST be before Authorization)
    app.UseAuthentication();

    // 7. AUTHORIZATION - Verify permissions (MUST be after Authentication)
    app.UseAuthorization();

    // 8. ROUTING - Map controllers and handle requests
    app.MapControllers();

    // 9. CLEANUP - Register shutdown handler
    app.Lifetime.ApplicationStopped.Register(LogManager.Shutdown);
}

/// <summary>
/// Initialize the database with migrations and seed data if in development mode.
/// TIMING: Must be called AFTER app.Build() but BEFORE the middleware pipeline starts processing requests.
/// This ensures the database is ready before any HTTP requests are handled.
/// </summary>
static void InitializeDatabase(WebApplication app, bool isDevelopment)
{
    using var scope = app.Services.CreateScope();
    IServiceProvider services = scope.ServiceProvider;

    PreconDbContext dbContext = services.GetRequiredService<PreconDbContext>();
    dbContext.Database.Migrate();

    // Only seed DB for development database
    if (app.Environment.IsDevelopment())
    {
        DbInitializer.Initialize(dbContext);
    }
}

/// <summary>
/// Helper method to get EntraIdConfig from application configuration.
/// </summary>
/// <param name="configuration">The application configuration instance.</param>
/// <returns>A configured EntraIdConfig instance with values from app settings.</returns>
static EntraIdConfig GetEntraIdConfig(IConfiguration configuration)
{
    return new EntraIdConfig
    {
        LoginUrl = configuration.GetValue<string>("EntraID:LoginURL") ?? string.Empty,
        TenantId = configuration.GetValue<string>("EntraID:TenantId") ?? string.Empty,
        ApiClientId = configuration.GetValue<string>("EntraID:API:ClientId") ?? string.Empty,
        UiClientId = configuration.GetValue<string>("EntraID:UI:ClientId") ?? string.Empty
    };
}

/// <summary>
/// Helper method to get DatabaseConfig from application configuration.
/// </summary>
/// <param name="configuration">The application configuration instance.</param>
/// <returns>A configured DatabaseConfig instance with values from app settings.</returns>
static DatabaseConfig GetDatabaseConfig(IConfiguration configuration)
{
    return new DatabaseConfig
    {
        Schema = configuration.GetValue<string>("BLAAppsPreconDatabaseSchema") ?? string.Empty,
        LocalConnection = configuration.GetConnectionString("LocalConnection") ?? string.Empty,
        DefaultConnection = configuration.GetConnectionString("DefaultConnection") ?? string.Empty
    };
}

/// <summary>
/// Helper method to get SwaggerConfig from application configuration.
/// </summary>
/// <param name="configuration">The application configuration instance.</param>
/// <returns>A configured SwaggerConfig instance with values from app settings.</returns>
static SwaggerConfig GetSwaggerConfig(IConfiguration configuration)
{
    return new SwaggerConfig
    {
        OAuth2RedirectUrl = configuration.GetValue<string>("Swagger:OAuth2RedirectUrl") ?? string.Empty
    };
}