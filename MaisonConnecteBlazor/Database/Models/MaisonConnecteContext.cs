using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MaisonConnecteBlazor.Database.Models;

public partial class MaisonConnecteContext : DbContext
{
    public MaisonConnecteContext()
    {
    }

    public MaisonConnecteContext(DbContextOptions<MaisonConnecteContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Enregistrement> Enregistrements { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");

        modelBuilder.Entity<Enregistrement>(entity =>
        {
            entity.ToTable("enregistrement");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date)
                .HasColumnType("date")
                .HasColumnName("date");
            entity.Property(e => e.FluxVideo).HasColumnName("flux_video");
            entity.Property(e => e.Thumbnail).HasColumnName("thumbnail");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.ToTable("events");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Data)
                .HasColumnType("text")
                .HasColumnName("data");
            entity.Property(e => e.Date)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.Event1).HasColumnName("event");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
