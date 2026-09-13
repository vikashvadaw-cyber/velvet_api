using AdminService.Models;
using Microsoft.EntityFrameworkCore;
using System.Formats.Tar;
using System.Security.Cryptography.X509Certificates;
namespace AdminService.Dbcontext
{
    public class admindbcontext : DbContext
    {
        public admindbcontext(DbContextOptions<admindbcontext> options) : base(options) {  }

        public DbSet<Mst_Users> Mst_Users => Set<Mst_Users>();

        public DbSet<Mst_Menu> Mst_Menu => Set<Mst_Menu>();
    }
}
