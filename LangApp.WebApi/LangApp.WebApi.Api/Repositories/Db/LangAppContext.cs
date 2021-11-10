using LangApp.Shared.Models;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace LangApp.WebApi.Api.Repositories.Db
{
    public partial class LangAppContext : DbContext
    {
        public static string ConnectionString { get; set; }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<CategoryName> CategoryNames { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<Level> Levels { get; set; }
        public virtual DbSet<User> Users { get; set; }

        public LangAppContext(DbContextOptions<LangAppContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(ConnectionString, ServerVersion.AutoDetect(ConnectionString));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_unicode_ci");

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.HasIndex(e => e.Email, "email")
                    .IsUnique();

                entity.HasIndex(e => e.Username, "login")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("email");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("username")
                    .UseCollation("utf8mb4_bin");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("password")
                    .UseCollation("utf8mb4_bin");

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasColumnType("enum('USER','ADMIN')")
                    .HasColumnName("role");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
