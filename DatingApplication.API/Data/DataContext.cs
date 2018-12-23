using Microsoft.EntityFrameworkCore;
using DatingApplication.API.Models;
namespace DatingApplication.API.Data
{
    public class DataContext:DbContext
    {
        
        public DataContext(DbContextOptions<DataContext> options):base
        (options)
        {
            
        }
        public DbSet<Value> Values { get; set; }
        public DbSet<User> User {get; set;}

        
    }
}