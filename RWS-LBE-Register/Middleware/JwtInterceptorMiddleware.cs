using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using RWS_LBE_Register.Common;

public class JwtInterceptorMiddleware
{
    private readonly RequestDelegate _next;
    private readonly byte[] _secretKey;

    public JwtInterceptorMiddleware(RequestDelegate next, IConfiguration config)
    {
        _next = next;
        _secretKey = Convert.FromBase64String(config["Jwt:Secret"]!);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if (string.IsNullOrEmpty(authHeader))
        {
            await WriteErrorResponse(context, ResponseTemplate.MissingAuthTokenErrorResponse());
            return;
        }

        var parts = authHeader.Split(' ');
        if (parts.Length != 2 ||
            !parts[0].Equals("Bearer", StringComparison.OrdinalIgnoreCase))
        {
            await WriteErrorResponse(context, ResponseTemplate.InvalidAuthTokenErrorResponse());
            return;
        }

        var tokenString = parts[1];
        if (tokenString.Count(c => c == '.') != 2)
        {
            await WriteErrorResponse(context, ResponseTemplate.InvalidAuthTokenErrorResponse());
            return;
        }

        var handler = new JwtSecurityTokenHandler();
        var validationParams = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(_secretKey),
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var principal = handler.ValidateToken(
                tokenString, validationParams, out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwt ||
                !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
            {
                await WriteErrorResponse(context, ResponseTemplate.InvalidSignatureErrorResponse());
                return;
            }

            context.User = principal;
            context.Items["app_id"] =
                principal.Claims.FirstOrDefault(c => c.Type == "app_id")?.Value;

            await _next(context);
        }
        catch (SecurityTokenException)
        {
            await WriteErrorResponse(context, ResponseTemplate.InvalidAuthTokenErrorResponse());
        }
    }

    // Adjusted to take non-generic ApiResponse
    private static Task WriteErrorResponse(HttpContext ctx, ApiResponse errorResponse)
    {
        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
        ctx.Response.ContentType = "application/json";
        return ctx.Response.WriteAsJsonAsync(errorResponse);
    }
}
