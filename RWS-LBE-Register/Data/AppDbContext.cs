using Microsoft.EntityFrameworkCore;
using RWS_LBE_Register.Models;

namespace RWS_LBE_Register.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }
         
        public DbSet<SysChannel> SysChannel { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        public DbSet<RLPUserNumbering> RLPUserNumbering { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 
        }
    }
}
