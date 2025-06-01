using Luftborn.Domain.Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luftborn.Domain
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<RoleTranslation> RoleTranslations { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<Article> Articles { get; set; }
        public DbSet<ArticleTranslation> ArticleTranslations { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure RoleTranslation entity
            modelBuilder.Entity<RoleTranslation>()
                .HasOne(rt => rt.Role)
                .WithMany()
                .HasForeignKey(rt => rt.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RoleTranslation>()
                .HasIndex(rt => new { rt.RoleId, rt.LanguageKey })
                .IsUnique();

            // Configure RefreshToken entity
            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RefreshToken>()
                .HasIndex(rt => rt.Token)
                .IsUnique();

            // Configure Article entity
            modelBuilder.Entity<Article>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Configure relationship with Writer (IdentityUser)
                entity.HasOne(e => e.Writer)
                    .WithMany()
                    .HasForeignKey(e => e.WriterId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Configure relationship with ArticleTranslations
                entity.HasMany(e => e.Translations)
                    .WithOne(e => e.Article)
                    .HasForeignKey(e => e.ArticleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure ArticleTranslation entity
            modelBuilder.Entity<ArticleTranslation>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Add unique constraint for ArticleId and LanguageKey combination
                entity.HasIndex(e => new { e.ArticleId, e.LanguageKey }).IsUnique();

                // Configure required fields
                entity.Property(e => e.Title).IsRequired();
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.LanguageKey).IsRequired().HasMaxLength(10);
            });
        }
    }
}
