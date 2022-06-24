﻿// <auto-generated />
using System;
using BookingService.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BookingService.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20220623063745_BookDate")]
    partial class BookDate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("BookingService.Entity.BookingDetailEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("Age")
                        .HasColumnType("int");

                    b.Property<int>("BookingId")
                        .HasColumnType("int");

                    b.Property<string>("Gender")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MealType")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SeatNo")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("BookingDetail");
                });

            modelBuilder.Entity("BookingService.Entity.BookingEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<DateTime>("BookingDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("FlightId")
                        .HasColumnType("int");

                    b.Property<int>("NoOfSeat")
                        .HasColumnType("int");

                    b.Property<string>("PNR")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Booking");
                });
#pragma warning restore 612, 618
        }
    }
}
