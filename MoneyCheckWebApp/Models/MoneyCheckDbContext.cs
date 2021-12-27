using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;

#nullable disable

namespace MoneyCheckWebApp.Models
{
    public partial class MoneyCheckDbContext : DbContext
    {
        public MoneyCheckDbContext()
        {
        }

        public MoneyCheckDbContext(DbContextOptions<MoneyCheckDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Debt> Debts { get; set; }
        public virtual DbSet<Debtor> Debtors { get; set; }
        public virtual DbSet<Purchase> Purchases { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserAuthToken> UserAuthTokens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .LogTo(Console.WriteLine,
                          (eventId, logLevel) => logLevel > LogLevel.Information
                                              || eventId == RelationalEventId.CommandExecuting)
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors();

                optionsBuilder.UseLazyLoadingProxies();

                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MoneyCheckDb;Trusted_Connection=true");

            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.SubCategory)
                    .WithMany(p => p.InverseSubCategory)
                    .HasForeignKey(d => d.SubCategoryId)
                    .HasConstraintName("FK__Categorie__SubCa__59FA5E80");
            });

            modelBuilder.Entity<Debt>(entity =>
            {
                entity.HasKey(e => e.DebtId)
                    .HasName("Debts_pk")
                    .IsClustered(false);

                entity.HasOne(d => d.Debtor)
                    .WithMany(p => p.Debts)
                    .HasForeignKey(d => d.DebtorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Debts__DebtorId__5165187F");

                entity.HasOne(d => d.Purchase)
                    .WithMany(p => p.Debts)
                    .HasForeignKey(d => d.PurchaseId)
                    .HasConstraintName("FK__Debts__PurchaseI__52593CB8");
            });

            modelBuilder.Entity<Debtor>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.NaturalMask)
                    .WithMany(p => p.Debtors)
                    .HasForeignKey(d => d.NaturalMaskId)
                    .HasConstraintName("FK__Debtors__Natural__4F7CD00D");
            });

            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.Property(e => e.Latitude).HasColumnType("decimal(8, 6)");

                entity.Property(e => e.Longitude).HasColumnType("decimal(9, 6)");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Purchases)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Purchases__Categ__5AEE82B9");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Purchases)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Purchases__Custo__276EDEB3");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.PasswordMd5Hash)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(32);
            });

            modelBuilder.Entity<UserAuthToken>(entity =>
            {
                entity.HasKey(e => e.Token)
                    .HasName("UserAuthTokens_pk")
                    .IsClustered(false);

                entity.Property(e => e.Token)
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserAuthTokens)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__UserAuthT__UserI__24927208");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
