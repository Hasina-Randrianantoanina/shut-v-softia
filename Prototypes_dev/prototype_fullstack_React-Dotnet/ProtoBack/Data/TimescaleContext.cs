using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ProtoBack.Models.Timescale;

namespace ProtoBack.Data;

public partial class TimescaleContext : DbContext
{
    public TimescaleContext()
    {
    }

    public TimescaleContext(DbContextOptions<TimescaleContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TsAna> TsAnas { get; set; }

    public virtual DbSet<TsTor> TsTors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Name=ConnectionStrings:TimescaleDb");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("timescaledb");

        modelBuilder.Entity<TsAna>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ts_ana");

            entity.HasIndex(e => e.DateTime, "ts_ana_date_time_idx").IsDescending();

            entity.Property(e => e.DateTime).HasColumnName("date_time");
            entity.Property(e => e.Libelle)
                .HasMaxLength(50)
                .HasColumnName("libelle");
            entity.Property(e => e.Valeur).HasColumnName("valeur");
        });

        modelBuilder.Entity<TsTor>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ts_tor");

            entity.HasIndex(e => e.DateTime, "ts_tor_date_time_idx").IsDescending();

            entity.Property(e => e.DateTime).HasColumnName("date_time");
            entity.Property(e => e.Libelle)
                .HasMaxLength(50)
                .HasColumnName("libelle");
            entity.Property(e => e.Valeur).HasColumnName("valeur");
        });
        modelBuilder.HasSequence("chunk_constraint_name", "_timescaledb_catalog");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
