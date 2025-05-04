using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CharityApp.Models;

public partial class CharityDbContext : DbContext
{
    public CharityDbContext()
    {
    }

    public CharityDbContext(DbContextOptions<CharityDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Donation> Donations { get; set; }

    public virtual DbSet<PasswordReset> PasswordResets { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Donation>(entity =>
        {
            entity.HasKey(e => e.DonationId).HasName("PK__Donation__C5082EFB7F8E8765");
        });

        modelBuilder.Entity<PasswordReset>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Password__3214EC0707D514D3");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C121365E9");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
