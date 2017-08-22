﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using StockWatcher.Model;
using System;

namespace StockWatcher.Migrations
{
    [DbContext(typeof(StockDbContext))]
    [Migration("20170820195655_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452");

            modelBuilder.Entity("StockWatcher.Model.Schemas.Stock", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Equity")
                        .IsRequired()
                        .HasMaxLength(4);

                    b.Property<double>("Price");

                    b.Property<string>("RequestId");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.HasKey("Id");

                    b.ToTable("StockRequests");
                });

            modelBuilder.Entity("StockWatcher.Model.Schemas.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(12);

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<string>("Uuid");

                    b.HasKey("Id");

                    b.HasIndex("Phone", "Username")
                        .IsUnique();

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
