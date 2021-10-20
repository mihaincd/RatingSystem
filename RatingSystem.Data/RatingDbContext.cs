using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using RatingSystem.Models;

#nullable disable

namespace RatingSystem.Data
{
    public partial class RatingDbContext : DbContext
    {
        public RatingDbContext()
        {
        }

        public RatingDbContext(DbContextOptions<RatingDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<RatingPerGroup> RatingPerGroups { get; set; }
        public virtual DbSet<UsersRating> UsersRatings { get; set; }

     

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<RatingPerGroup>(entity =>
            {
                entity.HasKey(e => e.GroupId);

                entity.ToTable("RatingPerGroup");

                entity.Property(e => e.GroupId).HasMaxLength(50);

                entity.Property(e => e.Category)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.RatingAvg).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<UsersRating>(entity =>
            {
                entity.HasKey(e => new { e.EmailUser, e.GroupId });

                entity.ToTable("UsersRating");

                entity.Property(e => e.EmailUser).HasMaxLength(50);

                entity.Property(e => e.GroupId).HasMaxLength(50);

                entity.Property(e => e.Category)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Rating).HasColumnType("decimal(18, 2)");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
