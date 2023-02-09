using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RuriAppSec.Model
{



    public class AuthDbContext : IdentityDbContext
    {
        private readonly IConfiguration _configuration;
        //public AuthDbContext(DbContextOptions<AuthDbContext> options):base(options){ }
        public AuthDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = _configuration.GetConnectionString("AuthConnectionString"); optionsBuilder.UseSqlServer(connectionString);
        }


        public DbSet<ApplicationUser> ApplicationUser { get; set; }


        public DbSet<AuditLogTrails> AuditLogTrails { get; set; }

        public DbSet<Password> PasswordDB { get; set; }

    }




}
