using Microsoft.EntityFrameworkCore;
using TMS.Domain.Entities;

namespace TMS.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<Request> Requests { get; set; }
        public DbSet<RequestApprovalLog> RequestApprovalLogs { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasOne<Role>()
                .WithMany()
                .HasForeignKey(u => u.RoleId);

            modelBuilder.Entity<User>()
                .Property(u => u.AccountStatus)
                .HasDefaultValue("INACTIVE");

            modelBuilder.Entity<Request>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Request>()
                .Property(r => r.Email)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<Request>()
                .Property(r => r.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            modelBuilder.Entity<RequestApprovalLog>()
                .HasOne(l => l.Request)
                .WithMany(r => r.ApprovalLogs)
                .HasForeignKey(l => l.RequestId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RequestApprovalLog>()
                .HasOne(l => l.Approver)
                .WithMany()
                .HasForeignKey(l => l.ApproverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RequestApprovalLog>()
                .HasOne(l => l.Role)
                .WithMany()
                .HasForeignKey(l => l.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RequestApprovalLog>()
                .Property(l => l.Decision)
                .HasConversion<string>()
                .HasMaxLength(20);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Teller)
                .WithMany()
                .HasForeignKey(t => t.TellerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<RefreshToken>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RefreshToken>()
                .Property(rt => rt.TokenHash)
                .IsRequired()
                .HasMaxLength(64);

            modelBuilder.Entity<RefreshToken>()
                .HasIndex(rt => rt.TokenHash)
                .IsUnique();
        }
    }
}
