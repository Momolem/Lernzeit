using Lernzeit.Application.Contracts;
using Lernzeit.DataProtection;
using Lernzeit.PostgresAdapter;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Lernzeit.RaumzeitAPI;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();
builder.Services
    .AddSingleton(TimeProvider.System)
    .AddScoped<RaumzeitService>()
    .AddScoped<ICalendarService>(sp => sp.GetRequiredService<RaumzeitService>())
    .AddScoped<Lernzeit.Application.GroupCalendarService>()
    .AddSingleton<ITokenEncryptionService, TokenEncryptionService>()
    .AddScoped<IRaumzeitTokenRepository, RaumzeitTokenRepository>();


builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.Cookie.Name = "Lernzeit.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.None; 
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; 
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
        options.LoginPath = "/api/auth/login"; 
        options.LogoutPath = "/api/auth/logout";
    })
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "placeholder";
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "placeholder";
        options.ClaimActions.MapJsonKey("picture", "picture", "url");
    });

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(builder.Configuration["FrontendUrl"] ?? "http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.AddNpgsqlDbContext<LernzeitDbContext>("postgres");

builder.Services.AddScoped<Lernzeit.Application.Contracts.IGroupRepository, Lernzeit.PostgresAdapter.GroupRepository>();
builder.Services.AddScoped<Lernzeit.Application.Contracts.IUserRepository, Lernzeit.PostgresAdapter.UserRepository>();

var app = builder.Build();

app.UseCors();

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        if (exception != null)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsJsonAsync(new { error = exception.Message });
        }
    });
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapDefaultEndpoints();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using var scope = app.Services.CreateScope();
await using var dbContext = scope.ServiceProvider.GetRequiredService<LernzeitDbContext>();
var test = await dbContext.Database.EnsureCreatedAsync();

app.Run();