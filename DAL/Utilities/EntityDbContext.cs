using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Models;
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => _onConfiguring(optionsBuilder);
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
/*            // Configure 1 to many relationship
            modelBuilder.Entity<Driver>()
                .HasMany<Student>()
                .WithOne();

            // Configure 1 to many relationship
            modelBuilder.Entity<Student>()
                .HasOne(x => x.Driver)
                .WithMany()
                .HasForeignKey(x => x.DriverRefId);*/

            base.OnModelCreating(modelBuilder);
        }
    }
}