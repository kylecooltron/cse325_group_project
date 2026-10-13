using BlazorGroupProjectApp.Components;
using BlazorGroupProjectApp.Data;
using BlazorGroupProjectApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Load dev.env in development (never commit this file)
if (builder.Environment.IsDevelopment())
{
    var devEnvPath = Path.Combine(Directory.GetCurrentDirectory(), "dev.env");
    if (File.Exists(devEnvPath))
    {
        var devEnvVars = new Dictionary<string, string?>();
        foreach (var line in File.ReadAllLines(devEnvPath))
        {
            var trimmed = line.Trim();
            if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith('#')) continue;
            var idx = trimmed.IndexOf('=');
            if (idx < 0) continue;
            // Convert double-underscore keys to colon-separated for IConfiguration
            var key = trimmed[..idx].Replace("__", ":");
            devEnvVars[key] = trimmed[(idx + 1)..];
        }
        builder.Configuration.AddInMemoryCollection(devEnvVars);
    }
}

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddControllers();

// Razor Pages required for Identity login/logout endpoints
builder.Services.AddRazorPages();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ClimateSeerContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<ClimateSeerContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]
            ?? throw new InvalidOperationException("Google ClientId not configured.");
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]
            ?? throw new InvalidOperationException("Google ClientSecret not configured.");
    });

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/auth/login";
    options.LogoutPath = "/auth/logout";
    options.AccessDeniedPath = "/auth/login";
});

builder.Services.AddHttpClient();

builder.Services.AddScoped<PredictionService>();
builder.Services.AddScoped<WeatherService>();
builder.Services.AddScoped<ScoringService>();
builder.Services.AddScoped<LeaderboardService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapControllers();
app.MapRazorPages();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
