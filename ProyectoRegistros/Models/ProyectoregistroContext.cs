using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace ProyectoRegistros.Models;

public partial class ProyectoregistroContext : DbContext
{
    public ProyectoregistroContext()
    {
    }

    public ProyectoregistroContext(DbContextOptions<ProyectoregistroContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Alumno> Alumnos { get; set; }

    public virtual DbSet<Listatallere> Listatalleres { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<Taller> Tallers { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Alumno>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("alumno");

            entity.Property(e => e.Direccion).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(45);
            entity.Property(e => e.FechaCumple)
                .HasColumnType("datetime")
                .HasColumnName("fechaCumple");
            entity.Property(e => e.Nombre).HasMaxLength(45);
            entity.Property(e => e.NumContacto).HasMaxLength(15);
            entity.Property(e => e.NumSecundario).HasMaxLength(15);
            entity.Property(e => e.Padecimientos).HasMaxLength(45);
            entity.Property(e => e.Tutor).HasMaxLength(30);
        });

        modelBuilder.Entity<Listatallere>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("listatalleres");

            entity.HasIndex(e => e.IdAlumno, "fkListaAlumno_idx");

            entity.HasIndex(e => e.IdTaller, "fkListaTaller_idx");

            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasOne(d => d.IdAlumnoNavigation).WithMany(p => p.Listatalleres)
                .HasForeignKey(d => d.IdAlumno)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fkListaAlumno");

            entity.HasOne(d => d.IdTallerNavigation).WithMany(p => p.Listatalleres)
                .HasForeignKey(d => d.IdTaller)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fkListaTaller");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("rol");

            entity.Property(e => e.Rol1)
                .HasMaxLength(15)
                .HasColumnName("Rol");
        });

        modelBuilder.Entity<Taller>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("taller");

            entity.HasIndex(e => e.IdUsuario, "fkTallerUsuario_idx");

            entity.Property(e => e.Costo).HasPrecision(10, 2);
            entity.Property(e => e.Dias).HasMaxLength(40);
            entity.Property(e => e.EdadMax).HasColumnName("Edad_max");
            entity.Property(e => e.EdadMin).HasColumnName("Edad_min");
            entity.Property(e => e.HoraFinal)
                .HasColumnType("time")
                .HasColumnName("Hora_final");
            entity.Property(e => e.HoraInicio)
                .HasColumnType("time")
                .HasColumnName("Hora_inicio");
            entity.Property(e => e.LugaresDisp).HasColumnName("Lugares_Disp");
            entity.Property(e => e.Nombre).HasMaxLength(30);

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Tallers)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fkTallerUsuario");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("usuario");

            entity.HasIndex(e => e.IdRol, "fkUsuarioRol_idx");

            entity.Property(e => e.Contraseña)
                .HasMaxLength(45)
                .HasColumnName("contraseña");
            entity.Property(e => e.Correo).HasMaxLength(50);
            entity.Property(e => e.Nombre).HasMaxLength(45);
            entity.Property(e => e.NumTel)
                .HasMaxLength(15)
                .HasColumnName("numTel");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fkUsuarioRol");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
