using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using YandexGeo.Models;

namespace YandexGeo
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Street> Streets { get; set; }
        public DbSet<House> Houses { get; set; }
        public ApplicationContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=dev-ws-v-07;Port=5432;Database=vote-dev;Username=mobnius;Password=mobnius-0");
        }
    }
}
