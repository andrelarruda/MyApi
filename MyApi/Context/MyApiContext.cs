using Microsoft.EntityFrameworkCore;
using MyApi.Models;

namespace MyApi.Context
{
    public class MyApiContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public DbSet<User> Users { get; set; } = null!;
        public MyApiContext(DbContextOptions<MyApiContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Registers the MyApiContext
            //"name=ConnectionStrings:DefaultConnection"
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
        }
    }
}
