using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using soap.Models;

namespace soap.Data;

public partial class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public virtual DbSet<Tauthor> Tauthors { get; set; }

    public virtual DbSet<Tbook> Tbooks { get; set; }

    public virtual DbSet<Tpublishingcompany> Tpublishingcompanies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tauthor>(entity =>
        {
            entity.HasKey(e => e.NAuthorId);

            entity.ToTable("tauthor");

            entity.Property(e => e.NAuthorId).HasColumnName("nAuthorID");
            entity.Property(e => e.CName)
                .HasColumnType("varchar(40)")
                .HasColumnName("cName");
            entity.Property(e => e.CSurname)
                .HasDefaultValueSql("NULL")
                .HasColumnType("varchar(60)")
                .HasColumnName("cSurname");
        });

        modelBuilder.Entity<Tbook>(entity =>
        {
            entity.HasKey(e => e.NBookId);

            entity.ToTable("tbook");

            entity.HasIndex(e => e.NAuthorId, "idx_tbook_IDX_BOOK_AUTHOR_ID");

            entity.HasIndex(e => e.NPublishingCompanyId, "idx_tbook_IDX_BOOK_PUBLISHING_COMPANY_ID");

            entity.Property(e => e.NBookId).HasColumnName("nBookID");
            entity.Property(e => e.CTitle)
                .HasColumnType("varchar(255)")
                .HasColumnName("cTitle");
            entity.Property(e => e.NAuthorId).HasColumnName("nAuthorID");
            entity.Property(e => e.NPublishingCompanyId).HasColumnName("nPublishingCompanyID");
            entity.Property(e => e.NPublishingYear)
                .HasDefaultValueSql("NULL")
                .HasColumnType("decimal(4,0)")
                .HasColumnName("nPublishingYear");
        });

        modelBuilder.Entity<Tpublishingcompany>(entity =>
        {
            entity.HasKey(e => e.NPublishingCompanyId);

            entity.ToTable("tpublishingcompany");

            entity.Property(e => e.NPublishingCompanyId).HasColumnName("nPublishingCompanyID");
            entity.Property(e => e.CName)
                .HasColumnType("varchar(40)")
                .HasColumnName("cName");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
