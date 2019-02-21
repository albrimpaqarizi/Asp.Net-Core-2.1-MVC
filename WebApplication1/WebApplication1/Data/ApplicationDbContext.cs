using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using Microsoft.EntityFrameworkCore.SqlServer;

namespace WebApplication1.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CarUser>()
            .HasKey(t => new { t.CarsId, t.UserId });

            modelBuilder.Entity<CarUser>()
                .HasOne(pt => pt.Cars)
                .WithMany(a => a.CarUsers)
                .HasForeignKey(pt => pt.CarsId);

            modelBuilder.Entity<CarUser>()
                .HasOne(pt => pt.User)
                .WithMany()
                .HasForeignKey(pt => pt.UserId);

            #region CarsSeed

            modelBuilder.Entity<Cars>().HasData(new Cars
            {
                Id = 1,
                Name = "BMW",
                Description = "Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of " +
                "type and scrambled it to make a type specimen book.",
                Image = "ok.jpg",
                Price = 2.99f
            });

            #endregion
        }

        public DbSet<Cars> Cars { get; set; }
        public DbSet<CarUser> CarUsers { get; set; }
        
    }
}
