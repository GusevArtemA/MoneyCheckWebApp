using System;
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

        public virtual DbSet<Debt> Debts { get; set; }
        public virtual DbSet<Friend> Friends { get; set; }
        public virtual DbSet<Purchase> Purchases { get; set; }
        public virtual DbSet<PurchaseShopItemTransfer> PurchaseShopItemTransfers { get; set; }
        public virtual DbSet<Shop> Shops { get; set; }
        public virtual DbSet<ShopItem> ShopItems { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserAuthToken> UserAuthTokens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MoneyCheckDb;Trusted_Connection=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Debt>(entity =>
            {
                entity.HasOne(d => d.FromNavigation)
                    .WithMany(p => p.DebtFromNavigations)
                    .HasForeignKey(d => d.From)
                    .HasConstraintName("FK__Debts__From__35BCFE0A");

                entity.HasOne(d => d.Purchase)
                    .WithMany(p => p.Debts)
                    .HasForeignKey(d => d.PurchaseId)
                    .HasConstraintName("FK__Debts__PurchaseI__36B12243");

                entity.HasOne(d => d.ToNavigation)
                    .WithMany(p => p.DebtToNavigations)
                    .HasForeignKey(d => d.To)
                    .HasConstraintName("FK__Debts__To__34C8D9D1");
            });

            modelBuilder.Entity<Friend>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.FriendAid).HasColumnName("FriendAId");

                entity.Property(e => e.FriendBid).HasColumnName("FriendBId");

                entity.HasOne(d => d.FriendA)
                    .WithMany()
                    .HasForeignKey(d => d.FriendAid)
                    .HasConstraintName("FK__Friends__FriendA__30F848ED");

                entity.HasOne(d => d.FriendB)
                    .WithMany()
                    .HasForeignKey(d => d.FriendBid)
                    .HasConstraintName("FK__Friends__FriendB__31EC6D26");
            });

            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Purchases)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__Purchases__Custo__276EDEB3");
            });

            modelBuilder.Entity<PurchaseShopItemTransfer>(entity =>
            {
                entity.HasNoKey();

                entity.HasOne(d => d.Purchase)
                    .WithMany()
                    .HasForeignKey(d => d.PurchaseId)
                    .HasConstraintName("FK__PurchaseS__Purch__2F10007B");

                entity.HasOne(d => d.ShopItem)
                    .WithMany()
                    .HasForeignKey(d => d.ShopItemId)
                    .HasConstraintName("FK__PurchaseS__ShopI__2E1BDC42");
            });

            modelBuilder.Entity<Shop>(entity =>
            {
                entity.Property(e => e.ShopName).HasMaxLength(50);
            });

            modelBuilder.Entity<ShopItem>(entity =>
            {
                entity.Property(e => e.ItemName).HasMaxLength(50);

                entity.HasOne(d => d.Shop)
                    .WithMany(p => p.ShopItems)
                    .HasForeignKey(d => d.ShopId)
                    .HasConstraintName("FK__ShopItems__ShopI__2C3393D0");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.PasswordMd5Hash)
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.Username).HasMaxLength(32);
            });

            modelBuilder.Entity<UserAuthToken>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Token)
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__UserAuthT__UserI__24927208");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
