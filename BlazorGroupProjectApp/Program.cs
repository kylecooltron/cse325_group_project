using BlazorGroupProjectApp.Components;
using BlazorGroupProjectApp.Data;
using BlazorGroupProjectApp.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ClimateSeerContext>(options =>
    options.UseSqlServer(connectionString));

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
app.UseAntiforgery();

app.MapStaticAssets();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
