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
        
        public DbSet<Event> Events { get; set; }

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
            modelBuilder.Entity<EventStudentRelationship>()
                .HasKey(t => new { t.StudentId, t.EventId });

            modelBuilder.Entity<EventStudentRelationship>()
                .HasOne(pt => pt.Event)
                .WithMany(p => p.Students)
                .HasForeignKey(pt => pt.StudentId);

            modelBuilder.Entity<EventStudentRelationship>()
                .HasOne(pt => pt.Event)
                .WithMany(t => t.Students)
                .HasForeignKey(pt => pt.EventId);
        }
    }
}