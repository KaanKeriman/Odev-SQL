using Game_Store.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace Game_Store.Data
{
    public class AppDataBaseContext:DbContext
    {
        public AppDataBaseContext(DbContextOptions<AppDataBaseContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
    }
}
