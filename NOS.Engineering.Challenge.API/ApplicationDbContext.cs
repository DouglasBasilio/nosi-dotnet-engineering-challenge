using Microsoft.EntityFrameworkCore;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
        {
        }

        public ApplicationDbContext() : base()
        {
        }

        public DbSet<Content> Contents { get; set; }
    }
}
