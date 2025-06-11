using GetBlog.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Data.Entity.Migrations;
using System.Linq;

internal sealed class Configuration : DbMigrationsConfiguration<GetBlog.Models.GetBlogDbContext>
{
    public Configuration()
    {
        AutomaticMigrationsEnabled = false;
    }

    protected override void Seed(GetBlog.Models.GetBlogDbContext context)
    {
        var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
        var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

        string[] rolesToCreate = { "Admin", "Editor", "Author", "User" }; 

        foreach (var roleName in rolesToCreate)
        {
            if (!roleManager.RoleExists(roleName))
            {
                roleManager.Create(new IdentityRole(roleName));
            }
        }

        var adminUser = userManager.FindByName("amir");

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = "amir",
                Email = "amirsoveizi2004@gmail.com",
                FullName = "Amir Soveizi",
                EmailConfirmed = true
            };

            var result = userManager.Create(adminUser, "asdw1234");

            if (result.Succeeded)
            {
                userManager.AddToRole(adminUser.Id, "Admin");
                userManager.AddToRole(adminUser.Id, "User");
            }
        }
    }
}
