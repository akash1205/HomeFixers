using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace HomeFixers.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<HomeFixers.Models.ServiceProvider> ServiceProviders { get; set; }

        public System.Data.Entity.DbSet<HomeFixers.Models.Service> Services { get; set; }

        public System.Data.Entity.DbSet<HomeFixers.Models.BucketList> BucketLists { get; set; }

        public System.Data.Entity.DbSet<HomeFixers.Models.Customer> Customers { get; set; }

        public System.Data.Entity.DbSet<HomeFixers.Models.Schedule> Schedules { get; set; }

        public System.Data.Entity.DbSet<HomeFixers.Models.CustomerBankAccount> CustomerBankAccounts { get; set; }

        public System.Data.Entity.DbSet<HomeFixers.Models.ProviderBankAccount> ProviderBankAccounts { get; set; }

        public System.Data.Entity.DbSet<HomeFixers.Models.Transaction> Transactions { get; set; }
    }
}