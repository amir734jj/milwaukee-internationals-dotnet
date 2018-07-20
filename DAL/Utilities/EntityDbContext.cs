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
            modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership)
                .ToList()
                .ForEach(x => x.DeleteBehavior = DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}