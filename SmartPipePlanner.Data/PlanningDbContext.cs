using System.Numerics;
using Microsoft.EntityFrameworkCore;

namespace SmartPipePlanner.Data;

public class PlanningDbContext : DbContext
{
    public DbSet<Element> Elements { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=planning.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Element>()
            .Property(e => e.Category)
            .HasConversion<string>();

        // GeometryType 轉 string
        modelBuilder.Entity<Element>()
            .OwnsOne(e => e.Geometry, g =>
            {
                g.Property(geo => geo.Type).HasConversion<string>();
                g.Property(geo => geo.Orientation)
                 .HasConversion(
                    v => $"{v.X},{v.Y},{v.Z}",
                    s => Vector3FromString(s));
            });

        // Vector3 轉換成 string 存 SQLite
        modelBuilder.Entity<Element>()
            .Property(e => e.Location)
            .HasConversion(
                v => $"{v.X},{v.Y},{v.Z}",
                s => Vector3FromString(s));
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
}

