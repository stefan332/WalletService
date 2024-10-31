using Data.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class AppDbContext : IdentityDbContext, IDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        public DbSet<Wallet> Wallets { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<TransactionType> TransactionTypes { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Wallet>(entity =>
            {
                entity.HasKey(w => w.Id);

                // One-to-one relationship with User
                entity.HasOne(w => w.User)
                      .WithOne()
                      .HasForeignKey<Wallet>(w => w.UserId);

                // RowVersion for concurrency control
                entity.Property(w => w.RowVersion)
                      .IsRowVersion();

                entity.Property(w => w.Balance)
                      .IsRequired()
                      .HasColumnType("decimal(18,2)");
            });

            builder.Entity<TransactionType>(entity =>
            {
                entity.HasKey(tt => tt.TransactionTypeId);
                entity.Property(tt => tt.Name)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(tt => tt.Description)
                      .HasMaxLength(200);
            });

            // Transaction configuration
            builder.Entity<Transaction>(entity =>
            {
                entity.HasKey(t => t.TransactionId);

                // Foreign Key to Wallet
                entity.HasOne(t => t.Wallet)
                      .WithMany(w => w.Transactions)
                      .HasForeignKey(t => t.WalletId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Foreign Key to TransactionType
                entity.HasOne(t => t.TransactionType)
                      .WithMany(tt => tt.Transactions)
                      .HasForeignKey(t => t.TransactionTypeId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(t => t.Amount)
                      .IsRequired()
                      .HasColumnType("decimal(18,2)");

                entity.Property(t => t.Date)
                      .IsRequired()
                      .HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
}