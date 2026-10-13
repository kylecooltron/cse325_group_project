using BlazorGroupProjectApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlazorGroupProjectApp.Data;

public class ClimateSeerContext : IdentityDbContext<IdentityUser>
{
    public ClimateSeerContext(DbContextOptions<ClimateSeerContext> options)
        : base(options)
    {
    }

    public DbSet<Prediction> Predictions { get; set; }
    public DbSet<PredictionResult> PredictionResults { get; set; }
    public DbSet<WeatherObservation> WeatherObservations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Prediction>()
            .HasOne<IdentityUser>()
            .WithMany()
            .HasForeignKey(p => p.UserId);

        modelBuilder.Entity<PredictionResult>()
            .HasOne(r => r.Prediction)
            .WithOne(p => p.Result)
            .HasForeignKey<PredictionResult>(r => r.PredictionId);
    }
}
