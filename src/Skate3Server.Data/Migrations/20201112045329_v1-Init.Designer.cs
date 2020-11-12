﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Skate3Server.Data;

namespace Skate3Server.Data.Migrations
{
    [DbContext(typeof(BlazeContext))]
    [Migration("20201112045329_v1-Init")]
    partial class v1Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.10");

            modelBuilder.Entity("Skate3Server.Data.Models.User", b =>
                {
                    b.Property<uint>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("AccountId")
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("ExternalBlob")
                        .HasColumnType("BLOB");

                    b.Property<ulong>("ExternalId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ExternalIdType")
                        .HasColumnType("INTEGER");

                    b.Property<uint>("LastLogin")
                        .HasColumnType("INTEGER");

                    b.Property<long>("ProfileId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}