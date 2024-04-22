using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SagaPatternMichael.Payment.Infrastructure.Core.Entities;

public partial class PaymentServiceContext : DbContext
{
    public PaymentServiceContext()
    {
    }

    public PaymentServiceContext(DbContextOptions<PaymentServiceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<EventBox> EventBoxes { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=nguynminhvu;Initial Catalog=PaymentService;Integrated Security=True;Encrypt=False;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EventBox>(entity =>
        {
            entity.ToTable("EventBox");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
