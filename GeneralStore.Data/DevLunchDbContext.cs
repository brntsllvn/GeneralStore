﻿using System.Data.Common;
using System.Data.Entity;
using DevLunch.Data.Models;

namespace DevLunch.Data
{
    public class DevLunchDbContext:DbContext
    {
        public DevLunchDbContext()
        {
            
        }

        public DevLunchDbContext(DbConnection connection) : base(connection,contextOwnsConnection:true)
        {
            
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<Restaurant> Restaurants { get; set; } 

     
    }
}