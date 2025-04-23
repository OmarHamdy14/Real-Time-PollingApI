using Microsoft.EntityFrameworkCore;

namespace RealTimePollingApI.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions op) :  base(op)
        {

        }
        public DbSet<Vote> Votes { get; set; }
    }
}
