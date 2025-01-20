﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Youxel.Check.LicensesGenerator.Infrastructure.Data;

#nullable disable

namespace Youxel.Check.LicensesGenerator.Migrations
{
    [DbContext(typeof(LicenseDbContext))]
    partial class LicenseDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.36")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Youxel.Check.LicensesGenerator.Entities.License", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CompanyName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("DatabaseName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DatabaseServer")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ExpiryDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("LicenseKey")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MachineKey")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Module")
                        .HasColumnType("int");

                    b.Property<int?>("NumberOfAdminUsers")
                        .HasColumnType("int");

                    b.Property<int>("NumberOfAssets")
                        .HasColumnType("int");

                    b.Property<int?>("NumberOfEndUsers")
                        .HasColumnType("int");

                    b.Property<int?>("NumberOfLocationUsers")
                        .HasColumnType("int");

                    b.Property<int>("NumberOfLocations")
                        .HasColumnType("int");

                    b.Property<int?>("NumberOfUnitUsers")
                        .HasColumnType("int");

                    b.Property<int>("NumberOfUsers")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Licenses");
                });
#pragma warning restore 612, 618
        }
    }
}
