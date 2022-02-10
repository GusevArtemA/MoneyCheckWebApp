﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

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
        public virtual DbSet<DefaultLogosForCategory> DefaultLogosForCategories { get; set; }
        public virtual DbSet<Purchase> Purchases { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserAuthToken> UserAuthTokens { get; set; }
        public virtual DbSet<VerifiedCompany> VerifiedCompanies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Logo)
                    .WithMany(p => p.Categories)
                    .HasForeignKey(d => d.LogoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Categorie__LogoI__55F4C372");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.Categories)
                    .HasForeignKey(d => d.OwnerId)
                    .HasConstraintName("FK__Categorie__Owner__01142BA1");

                entity.HasOne(d => d.ParentCategory)
                    .WithMany(p => p.InverseParentCategory)
                    .HasForeignKey(d => d.ParentCategoryId)
                    .HasConstraintName("FK__Categorie__SubCa__59FA5E80");
            });

            modelBuilder.Entity<Debt>(entity =>
            {
                entity.HasKey(e => e.DebtId)
                    .HasName("Debts_pk")
                    .IsClustered(false);

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.HasOne(d => d.Debtor)
                    .WithMany(p => p.Debts)
                    .HasForeignKey(d => d.DebtorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Debts__DebtorId__5165187F");

                entity.HasOne(d => d.Initiator)
                    .WithMany(p => p.Debts)
                    .HasForeignKey(d => d.InitiatorId)
                    .HasConstraintName("FK__Debts__Initiator__14270015");

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

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.Debtors)
                    .HasForeignKey(d => d.OwnerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Debtors__OwnerId__73852659");
            });

            modelBuilder.Entity<DefaultLogosForCategory>(entity =>
            {
                entity.HasIndex(e => e.LogoName, "DefaultLogosForCategories_LogoName_uindex")
                    .IsUnique();

                entity.Property(e => e.LogoName)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.Property(e => e.Amount).HasColumnType("money");

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

                entity.HasOne(d => d.VerifiedCompany)
                    .WithMany(p => p.Purchases)
                    .HasForeignKey(d => d.VerifiedCompanyId)
                    .HasConstraintName("FK__Purchases__Verif__56E8E7AB");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Balance).HasColumnType("money");

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

            modelBuilder.Entity<VerifiedCompany>(entity =>
            {
                entity.Property(e => e.CompanyName).IsRequired();

                entity.Property(e => e.LogoSvg).IsRequired();

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.VerifiedCompanies)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__VerifiedC__Categ__4F47C5E3");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
