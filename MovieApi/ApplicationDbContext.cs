using Microsoft.EntityFrameworkCore;
using MovieApi.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Movie> Movies { get; set; }
    public DbSet<MovieDetails> MovieDetails { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Actor> Actors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Explicit plural table names
        modelBuilder.Entity<Movie>().ToTable("Movies");
        modelBuilder.Entity<MovieDetails>().ToTable("MovieDetails");
        modelBuilder.Entity<Review>().ToTable("Reviews");
        modelBuilder.Entity<Actor>().ToTable("Actors");

        // 1:1 Movie <-> MovieDetails
        modelBuilder.Entity<Movie>()
            .HasOne(m => m.MovieDetails)
            .WithOne(md => md.Movie)
            .HasForeignKey<MovieDetails>(md => md.MovieId);

        // 1:M Movie <-> Review
        modelBuilder.Entity<Movie>()
            .HasMany(m => m.Reviews)
            .WithOne(r => r.Movie)
            .HasForeignKey(r => r.MovieId);

        // Budget precision
        modelBuilder.Entity<MovieDetails>()
            .Property(md => md.Budget)
            .HasPrecision(18, 2);
    }
}