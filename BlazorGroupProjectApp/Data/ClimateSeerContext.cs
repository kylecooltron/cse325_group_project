using BlazorGroupProjectApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorGroupProjectApp.Data;

public class ClimateSeerContext : DbContext
{
    public ClimateSeerContext(DbContextOptions<ClimateSeerContext> options)
        : base(options)
    {
    }

    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Prediction> Predictions { get; set; }
    public DbSet<PredictionResult> PredictionResults { get; set; }
    public DbSet<LeaderboardEntry> LeaderboardEntries { get; set; }
    public DbSet<WeatherObservation> WeatherObservations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Prediction>()
            .HasOne(p => p.UserProfile)
            .WithMany(u => u.Predictions)
            .HasForeignKey(p => p.UserProfileId);

        modelBuilder.Entity<PredictionResult>()
            .HasOne(r => r.Prediction)
            .WithOne(p => p.Result)
            .HasForeignKey<PredictionResult>(r => r.PredictionId);

        modelBuilder.Entity<LeaderboardEntry>()
            .HasOne(e => e.UserProfile)
            .WithMany()
            .HasForeignKey(e => e.UserProfileId);
    }
}
