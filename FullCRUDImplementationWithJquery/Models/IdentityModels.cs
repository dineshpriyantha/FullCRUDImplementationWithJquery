using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace FullCRUDImplementationWithJquery.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("ORP_Connection")
        {
        }

        //public DbSet<RegisterViewModel> register { get; set; }
        //public DbSet<IdentityRole> Roles { get; set; }
        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);
        //    modelBuilder.Entity<IdentityUser>().ToTable("Admins");
        //    modelBuilder.Entity<ApplicationUser>().ToTable("Admins");
        //    modelBuilder.Entity<IdentityUserRole>().ToTable("AdminRoles");
        //    modelBuilder.Entity<IdentityUserLogin>().ToTable("Logins");
        //    modelBuilder.Entity<IdentityUserClaim>().ToTable("Claims");
        //    modelBuilder.Entity<IdentityRole>().ToTable("Roles");
        //}
    }

}