using Microsoft.EntityFrameworkCore;
using RWS_LBE_Register.Data;
using RWS_LBE_Register.DTOs.Configurations;
using RWS_LBE_Register.Helpers;
using RWS_LBE_Register.Services;
using RWS_LBE_Register.Services.Implementations;
using RWS_LBE_Register.Services.Interfaces;
using RWS_LBE_Register.Settings;

var builder = WebApplication.CreateBuilder(args);

// 1) Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// Register HTTP Client Factory (required for injecting HttpClient)
builder.Services.AddHttpClient();
builder.Services.Configure<CiamSettings>(builder.Configuration.GetSection("Api:Eeid"));
builder.Services.AddSingleton<CiamService>();

builder.Services.Configure<RlpNumberingOptions>(
    builder.Configuration.GetSection("Application:RLPNumberingFormat"));

builder.Services.Configure<RlpApiConfig>(
    builder.Configuration.GetSection("ExternalApiConfig:RlpApiConfig"));

// Add OTP service
builder.Services.AddScoped<IOtpService, OTPService>();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IOtpService, OTPService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRlpService, RlpService>();
builder.Services.AddScoped<RlpNumberingHelper>();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new DateOnlyConverter());
});



builder.Services.Configure<AcsSettings>(
    builder.Configuration.GetSection("Api:Acs"));
// Register the AcsService with HttpClient support
builder.Services.AddHttpClient<AcsService>();
var app = builder.Build();

// Log incoming requests
app.UseMiddleware<RequestLoggingMiddleware>();

//Conditionally run the JWT middleware for "/api/v1/user/**" only
var apiPrefix = "/api/v1/";
var protectedPrefixes = new[]
{
    apiPrefix + "user"
};

app.UseWhen(
    ctx =>
    {
        // if the request path starts with any of our protected prefixes�
        var path = ctx.Request.Path;
        return protectedPrefixes
            .Any(p => path.StartsWithSegments(p, StringComparison.OrdinalIgnoreCase));
    },
    branch =>
    {
        // �then run the JWT interceptor on that branch.
        branch.UseMiddleware<JwtInterceptorMiddleware>();
    });

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var secretBase64 = builder.Configuration["Jwt:Secret"];
if (string.IsNullOrWhiteSpace(secretBase64))
    throw new InvalidOperationException("Missing Jwt:Secret in configuration");

byte[] keyBytes;
try
{
    keyBytes = Convert.FromBase64String(secretBase64);
}
catch
{
    throw new InvalidOperationException("Jwt:Secret must be a Base64-encoded string");
}
// Auto-migrate DB on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); // This applies any pending migrations
}

TokenInterceptor.SetJwtSecret(keyBytes);
app.UseHttpsRedirection();
app.UseMiddleware<AuditLogMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.Run();
