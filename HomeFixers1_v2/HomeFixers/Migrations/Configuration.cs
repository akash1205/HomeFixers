namespace HomeFixers.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<HomeFixers.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(HomeFixers.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            var roles = new[]
           {
                 "Admin",
                 "Customer",
                 "Provider"

             };

            var users = new[]
              {
                 new {Email = "admin@example.com", Pwd = "Password123", Roles = "Admin"},

             };
            roles.ToList().ForEach(r => context.Roles.AddOrUpdate(x => x.Name,
                new Microsoft.AspNet.Identity.EntityFramework.IdentityRole { Id = Guid.NewGuid().ToString(), Name = r }));
            foreach (var user in users)
            {
                ApplicationUserManager mgr = new ApplicationUserManager(
                    new Microsoft.AspNet.Identity.EntityFramework.UserStore<Models.ApplicationUser>(context));
                Models.ApplicationUser existingUser = context.Users.FirstOrDefault(x => x.UserName == user.Email);
                if (existingUser != null) Microsoft.AspNet.Identity.UserManagerExtensions.Delete(mgr, existingUser);
                Models.ApplicationUser au = new Models.ApplicationUser { Email = user.Email, UserName = user.Email };
                var result = mgr.CreateAsync(au, user.Pwd).Result;
                if (!string.IsNullOrEmpty(user.Roles))
                    Microsoft.AspNet.Identity.UserManagerExtensions.AddToRoles(mgr, au.Id, user.Roles.Split(','));

            }
        }
    }
}
