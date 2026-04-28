using Microsoft.EntityFrameworkCore;
using rest.Models;

namespace rest.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Director> Directors { get; set; }
        public DbSet<ProductionCompany> ProductionCompanies { get; set; }
    }
}
