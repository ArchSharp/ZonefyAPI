using Domain.Entities.Token;
using Microsoft.EntityFrameworkCore;
using ZonefyDotnet.Entities;

namespace ZonefyDotnet.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<HouseProperty> HouseProperties { get; set; }
        public DbSet<ForgotPassword> ForgotPasswords { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<PropertyStatistics> PropertyStatistics { get; set; }
    }
}
