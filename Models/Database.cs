﻿using Geo.Models.Db;
using GeoMeasure.Models.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geo.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Project> Projects { get; set; } = null!;
        public DbSet<Area> Areas { get; set; } = null!;
        public DbSet<AreaPoint> AreaPoints { get; set; } = null!;
        public DbSet<Operator> Operators { get; set; } = null!;
        public DbSet<Profile> Profiles { get; set; } = null!;
        public DbSet<ProfilePoint> ProfilePoints { get; set; } = null!;
        public DbSet<Picket> Pickets { get; set; } = null!;

        public static string ConnectionString { get; set; }

        private static ApplicationContext? instance;
        public static ApplicationContext getInstance()
        {
            if (instance == null)
            {
                instance = new ApplicationContext(ConnectionString.ToString());
                //instance.Database.EnsureDeleted();
                //var exists = instance.Database.EnsureCreated();

                instance.Customers.Load();
                instance.Projects.Load();
                instance.AreaPoints.Load();
                instance.Areas.Load();
                instance.Operators.Load();
                instance.ProfilePoints.Load();
                instance.Profiles.Load();
                instance.Pickets.Load();
                //if (exists)
                //    instance.Customers.Add(DefaultData);
                instance.SaveChanges();
            }
            return instance;
        }

        public ApplicationContext() { }

        public ApplicationContext(string connectionString)
        {
            ConnectionString = connectionString;
        }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(@"Server=DESKTOP-6R0PLHC\MSSQLSERVER2024;Database={ConnectionString};Trusted_Connection=True; TrustServerCertificate=True");

        //}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer($@"Server=DBSRV\GLO2023;Database={ConnectionString};Trusted_Connection=True;TrustServerCertificate=True");
        }
        //static Customer DefaultData = new Customer()
        //{
        //    Name = "ООО МойДом",
        //    Phone = "7912300434",
        //    Projects = new()
        //    {
        //        new() { Name = "Алмаз 2", Address="Пушкина 16", Areas = new()
        //            {
        //                new() { Name="Поле кукурузы", Points = new()
        //                {
        //                    new() { X=0, Y=0 },
        //                    new() { X=35, Y=2 },
        //                    new() { X=29, Y=25 },
        //                    new() { X=3, Y=27 },
        //                },
        //                    Profiles=new() {
        //                        new()
        //                        {
        //                            Operator=new() { Name="Илья", Surname="Буров" },
        //                            Points= new()
        //                            {
        //                                new () {X=2, Y=2 },
        //                                new () {X=8, Y=4 },
        //                                new () {X=17, Y=4 },
        //                                new () {X=25, Y=6 },
        //                                new () {X=32, Y=7 },
        //                            },
        //                            Pickets = new()
        //                            {
        //                                new () {X=30, Y=3, Ra=0, Th =0},
        //                                new () {X=32, Y=32, Ra=5, Th =3},
        //                                new () {X=33, Y=33, Ra=8,Th =10},
        //                                new () {X=34, Y=0, Ra=4, Th = 12},
        //                                new () {X=33, Y=0, Ra=7, Th = 23},
        //                                 new () {X=33, Y=24, Ra=14, Th = 25},
        //                            }
        //                        },
        //                        new()
        //                        {
        //                            Points= new()
        //                            {
        //                                new () {X=4, Y=20 },
        //                                new () {X=7, Y=15 },
        //                                new () {X=11, Y=17 },
        //                                new () {X=17, Y=22 },
        //                                new () {X=23, Y=18 },
        //                            }
        //                        }
        //                    } },
        //                new() { Name="Огород", Points = new()
        //                    {
        //                        new() { X=32, Y=-22 },
        //                        new() { X=54, Y=-35 },
        //                        new() { X=45, Y=-22 },
        //                        new() { X=31, Y=-17 },
        //                        new() { X=12, Y=-15 },
        //                    }
        //                },
        //                new() { Name="Пустая" }
        //            }
        //        }
        //    }
        //};
    }
}
