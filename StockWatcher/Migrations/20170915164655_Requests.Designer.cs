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
    [Migration("20170915164655_Requests")]
    partial class Requests
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452");

            modelBuilder.Entity("StockWatcher.Model.Schemas.Company", b =>
                {
                    b.Property<string>("Symbol");

                    b.Property<string>("Adrtso")
                        .HasColumnName("ADRTSO");

                    b.Property<string>("IPOyear")
                        .HasColumnName("IPOyear");

                    b.Property<string>("Industry");

                    b.Property<string>("LastSale");

                    b.Property<string>("MarketCap");

                    b.Property<string>("Name")
                        .HasColumnName("Name");

                    b.Property<string>("Sector");

                    b.Property<string>("SummaryQuote");

                    b.HasKey("Symbol");

                    b.ToTable("companies");
                });

            modelBuilder.Entity("StockWatcher.Model.Schemas.RequestRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("Price");

                    b.Property<string>("RequestId")
                        .IsRequired();

                    b.Property<string>("TwilioBinding")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Requests");
                });
#pragma warning restore 612, 618
        }
    }
}
