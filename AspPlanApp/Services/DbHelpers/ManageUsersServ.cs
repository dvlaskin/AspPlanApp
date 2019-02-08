using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AspPlanApp.Data;
using AspPlanApp.Models;
using AspPlanApp.Models.DbModels;
using AspPlanApp.Models.ManageUsersViewModels;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;


namespace AspPlanApp.Services.DbHelpers
{
    public class ManageUsersServ
    {
        private AppDbContext _dbContext;
        private IConfiguration _config;
        private UserManager<User> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public ManageUsersServ(AppDbContext dbContext, 
            IConfiguration config,
            UserManager<User> userManager, 
            RoleManager<IdentityRole> roleManger)
        {
            _dbContext = dbContext;
            _config = config;
            _userManager = userManager;
            _roleManager = roleManger;
        }
        
        
        public async Task<EditOwnerViewModel> GetOwnerInfoAcync(User user)
        {
            if (user == null) return null;

            return await Task.Run(async () =>
            {
                EditOwnerViewModel result = new EditOwnerViewModel()
                {
                    Id = user.Id,
                    Name = user.UserName,
                    Email = user.Email,
                    NewPassword = string.Empty,
                    OldPassword = string.Empty,
                    ConfirmNewPassword = string.Empty
                };
                
                result.Orgs = _dbContext.Org
                    .Where(w => w.owner == user.Id).ToList();
                
                ConnectionDb conn = new ConnectionDb(_config);

                using (IDbConnection sqlCon = conn.GetConnection)
                {
                    string query = @"
                        select 
                            u.UserName,
                            u.Email as emailUser,
                            s.orgStaffId,
                            s.isConfirm,
                            s.orgId,
                            s.staffId
                        from org o
                        join orgStaff s on s.orgId = o.orgId
                        join user u on u.id = s.staffId
                        where o.owner = @userId
                        ";
                    
                    // Ensure that the connection state is Open
                    ConnectionDb.OpenConnect(sqlCon);
                    
                    var resultQuery = await sqlCon.QueryAsync<OwnerOrgStaffViewModel>(query, new { userId = user.Id });

                    result.Staff = resultQuery.ToList();
                }

                return result;
            });
        }

        public async Task<(bool isSuccess, IEnumerable<string> errors)> ClientUpdate(IEditClient updUser)
        {

            var result = (isSuccess: false, errors: new List<string>());

            if (updUser == null)
            {
                result.errors.Add("User is empty!");

                return result;
            }
                           
            User user = await _userManager.FindByIdAsync(updUser?.Id);

            if (user == null)
            {
                result.errors.Add("User is not exists!");

                return result;
            }

            bool isChanged = CompareUsers(updUser, ref user);
            if (isChanged)
            {
                IdentityResult updResult = await _userManager.UpdateAsync(user);
                if (!updResult.Succeeded)
                {
                    foreach (var error in updResult.Errors)
                    {
                        result.errors.Add(error.Description);
                    }

                    return result;
                }
            }
            
            if (
                !string.IsNullOrEmpty(updUser.OldPassword) &&
                !string.IsNullOrEmpty(updUser.NewPassword) &&
                !string.IsNullOrEmpty(updUser.ConfirmNewPassword)
            )
            {
                bool checkPassResult = await _userManager.CheckPasswordAsync(user, updUser.OldPassword);

                if (checkPassResult)
                {
                    IdentityResult changePassResult =
                        await _userManager.ChangePasswordAsync(user, updUser.OldPassword, updUser.NewPassword);

                    if (!changePassResult.Succeeded)
                    {
                        foreach (var error in changePassResult.Errors)
                        {
                            result.errors.Add(error.Description);
                        }
                        return result;
                    }
                }
                else
                {
                    result.errors.Add("Password was not changed!");
                    return result;
                }
            }

            result.isSuccess = true;

            return result;
        }

        private bool CompareUsers(IEditClient updUser, ref User user)
        {
            bool result = false;

            if (user.UserName != updUser.Name)
            {
                user.UserName = updUser.Name;
                result = true;
            }
            
            if (user.Email != updUser.Email)
            {
                user.Email = updUser.Email;
                result = true;
            }

            return result;
        }

    }
}