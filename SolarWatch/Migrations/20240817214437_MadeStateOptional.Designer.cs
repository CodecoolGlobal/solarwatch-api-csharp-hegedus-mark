﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SolarWatch.Data;

#nullable disable

namespace SolarWatch.Migrations
{
    [DbContext(typeof(SolarWatchDbContext))]
    [Migration("20240817214437_MadeStateOptional")]
    partial class MadeStateOptional
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("SolarWatch.Data.Models.City", b =>
                {
                    b.Property<int>("CityId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CityId"));

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Latitude")
                        .HasColumnType("float");

                    b.Property<double>("Longitude")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("State")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CityId");

                    b.ToTable("Cities");
                });

            modelBuilder.Entity("SolarWatch.Data.Models.City", b =>
                {
                    b.OwnsOne("SolarWatch.Data.Models.SunriseSunset", "SunriseSunset", b1 =>
                        {
                            b1.Property<int>("CityId")
                                .HasColumnType("int");

                            b1.Property<TimeOnly>("Sunrise")
                                .HasColumnType("TIME")
                                .HasColumnName("Sunrise");

                            b1.Property<TimeOnly>("Sunset")
                                .HasColumnType("TIME")
                                .HasColumnName("Sunset");

                            b1.HasKey("CityId");

                            b1.ToTable("Cities");

                            b1.WithOwner()
                                .HasForeignKey("CityId");
                        });

                    b.Navigation("SunriseSunset")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
