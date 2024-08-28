using Microsoft.EntityFrameworkCore;
using UnstopAPI.Models;

namespace UnstopAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<Job> Jobs { get; set; }

        public DbSet<Candidate> Candidates { get; set; }

        public DbSet<Company> Companies { get; set; }

        public DbSet<Application> Applications { get; set; }

        public DbSet<Interview> Interviews { get; set; }

        public DbSet<FavoriteJob> FavoriteJobs { get; set;}

        public DbSet<UserPreference> UserPreferences { get; set; }

        public DbSet<OTP> OTPs { get; set; }

        public DbSet<JobFair> JobFairs { get; set; }

        public DbSet<Template> Templates { get; set; }

        public DbSet<Element> Elements { get; set; }
    }
}
