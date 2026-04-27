using DevJBackend.Entity;
using DevJBackend.Model;
using Microsoft.EntityFrameworkCore;

namespace DevJBackend.Data
{
    public class DevDbContext : DbContext
    {
        public DevDbContext(DbContextOptions<DevDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<CrsModel> CrsList { get; set; }
        public DbSet<CrsTopicModel> CrsInfo { get; set; }
    }
}
