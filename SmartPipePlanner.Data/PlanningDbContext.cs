using System.Numerics;
using Microsoft.EntityFrameworkCore;
using SmartPipePlanner.Core;

namespace SmartPipePlanner.Data;

public class PlanningDbContext : DbContext
{
    public DbSet<Element> Elements { get; set; }
    public DbSet<ProblemDto> Problems { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=planning.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Element 設定
        modelBuilder.Entity<Element>()
            .Property(e => e.Category)
            .HasConversion<string>();

        modelBuilder.Entity<Element>()
            .OwnsOne(e => e.Geometry, g =>
            {
                g.Property(geo => geo.Type).HasConversion<string>();
                g.Property(geo => geo.Orientation)
                 .HasConversion(
                    v => $"{v.X},{v.Y},{v.Z}",
                    s => Vector3FromString(s));
            });

        modelBuilder.Entity<Element>()
            .Property(e => e.Location)
            .HasConversion(
                v => $"{v.X},{v.Y},{v.Z}",
                s => Vector3FromString(s));

        // ProblemDto 設定
        modelBuilder.Entity<ProblemDto>()
            .Property(p => p.Category)
            .HasConversion<string>();  // PipeCategory → string

        modelBuilder.Entity<ProblemDto>()
            .Property(p => p.Start)
            .HasConversion(
                v => $"{v.X},{v.Y},{v.Z}",
                s => CoordinateFromString(s));

        modelBuilder.Entity<ProblemDto>()
            .Property(p => p.End)
            .HasConversion(
                v => $"{v.X},{v.Y},{v.Z}",
                s => CoordinateFromString(s));

        modelBuilder.Entity<ProblemDto>()
            .Property(p => p.StartDir)
            .HasConversion<string>(); // Direction → string
    }

    static Vector3 Vector3FromString(string s)
    {
        var parts = s.Split(',');
        return new Vector3(
            float.Parse(parts[0]),
            float.Parse(parts[1]),
            float.Parse(parts[2])
        );
    }

    static Coordinate CoordinateFromString(string s)
    {
        var parts = s.Split(',');
        return new Coordinate(
            int.Parse(parts[0]),
            int.Parse(parts[1]),
            int.Parse(parts[2])
        );
    }
}
