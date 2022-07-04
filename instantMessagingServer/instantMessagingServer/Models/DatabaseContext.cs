using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using instantMessagingCore.Models.Dto;

namespace instantMessagingServer.Models
{
    public class DatabaseContext : DbContext
    {
        private readonly IConfiguration Configuration;

        public DbSet<Users> Users { get; set; }
        public DbSet<Tokens> Tokens { get; set; }
        public DbSet<PublicKeys> PublicKeys { get; set; }
        public DbSet<Logs> Logs { get; set; }
        public DbSet<Friends> Friends { get; set; }
        public DbSet<Peers> Peers { get; set; }

        public DatabaseContext(IConfiguration Configuration)
        {
            this.Configuration = Configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // SQLLite
            //options.UseSqlite(Configuration.GetConnectionString("sqlite"));

            // MariaDB/MySQL
            string connectionString = Configuration.GetConnectionString("sql");
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>().HasKey(u => u.Id);
            modelBuilder.Entity<Users>().Property(u => u.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Users>().HasIndex(u => u.Username).IsUnique();

            modelBuilder.Entity<Tokens>().HasKey(t => t.UserId);

            modelBuilder.Entity<PublicKeys>().HasKey(p => p.UserId);

            modelBuilder.Entity<Logs>().HasKey(l => l.Id);
            modelBuilder.Entity<Logs>().Property(l => l.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<Friends>().HasKey(f => new { f.UserId, f.FriendId });

            modelBuilder.Entity<Peers>().HasKey(p => p.UserId);
        }
    }
}
