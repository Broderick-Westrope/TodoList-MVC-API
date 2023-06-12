﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TodoList.MVC.API;

#nullable disable

namespace TodoList.MVC.API.Migrations.TodoDb
{
    [DbContext(typeof(TodoContext))]
    [Migration("20230610004727_Remove-userId-FKs")]
    partial class RemoveuserIdFKs
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("TodoList.MVC.API.Models.UserAggregate", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("TodoList.MVC.API.Models.UserAggregate", b =>
                {
                    b.OwnsMany("TodoList.MVC.API.Models.Project", "Projects", b1 =>
                        {
                            b1.Property<Guid>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Title")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<Guid>("UserAggregateRootId")
                                .HasColumnType("uniqueidentifier");

                            b1.HasKey("Id");

                            b1.HasIndex("UserAggregateRootId");

                            b1.ToTable("Projects");

                            b1.WithOwner()
                                .HasForeignKey("UserAggregateRootId");
                        });

                    b.OwnsMany("TodoList.MVC.API.Models.TodoItem", "TodoItems", b1 =>
                        {
                            b1.Property<Guid>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Description")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<DateTime>("DueDate")
                                .HasColumnType("datetime2");

                            b1.Property<bool>("IsCompleted")
                                .HasColumnType("bit");

                            b1.Property<string>("Title")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<Guid>("UserAggregateRootId")
                                .HasColumnType("uniqueidentifier");

                            b1.HasKey("Id");

                            b1.HasIndex("UserAggregateRootId");

                            b1.ToTable("TodoItems");

                            b1.WithOwner()
                                .HasForeignKey("UserAggregateRootId");
                        });

                    b.Navigation("Projects");

                    b.Navigation("TodoItems");
                });
#pragma warning restore 612, 618
        }
    }
}
