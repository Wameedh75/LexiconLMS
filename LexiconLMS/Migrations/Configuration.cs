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
            foreach (var roleName in roleNames) {
                if (context.Roles.Any(r => r.Name == roleName))
                    continue;
                var role = new IdentityRole { Name = roleName };
                var result = roleManager.Create(role);
                if (!result.Succeeded) {
                    throw new Exception(string.Join("\n", result.Errors));
                }
            }

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            //add our users to the users table
            var emails = new[] { "student@lexicon.se", "teacher@lexicon.se" };
            foreach (var email in emails) {
                if (context.Users.Any(u => u.UserName == email))
                    continue;
                var user = new ApplicationUser { UserName = email, Email = email };
                var result = userManager.Create(user, "foobar");
                if (!result.Succeeded) {
                    throw new Exception(string.Join("\n", result.Errors));
                }
            }

            //adding roles to our users
            var teacherUser = userManager.FindByName("teacher@lexicon.se");
            userManager.AddToRole(teacherUser.Id, "teacher");

            var studentUser = userManager.FindByName("student@lexicon.se");
            userManager.AddToRoles(studentUser.Id, "student");

            // Seed Courses
            var courses = new[]
            {
                new Course { Name = ".NET", StartDate = new DateTime(2018, 1, 1), EndDate = new DateTime(2018, 8, 30), Description = "MVC, C#, EF, Javascript" },
                new Course { Name = "Java", StartDate = new DateTime(2018, 08, 01), EndDate = new DateTime(2019, 05, 30), Description = "JDK, JSP, Spring" },
                new Course { Name = "Monkey care", StartDate = new DateTime(2017, 01, 01), EndDate = new DateTime(2017, 12, 31), Description = "Fur grooming, washing, de-lousing, toothbrushing" }

            };
            context.Courses.AddOrUpdate(
                c => c.Name,
                courses
            );

            // Seed Modules
            var modules = new[]
            {
                new Module { Name = "C#", Course = courses[0], StartDate = new DateTime(2018, 1, 1), EndDate = new DateTime(2018, 2, 1), Description = "Basics of the C# language" },
                new Module { Name = "MVC", Course = courses[0], StartDate = new DateTime(2018, 2, 2), EndDate = new DateTime(2018, 3, 1), Description = "Learn about MVC.NET" },
                new Module { Name = "EF", Course = courses[0], StartDate = new DateTime(2018, 3, 2), EndDate = new DateTime(2018, 4, 1), Description = "Entity Framework" },
                new Module { Name = "Javascript", Course = courses[0], StartDate = new DateTime(2018, 4, 2), EndDate = new DateTime(2018, 5, 1), Description = "JS basics, Jquery, AJAX" },
                new Module { Name = "JDK", Course = courses[1], StartDate = new DateTime(2018, 8, 1), EndDate = new DateTime(2018, 10, 1), Description = "Everything about the JDK environment" },
                new Module { Name = "JSP", Course = courses[1], StartDate = new DateTime(2018, 10, 2), EndDate = new DateTime(2018, 12, 1), Description = "Java Server Pages" },
                new Module { Name = "Spring", Course = courses[1], StartDate = new DateTime(2018, 12, 2), EndDate = new DateTime(2019, 2, 1), Description = "Well, what is Spring anyway?" },
                new Module { Name = "Fur grooming", Course = courses[2], StartDate = new DateTime(2017, 1, 1), EndDate = new DateTime(2017, 4, 1), Description = "All about caring for the monkey's fur." },
                new Module { Name = "Washing", Course = courses[2], StartDate = new DateTime(2017, 4, 2), EndDate = new DateTime(2017, 7, 1), Description = "How to best wash a monkey without being bitten" },
                new Module { Name = "De-lousing", Course = courses[2], StartDate = new DateTime(2017, 7, 2), EndDate = new DateTime(2017, 10, 1), Description = "Different types of monkey lice and how to remove them" },
                new Module { Name = "Toothbrushing", Course = courses[2], StartDate = new DateTime(2017, 10, 2), EndDate = new DateTime(2017, 12, 31), Description = "How to brush a monkey's teeth or build a monkey-toothbrushing machine" }
            };
            context.Modules.AddOrUpdate(
                m => new { m.Name, m.CourseId },
                modules
            );

            // Seed Activities
            var activities = new[]
            {
                new Activity { Module = modules[0], Name = "Activity 1", Description = "Hej hopp", Starttime = new DateTime(2018, 1, 1, 8, 30, 0), EndTime = new DateTime(2018, 1, 1, 17, 0, 0), Type = ActivityType.ELearningspass },
                new Activity { Module = modules[1], Name = "Activity 2", Description = "Hej hopp", Starttime = new DateTime(2018, 2, 2, 8, 30, 0), EndTime = new DateTime(2018, 2, 2, 17, 0, 0), Type = ActivityType.Föreläsning },
                new Activity { Module = modules[2], Name = "Activity 3", Description = "Hej hopp", Starttime = new DateTime(2018, 3, 2, 8, 30, 0), EndTime = new DateTime(2018, 3, 2, 17, 0, 0), Type = ActivityType.Övningstillfälle },
            };

            // Bootstrap some DocumentTypes
            var teacherRole = roleManager.Roles.Where(r => r.Name == "teacher").ToList();
            var studentRole = roleManager.Roles.Where(r => r.Name == "student").ToList();
            var allRoles = roleManager.Roles.ToList();
            context.DocumentTypes.AddOrUpdate(
                t => t.Name,
                new DocumentType { Name = "Student assignment", CanCreate = studentRole, CanView = teacherRole },
                new DocumentType { Name = "Information", CanCreate = teacherRole, CanView = allRoles },
                new DocumentType { Name = "Module document", CanCreate = teacherRole, CanView = allRoles },
                new DocumentType { Name = "Lecture document", CanCreate = teacherRole, CanView = allRoles },
                new DocumentType { Name = "Exercise", CanCreate = teacherRole, CanView = allRoles }
            );
        }
    }
}
