using Microsoft.EntityFrameworkCore;
using WebApi_TestWork1.Models;

namespace WebApi_TestWork1.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)  { }
        public DbSet<User> Users { get; set; }
    }
}
