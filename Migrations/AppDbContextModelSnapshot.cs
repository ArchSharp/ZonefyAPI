﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ZonefyDotnet.Database;

#nullable disable

namespace ZonefyDotnet.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Domain.Entities.Token.RefreshToken", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("ExpirationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("ZonefyDotnet.Entities.ChatMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("ChatIdentifier")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsRead")
                        .HasColumnType("boolean");

                    b.Property<Guid>("PropertyId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ReceiverId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("SenderId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("ChatMessages");
                });

            modelBuilder.Entity("ZonefyDotnet.Entities.ForgotPassword", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("ForgotToken")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("ForgotPasswords");
                });

            modelBuilder.Entity("ZonefyDotnet.Entities.HouseProperty", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CheckInTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("CheckOutTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatorEmail")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("Dimension")
                        .HasColumnType("double precision");

                    b.Property<int>("Guests")
                        .HasColumnType("integer");

                    b.Property<bool>("IsApproved")
                        .HasColumnType("boolean");

                    b.Property<string>("OwnerName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("OwnerPhoneNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ParkingLot")
                        .HasColumnType("integer");

                    b.Property<string>("PropertyDescription")
                        .IsRequired()
                        .HasColumnType("text");

                    b.PrimitiveCollection<List<string>>("PropertyImageUrl")
                        .HasColumnType("text[]");

                    b.Property<string>("PropertyLocation")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PropertyName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("PropertyPrice")
                        .HasColumnType("double precision");

                    b.Property<string>("PropertyType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ToiletNumber")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("HouseProperties");
                });

            modelBuilder.Entity("ZonefyDotnet.Entities.PropertyStatistics", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatorEmail")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("PropertyId")
                        .HasColumnType("uuid");

                    b.Property<string>("PropertyName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserEmail")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("PropertyStatistics");
                });

            modelBuilder.Entity("ZonefyDotnet.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsEmailVerified")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsTwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TwoFactorSecretKey")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("VerificationCode")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
