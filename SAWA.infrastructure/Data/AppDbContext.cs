using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SAWA.core.Models;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SAWA.infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        private readonly IServiceProvider _serviceProvider;

        public AppDbContext(DbContextOptions<AppDbContext> options, IServiceProvider serviceProvider) : base(options)
        {
            _serviceProvider = serviceProvider;
        }

        public DbSet<Report> Reports { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<HelpRequest> HelpRequests { get; set; }
        public DbSet<Photo> Photos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public static async Task SeedRolesAndUsers(IServiceProvider serviceProvider, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            

            // Seed users
            var admin = await userManager.FindByEmailAsync("admin@example.com");
            if (admin == null)
            {
                var newAdmin = new AppUser
                {
                    UserName = "admin@example.com",
                    Email = "admin@example.com",
                    FullName = "Admin User"
                };
                await userManager.CreateAsync(newAdmin, "Admin@1234");
                await userManager.AddToRoleAsync(newAdmin, "admin");
            }

            var regularUser = await userManager.FindByEmailAsync("user@example.com");
            if (regularUser == null)
            {
                var newUser = new AppUser
                {
                    UserName = "user@example.com",
                    Email = "user@example.com",
                    FullName = "Regular User"
                };
                await userManager.CreateAsync(newUser, "User@1234");
                await userManager.AddToRoleAsync(newUser, "user");
            }

            var charityUser = await userManager.FindByEmailAsync("charity@example.com");
            if (charityUser == null)
            {
                var newCharity = new AppUser
                {
                    UserName = "charity@example.com",
                    Email = "charity@example.com",
                    FullName = "Charity User"
                };
                await userManager.CreateAsync(newCharity, "Charity@1234");
                await userManager.AddToRoleAsync(newCharity, "charity");
            }
        }
    }
}
