using core_cosmo_cs.Models;
using Microsoft.EntityFrameworkCore;

namespace core_cosmo_cs.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }

        public DbSet<ResultViewModel> Results { get; set; }
    }
}