using Microsoft.AspNet.Identity.EntityFramework;

namespace LexiconLMS.Migrations
{
    using LexiconLMS.Models;
    using Microsoft.AspNet.Identity;
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Models.ApplicationDbContext>
    {
        public Configuration() {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ApplicationDbContext context) {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);

            //create roles and add it to the role table
            var roleNames = new[] { "teacher", "student" };
            foreach (var roleName in roleNames)
            {
                if (context.Roles.Any(r => r.Name == roleName)) continue;
                var role = new IdentityRole { Name = roleName };
                var result = roleManager.Create(role);
                if (!result.Succeeded)
                {
                    throw new Exception(string.Join("\n", result.Errors));
                }
            }

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);


            //add our users to the users table
            var emails = new[] { "student@lexicon.se", "teacher@lexicon.se" };
            foreach (var email in emails)
            {
                if (context.Users.Any(u => u.UserName == email)) continue;
                var user = new ApplicationUser { UserName = email, Email = email };
                var result = userManager.Create(user, "foobar");
                if (!result.Succeeded)
                {
                    throw new Exception(string.Join("\n", result.Errors));
                }
            }


            //adding roles to our users
            var teacherUser = userManager.FindByName("teacher@lexicon.se");
            userManager.AddToRole(teacherUser.Id, "teacher");

            var studentUser = userManager.FindByName("student@lexicon.se");
            userManager.AddToRoles(studentUser.Id, "student");
        }
    
    }
}
