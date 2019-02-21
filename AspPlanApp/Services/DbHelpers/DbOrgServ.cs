using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AspPlanApp.Data;
using AspPlanApp.Models;
using AspPlanApp.Models.DbModels;
using AspPlanApp.Models.ManageUsersViewModels;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace AspPlanApp.Services.DbHelpers
{
    public class DbOrgServ
    {
        private static AppDbContext _dbContext;
        private static IConfiguration _config;
        private static UserManager<User> _userManager;
        private static RoleManager<IdentityRole> _roleManager;

        public DbOrgServ(AppDbContext dbContext,
            IConfiguration config,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManger)
        {
            _dbContext = dbContext;
            _config = config;
            _userManager = userManager;
            _roleManager = roleManger;
        }

        /// <summary>
        /// Update Organization
        /// </summary>
        /// <param name="updOrg">object with new organization info</param>
        /// <returns></returns>
        public static async Task<(bool isSuccess, IEnumerable<string> errors)> OrgUpdateAsync(IEditOrg updOrg)
        {
            var result = (isSuccess: false, errors: new List<string>());
            
            if (updOrg == null)
            {
                result.errors.Add("Org is empty!");
                return result;
            }

            var org = await GetOrgByIdAsync(updOrg.orgId);
            if (org == null)
            {
                result.errors.Add("Org is not exists!");
                return result;
            }

            bool isChanged = CompareOrgs(updOrg, ref org);
            if (isChanged)
            {
                _dbContext.Entry(org).State = EntityState.Modified;

                var updResult = await _dbContext.SaveChangesAsync();

                if (updResult == 0)
                {
                    result.errors.Add("Org was not updated!");
                }
                else
                {
                    result.isSuccess = true;
                }
            }
            else
            {
                result.isSuccess = true;
            }
            
            return result;
        }

        /// <summary>
        /// Compare two organization and set change in second organization
        /// if has found changed data
        /// </summary>
        /// <param name="updOrg">org with new data</param>
        /// <param name="sourceOrg">org with old data which will be changed</param>
        /// <returns></returns>
        private static bool CompareOrgs(IEditOrg updOrg, ref Models.DbModels.Org sourceOrg)
        {
            bool result = false;

            if (sourceOrg.orgName != updOrg.orgName)
            {
                sourceOrg.orgName = updOrg.orgName;
                result = true;
            }
            
            if (updOrg.owner != null && sourceOrg.owner != updOrg.owner)
            {
                sourceOrg.owner = updOrg.owner;
                result = true;
            }
            
            if (sourceOrg.address != updOrg.address)
            {
                sourceOrg.address = updOrg.address;
                result = true;
            }
            
            if (sourceOrg.city != updOrg.city)
            {
                sourceOrg.city = updOrg.city;
                result = true;
            }
            
            if (sourceOrg.country != updOrg.country)
            {
                sourceOrg.country = updOrg.country;
                result = true;
            }
            
            if (sourceOrg.phoneNumber != updOrg.phoneNumber)
            {
                sourceOrg.phoneNumber = updOrg.phoneNumber;
                result = true;
            }
            
            if (sourceOrg.category != updOrg.category)
            {
                sourceOrg.category = updOrg.category;
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Get Organization by ID
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public static async Task<Models.DbModels.Org> GetOrgByIdAsync(int orgId)
        {
            return await _dbContext.Org.Where(w => w.orgId == orgId).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get Organization List by Owner ID
        /// </summary>
        /// <param name="ownerId">owner userId</param>
        /// <returns></returns>
        public static List<Models.DbModels.Org> GetOrgsByOwner(string ownerId)
        {
            return _dbContext.Org.Where(w => w.owner == ownerId).ToList();
        }

        /// <summary>
        /// Search Organization By Name
        /// </summary>
        /// <param name="strOrgName"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Models.DbModels.Org>> SearchOrgName(string strOrgName)
        {
            return await _dbContext.Org
                .Where( w => w.orgName.ToLower().Contains(strOrgName.ToLower()) )
                .Take(10)
                .ToArrayAsync();
        }

        /// <summary>
        /// Add new org
        /// </summary>
        /// <param name="org"></param>
        /// <returns></returns>
        public static async Task<bool> AddNewOrg(Models.DbModels.Org org)
        {
            _dbContext.Entry(org).State = EntityState.Added;
            var result = await _dbContext.SaveChangesAsync();

            return result == 1;
        }

        /// <summary>
        /// Delete Company from user Organizations
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public static async Task<bool> DeleteOrg(string userId, int orgId)
        {
            bool result = false;

            if (string.IsNullOrEmpty(userId) || orgId == 0) return result;
            
            Models.DbModels.Org org = await _dbContext.Org
                .Where(w => w.owner == userId && w.orgId == orgId)
                .FirstOrDefaultAsync();

            if (org != null)
            {
                _dbContext.Entry(org).State = EntityState.Deleted;
                int isDel = _dbContext.SaveChanges();

                result = isDel == 1;
            }

            if (result)
            {
                var deletedOrgStaff = _dbContext.OrgStaff.Where(w => w.orgId == orgId).ToArray();
                foreach (var orgStaff in deletedOrgStaff)
                {
                    _dbContext.Entry(orgStaff).State = EntityState.Deleted;
                }
                
                _dbContext.SaveChanges();
            }

            return result;
        }
        
    }
}