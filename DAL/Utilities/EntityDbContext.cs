using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Models.Entities;
using static DAL.Utilities.ConnectionStringUtility;

namespace DAL.Utilities
{
    public sealed class EntityDbContext: IdentityDbContext<User, IdentityRole<int>, int>, IDesignTimeDbContextFactory<EntityDbContext>
    {
        public DbSet<Student> Students { get; set; }
                
        public DbSet<Driver> Drivers { get; set; }
        
        public DbSet<Host> Hosts { get; set; }
        
        public DbSet<Event> Events { get; set; }

        public EntityDbContext() { }
        
        /// <inheritdoc />
        /// <summary>
        /// Constructor that will be called by startup.cs
        /// </summary>
        /// <param name="optionsBuilderOptions"></param>
        // ReSharper disable once SuggestBaseTypeForParameter
        public EntityDbContext(DbContextOptions<EntityDbContext> optionsBuilderOptions) : base(optionsBuilderOptions)
        {
            Database.EnsureCreated();
        }
        
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

            base.OnModelCreating(modelBuilder);
        }

        public EntityDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            var options = new DbContextOptionsBuilder<EntityDbContext>()
                .UseNpgsql(ConnectionStringUrlToResource(configuration.GetValue<string>("DATABASE_URL")))
                .Options;

            return new EntityDbContext(options);
        }
    }
}