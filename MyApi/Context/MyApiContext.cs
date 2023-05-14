using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using MyApi.Models;

namespace MyApi.Context
{
    public class MyApiContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Member> Members { get; set; }

        public MyApiContext(DbContextOptions<MyApiContext> options) : base(options)
        {
        }
    }
}
