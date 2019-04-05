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
    public class DbUsers : IDbUsers
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;
        private readonly IDbOrg _dbOrg;

        public DbUsers(
            IDbOrg dbOrg,
            IConfiguration config,
            UserManager<User> userManager)
        {
            _config = config;
            _userManager = userManager;
            _dbOrg = dbOrg;
        }
        
        /// <summary>
        /// Get User Info with Owner Role
        /// User data, Organization data, Staff in organization data
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<EditOwnerViewModel> GetOwnerInfoAsync(User user)
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
              
                result.Orgs = _dbOrg.GetOrgsByOwner(user.Id);
                
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

        /// <summary>
        /// Update User with Client Role
        /// </summary>
        /// <param name="updUser">client role object with new data</param>
        /// <returns></returns>
        public async Task<(bool isSuccess, IEnumerable<string> errors)> ClientUpdateAsync(IEditClient updUser)
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

        /// <summary>
        /// Compare two Client and set change in second Client
        /// if has found changed data
        /// </summary>
        /// <param name="updUser">client with new data</param>
        /// <param name="user">client with old data which will be changed</param>
        /// <returns></returns>
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

        /// <summary>
        /// Get User List with short info
        /// </summary>
        /// <returns></returns>
        public async Task<List<UsersListViewModel>> GetUserListAsync()
        {

            return await Task.Run(async () =>
            {
                List<UsersListViewModel> result = new List<UsersListViewModel>();
                ConnectionDb conn = new ConnectionDb(_config);
                using (IDbConnection sqlCon = conn.GetConnection)
                {
                    string query = @"
                    select 
                        u.id as UserId,
                        u.UserName,
                        u.Email as UserMail,
                        r.name as UserRole
                    from User u
                    join UserRole ur on u.id = ur.UserId
                    join Role r on r.id = ur.RoleId
                    ";

                    ConnectionDb.OpenConnect(sqlCon);
                    var resultQuery = await sqlCon.QueryAsync<UsersListViewModel>(query);
                    result = resultQuery.ToList();
                }

                return result;
            });
        }
    }
}