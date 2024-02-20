﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace linq.model;

public partial class PostgresContext : DbContext
{
    public PostgresContext()
    {
    }

    public PostgresContext(DbContextOptions<PostgresContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseStudent> CourseStudents { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<RoomCourse> RoomCourses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=db.pupukkgnauphdhrlgttk.supabase.co;Database=postgres;username=postgres;password=X8FzEv.9F2!w9zc;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("auth", "aal_level", new[] { "aal1", "aal2", "aal3" })
            .HasPostgresEnum("auth", "code_challenge_method", new[] { "s256", "plain" })
            .HasPostgresEnum("auth", "factor_status", new[] { "unverified", "verified" })
            .HasPostgresEnum("auth", "factor_type", new[] { "totp", "webauthn" })
            .HasPostgresEnum("pgsodium", "key_status", new[] { "default", "valid", "invalid", "expired" })
            .HasPostgresEnum("pgsodium", "key_type", new[] { "aead-ietf", "aead-det", "hmacsha512", "hmacsha256", "auth", "shorthash", "generichash", "kdf", "secretbox", "secretstream", "stream_xchacha20" })
            .HasPostgresExtension("extensions", "pg_stat_statements")
            .HasPostgresExtension("extensions", "pgcrypto")
            .HasPostgresExtension("extensions", "pgjwt")
            .HasPostgresExtension("extensions", "uuid-ossp")
            .HasPostgresExtension("graphql", "pg_graphql")
            .HasPostgresExtension("pgsodium", "pgsodium")
            .HasPostgresExtension("vault", "supabase_vault");

        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("AspNetRoles_pkey");

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("AspNetRoleClaims_pkey");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("AspNetRoleClaims_RoleId_fkey");
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("AspNetUsers_pkey");

            entity.Property(e => e.Id).HasMaxLength(128);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Email).HasMaxLength(256);    
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.LockoutEnd).HasPrecision(6);
            entity.Property(e => e.LockDate).HasPrecision(6);
            entity.Property(e => e.Image).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("AspNetUserRoles_RoleId_fkey"),
                    l => l.HasOne<AspNetUser>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("AspNetUserRoles_UserId_fkey"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId").HasName("AspNetUserRoles_pkey");
                        j.ToTable("AspNetUserRoles");
                        j.IndexerProperty<string>("UserId").HasMaxLength(128);
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("AspNetUserClaims_pkey");

            entity.Property(e => e.UserId).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("AspNetUserClaims_UserId_fkey");
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey }).HasName("AspNetUserLogins_pkey");

            entity.Property(e => e.UserId).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("AspNetUserLogins_UserId_fkey");
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name }).HasName("AspNetUserTokens_pkey");

            entity.Property(e => e.UserId).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("AspNetUserTokens_UserId_fkey");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("Courses_pkey");

            entity.Property(e => e.CourseId).HasMaxLength(128);
            entity.Property(e => e.CreateDate).HasPrecision(6);//
            entity.Property(e => e.CreatedUserId).HasMaxLength(128);
            entity.Property(e => e.EndDate).HasPrecision(6);
            entity.Property(e => e.Price).HasPrecision(15, 5);
            entity.Property(e => e.StartDate).HasPrecision(6);
            entity.Property(e => e.TeacherId)
                .HasMaxLength(128)
                .HasColumnName("TeacherId ");
            entity.Property(e => e.UpdateDate).HasPrecision(6);
            entity.Property(e => e.UpdatedUserId).HasMaxLength(128);

            entity.HasOne(d => d.Teacher).WithMany(p => p.Courses)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Courses_TeacherId _fkey");
        });

        modelBuilder.Entity<CourseStudent>(entity =>
        {
            entity.HasKey(e => e.CourseStudentId).HasName("CourseStudents_pkey");

            entity.Property(e => e.CourseStudentId).HasMaxLength(128);
            entity.Property(e => e.CourseId).HasMaxLength(128);
            entity.Property(e => e.CreateDate).HasPrecision(6);
            entity.Property(e => e.CreatedUserId).HasMaxLength(128);
            entity.Property(e => e.StudentId).HasMaxLength(128);
            entity.Property(e => e.UpdateDate).HasPrecision(6);
            entity.Property(e => e.UpdatedUserId).HasMaxLength(128);

            entity.HasOne(d => d.Course).WithMany(p => p.CourseStudents)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("CourseStudents_CourseId_fkey");

            entity.HasOne(d => d.Student).WithMany(p => p.CourseStudents)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("CourseStudents_StudentId_fkey");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("Rooms_pkey");

            entity.Property(e => e.RoomId).HasMaxLength(128);
            entity.Property(e => e.CreateDate).HasPrecision(6);
            entity.Property(e => e.CreatedUserId).HasMaxLength(128);
            entity.Property(e => e.UpdateDate).HasPrecision(6);
            entity.Property(e => e.UpdatedUserId).HasMaxLength(128);
        });

        modelBuilder.Entity<RoomCourse>(entity =>
        {
            entity.HasKey(e => e.RoomCourseId).HasName("RoomCourses_pkey");

            entity.Property(e => e.RoomCourseId).HasMaxLength(128);
            entity.Property(e => e.CourseId).HasMaxLength(128);
            entity.Property(e => e.CreateDate).HasPrecision(6);
            entity.Property(e => e.CreatedUserId).HasMaxLength(128);
            entity.Property(e => e.RoomId).HasMaxLength(128);
            entity.Property(e => e.UpdateDate).HasPrecision(6);
            entity.Property(e => e.UpdatedUserId).HasMaxLength(128);

            entity.HasOne(d => d.Course).WithMany(p => p.RoomCourses)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RoomCourses_CourseId_fkey");

            entity.HasOne(d => d.Room).WithMany(p => p.RoomCourses)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RoomCourses_RoomId_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
