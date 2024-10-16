﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using server.Models;

#nullable disable

namespace webapi.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241016151415_InitialCreate0")]
    partial class InitialCreate0
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("webapi.Models.Coordinate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Latitude")
                        .HasColumnType("numeric");

                    b.Property<decimal>("Longitude")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.ToTable("Coordinate");
                });

            modelBuilder.Entity("webapi.Models.Entities.AlertSettingEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("DisasterType")
                        .HasColumnType("integer");

                    b.Property<int>("RegionID")
                        .HasColumnType("integer");

                    b.Property<int>("ThresholdScore")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("AlertSettings");
                });

            modelBuilder.Entity("webapi.Models.Entities.RegionEntity", b =>
                {
                    b.Property<string>("RegionId")
                        .HasColumnType("text");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int[]>("DisasterTypes")
                        .HasColumnType("integer[]");

                    b.Property<int?>("LocationId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("RegionId");

                    b.HasIndex("LocationId");

                    b.ToTable("Regions");
                });

            modelBuilder.Entity("webapi.Models.Entities.RegionEntity", b =>
                {
                    b.HasOne("webapi.Models.Coordinate", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId");

                    b.Navigation("Location");
                });
#pragma warning restore 612, 618
        }
    }
}
