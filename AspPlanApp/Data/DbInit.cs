using AspPlanApp.Models.DbModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspPlanApp.Models;

namespace AspPlanApp.Data
{
    public class DbDefaultValuesInit
    {

        public static void DbInitAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            // создание стандартынх ролей пользователей приложения
            RoleAdd(roleManager, AppRoles.Admin);
            RoleAdd(roleManager, AppRoles.Owner);
            RoleAdd(roleManager, AppRoles.Staff);
            RoleAdd(roleManager, AppRoles.Client);

            // создание стандартных пользователей для каждой роли
            UserAdd(userManager, UserGenerate(AppRoles.Admin));
            UserAdd(userManager, UserGenerate(AppRoles.Owner));
            UserAdd(userManager, UserGenerate(AppRoles.Staff));
            UserAdd(userManager, UserGenerate(AppRoles.Client));

        }

        private static void RoleAdd(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (roleManager.FindByNameAsync(roleName).Result == null)
            {
                roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
        
        private static void UserAdd(UserManager<User> userManager, AddDefUser userInfo)
        {
     
            if (userManager.FindByEmailAsync(userInfo.userName).Result == null)
            {
                User addUser = new User() { UserName = userInfo.userName, Email = userInfo.userMail };

                var resultTask = userManager.CreateAsync(addUser, userInfo.userPassword);
                IdentityResult result = resultTask.Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(addUser, userInfo.userRole);
                }
            }
        }

        private static AddDefUser UserGenerate(string userPrefix)
        {
            return new AddDefUser()
            {
                userName = userPrefix,
                userMail = $"{userPrefix}@mail.com",
                userPassword = userPrefix,
                userRole = userPrefix
            };
        }
    }

    class AddDefUser
    {
        public string userName { get; set; }
        public string userMail { get; set; }
        public string userPassword { get; set; }
        public string userRole { get; set; }
    }
}
