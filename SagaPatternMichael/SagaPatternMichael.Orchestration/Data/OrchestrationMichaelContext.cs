using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SagaPatternMichael.Orchestration.Data;

public partial class OrchestrationMichaelContext : DbContext
{
    public OrchestrationMichaelContext()
    {
    }

    public OrchestrationMichaelContext(DbContextOptions<OrchestrationMichaelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<EventBox> EventBoxes { get; set; }

    public virtual DbSet<EventErrorBox> EventErrorBoxes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=nguynminhvu;Initial Catalog=OrchestrationMichael;Integrated Security=True;Encrypt=False;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EventBox>(entity =>
        {
            entity.ToTable("EventBox");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<EventErrorBox>(entity =>
        {
            entity.ToTable("EventErrorBox");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
