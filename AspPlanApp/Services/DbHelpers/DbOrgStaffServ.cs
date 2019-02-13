﻿using System.Linq;
using System.Threading.Tasks;
using AspPlanApp.Models;
using AspPlanApp.Models.DbModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AspPlanApp.Services.DbHelpers
{
    public class DbOrgStaffServ
    {
        private static AppDbContext _dbContext;
        private static IConfiguration _config;
        private static UserManager<User> _userManager;
        private static RoleManager<IdentityRole> _roleManager;

        public DbOrgStaffServ(AppDbContext dbContext,
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
        /// Remove Staff from Organization
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="orgId"></param>
        /// <param name="staffId"></param>
        /// <returns></returns>
        public static async Task<bool> RemoveStaffFromOrgAsync(string ownerId, int orgId, string staffId)
        {
            bool result = false;

            Models.DbModels.Org org = await DbOrgServ.GetOrgByIdAsync(orgId);

            if (org.owner != ownerId) 
                return result;
            
            OrgStaff orgStaff = await GetOrgStaffAsync(orgId, staffId);

            if (orgStaff == null) 
                return result;
            
            _dbContext.Entry(orgStaff).State = EntityState.Deleted;
            int remResult = await _dbContext.SaveChangesAsync();

            if (remResult == 1)
                result = true;
            
            return result;
        }
        
        /// <summary>
        /// Confirm new staff in Organization
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="orgId"></param>
        /// <param name="staffId"></param>
        /// <returns></returns>
        public static async Task<bool> ConfirmNewStaffAsync(string ownerId, int orgId, string staffId)
        {
            bool result = false;

            Models.DbModels.Org org = await DbOrgServ.GetOrgByIdAsync(orgId);

            if (org.owner != ownerId) 
                return result;
            
            OrgStaff orgStaff = await GetOrgStaffAsync(orgId, staffId);

            if (orgStaff == null) 
                return result;

            orgStaff.isConfirm = true;
            
            _dbContext.Entry(orgStaff).State = EntityState.Modified;
            int remResult = await _dbContext.SaveChangesAsync();

            if (remResult == 1)
                result = true;
            
            return result;
        }

        /// <summary>
        /// Get Organization
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="staffId"></param>
        /// <returns></returns>
        public static async Task<OrgStaff> GetOrgStaffAsync(int orgId, string staffId)
        {
            return await _dbContext.OrgStaff
                .Where(w => w.orgId == orgId && w.staffId == staffId)
                .FirstOrDefaultAsync();
        }
    }
}