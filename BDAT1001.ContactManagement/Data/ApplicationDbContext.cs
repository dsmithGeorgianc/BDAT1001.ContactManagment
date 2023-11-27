using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BDAT1001.ContactManagement.Models;

namespace BDAT1001.ContactManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<BDAT1001.ContactManagement.Models.Contact> Contact { get; set; } = default!;
    }
}
