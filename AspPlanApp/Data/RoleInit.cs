using AspPlanApp.Models.DbModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspPlanApp.Data
{
    public class RoleInit
    {

        public static void RoleInitAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            string adminName = "admin";
            string adminMail = "admin@mail.com";
            string adminPassword = "admin";

            if (roleManager.FindByNameAsync("admin").Result == null)
            {
                roleManager.CreateAsync(new IdentityRole("admin"));
            }

            if (roleManager.FindByNameAsync("owner").Result == null)
            {
                roleManager.CreateAsync(new IdentityRole("owner"));
            }

            if (roleManager.FindByNameAsync("staff").Result == null)
            {
                roleManager.CreateAsync(new IdentityRole("staff"));
            }

            if (roleManager.FindByNameAsync("user").Result == null)
            {
                roleManager.CreateAsync(new IdentityRole("user"));
            }

            if (userManager.FindByEmailAsync(adminMail).Result == null)
            {
                User adminUser = new User() { UserName = adminName, Email = adminMail };

                var resultTask = userManager.CreateAsync(adminUser, adminPassword);
                IdentityResult result = resultTask.Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(adminUser, "admin");
                }
            }

        }

    }
}
