using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// JWT validation — this MUST match AuthService's Issuer, Audience, and signing
// key exactly, or every token it issues will be rejected here.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).
        AddJwtBearer(options => options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["jwt:Issuer"],
            ValidAudience = builder.Configuration["jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["jwt:Token"]!))

        });

// A bare AddAuthorization() registers ASP.NET Core's built-in "Default" policy,
// which requires an authenticated user. Routes in appsettings.json reference
// this by name ("Default") or opt out entirely ("Anonymous").
builder.Services.AddAuthorization();

// Basic per-client rate limiting at the edge, before requests reach any service.
builder.Services.AddRateLimiter(options =>
{
    options.OnRejected = (context, _) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        return ValueTask.CompletedTask;
    };

    options.AddFixedWindowLimiter("PerClientLimit", opt =>
    {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueLimit = 0;
    });
});

// Routes and clusters (which service each path forwards to) live in
// appsettings.json under "ReverseProxy" — see that file for the actual mapping.
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapReverseProxy().RequireRateLimiting("PerClientLimit");

// Unified Scalar UI at /scalar — dropdown lists every service, each backed by
// its own OpenAPI doc proxied through the gateway (see *-docs routes below).
app.MapScalarApiReference(options =>
{
    options.AddDocument("Admin", "AdminService", "/Admin-docs/openapi.json")
           .AddDocument("Movies", "Movieservice", "/Movies-docs/openapi.json");
}).AllowAnonymous();

app.Run();