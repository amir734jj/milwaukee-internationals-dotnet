using System;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace DAL.Utilities
{
    public sealed class EntityDbContext: DbContext
    {
        public DbSet<Student> Students { get; set; }
        
        public DbSet<User> Users { get; set; }
        
        public DbSet<Driver> Drivers { get; set; }
        
        public DbSet<Host> Hosts { get; set; }

        private readonly Action<DbContextOptionsBuilder> _onConfiguring;

        /// <summary>
        /// Constructor that will be called by startup.cs
        /// </summary>
        /// <param name="dbContextOptionsBuilderAction"></param>
        public EntityDbContext(Action<DbContextOptionsBuilder> dbContextOptionsBuilderAction)
        {
            _onConfiguring = dbContextOptionsBuilderAction;
            
            Database.EnsureCreated();
            Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _onConfiguring(optionsBuilder);
        }
    }
}