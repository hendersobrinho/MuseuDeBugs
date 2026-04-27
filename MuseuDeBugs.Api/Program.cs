using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MuseuDeBugs.Api.Data;
using MuseuDeBugs.Api.Options;
using MuseuDeBugs.Api.Security;
using MuseuDeBugs.Api.Services;

var builder = WebApplication.CreateBuilder(args);

const string politicaCors = "PermitirFrontend";

// Controllers e documentacao da API.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Services da aplicacao.
builder.Services.AddScoped<BugService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddSingleton<LoginAttemptLimiter>();

// Configuracoes tipadas lidas do appsettings.
builder.Services.Configure<AdminOptions>(
    builder.Configuration.GetSection("Admin")
);

// Autenticacao por cookie para manter o admin logado apos o login.
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "museu_admin";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always;

        // Em API, retorno HTTP e melhor do que redirect para pagina de login.
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };

        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("LoginPolicy", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));
});

// CORS libera o front local a chamar a API.
builder.Services.AddCors(options =>
{
    options.AddPolicy(politicaCors, policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Banco de dados MySQL via Entity Framework Core.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    ));

var app = builder.Build();

app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

    if (context.Request.Path.StartsWithSegments("/api/auth") ||
        context.Request.Path.StartsWithSegments("/api/bugs"))
    {
        context.Response.Headers.CacheControl = "no-store";
    }

    await next();
});

// Swagger fica habilitado apenas em desenvolvimento.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Pipeline HTTP: a ordem aqui importa.
app.UseHttpsRedirection();
app.UseCors(politicaCors);
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
