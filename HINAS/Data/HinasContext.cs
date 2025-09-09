using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HINAS;

public partial class HinasContext : DbContext
{
    public HinasContext()
    {
    }

    public HinasContext(DbContextOptions<HinasContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CheckList> CheckLists { get; set; }

    public virtual DbSet<Marker> Markers { get; set; }

    public virtual DbSet<Planner> Planners { get; set; }

    public virtual DbSet<Supply> Supplies { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-11PGGLI\\SQLEXPRESS;Database=HINAS;Integrated Security=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CheckList>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CheckLis__3214EC276BCF798B");

            entity.ToTable("CheckList");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.PlannerId).HasColumnName("PlannerID");

            entity.HasOne(d => d.Planner).WithMany(p => p.CheckLists)
                .HasForeignKey(d => d.PlannerId)
                .HasConstraintName("FK__CheckList__Plann__267ABA7A");
        });

        modelBuilder.Entity<Marker>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Markers__3214EC27A634A871");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ImagePath).HasMaxLength(1000);
            entity.Property(e => e.MarkerName).HasMaxLength(255);
        });

        modelBuilder.Entity<Planner>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Planner__3214EC274C96012D");

            entity.ToTable("Planner");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Route).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(255);
        });

        modelBuilder.Entity<Supply>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Supplies__3214EC2731AF5B11");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
