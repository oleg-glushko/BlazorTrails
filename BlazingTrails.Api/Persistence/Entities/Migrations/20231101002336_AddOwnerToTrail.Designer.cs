﻿// <auto-generated />
using BlazingTrails.Api.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BlazingTrails.Api.Persistence.Entities.Migrations
{
    [DbContext(typeof(BlazingTrailsContext))]
    [Migration("20231101002336_AddOwnerToTrail")]
    partial class AddOwnerToTrail
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0-rc.2.23480.1");

            modelBuilder.Entity("BlazingTrails.Api.Persistence.Entities.Trail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Image")
                        .HasColumnType("TEXT");

                    b.Property<int>("Length")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Owner")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("TimeInMinutes")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Trails");
                });

            modelBuilder.Entity("BlazingTrails.Api.Persistence.Entities.Waypoint", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Latitude")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Longitude")
                        .HasColumnType("TEXT");

                    b.Property<int>("TrailId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("TrailId");

                    b.ToTable("Waypoints");
                });

            modelBuilder.Entity("BlazingTrails.Api.Persistence.Entities.Waypoint", b =>
                {
                    b.HasOne("BlazingTrails.Api.Persistence.Entities.Trail", "Trail")
                        .WithMany("Waypoints")
                        .HasForeignKey("TrailId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Trail");
                });

            modelBuilder.Entity("BlazingTrails.Api.Persistence.Entities.Trail", b =>
                {
                    b.Navigation("Waypoints");
                });
#pragma warning restore 612, 618
        }
    }
}
