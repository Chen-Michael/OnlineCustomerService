using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EF
{
    public partial class OnlineCustomerServiceContext : DbContext
    {
        public OnlineCustomerServiceContext()
        {
        }

        public OnlineCustomerServiceContext(DbContextOptions<OnlineCustomerServiceContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Admin> Admin { get; set; }
        public virtual DbSet<DialogueRecord> DialogueRecord { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Account)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(15);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<DialogueRecord>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AdminId).HasColumnName("AdminID");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Mail).HasMaxLength(100);

                entity.Property(e => e.Phone).HasMaxLength(30);

                entity.Property(e => e.Question)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.Record)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(15);
            });
        }
    }
}
