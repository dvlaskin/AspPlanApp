using System.Linq;
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
        /// Add new staff in OrgStaff table
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        private static async Task<bool> AddNewStaff(string userId, int orgId)
        {
            OrgStaff orgStaff = new OrgStaff()
            {
                orgId = orgId,
                staffId = userId,
                isConfirm = false
            };

            _dbContext.OrgStaff.Add(orgStaff);
            var result =  await _dbContext.SaveChangesAsync();

            return result == 1;
        }

        /// <summary>
        /// Change Organization where work Staff User
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public static async Task<bool> ChangeOrgForStaff(string userId, int orgId)
        {
            bool result = false;

            if (string.IsNullOrEmpty(userId) || orgId == 0) return result;
            
            OrgStaff currOrgStaff = _dbContext.OrgStaff.FirstOrDefault(w => w.staffId == userId);

            if (currOrgStaff == null)
            {
                result = await AddNewStaff(userId, orgId);
            }
            else
            {
                currOrgStaff.orgId = orgId;
                currOrgStaff.isConfirm = false;
                _dbContext.Entry(currOrgStaff).State = EntityState.Modified;
                var updResult = await _dbContext.SaveChangesAsync();
                result = updResult == 1;
            }

            return result;
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
            
            _dbContext.OrgStaff.Remove(orgStaff);
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
            
            _dbContext.OrgStaff.Update(orgStaff);
            int remResult = await _dbContext.SaveChangesAsync();

            if (remResult == 1)
                result = true;
            
            return result;
        }

        /// <summary>
        /// Get Organization Staff
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

        /// <summary>
        /// Get Organization by Staff
        /// </summary>
        /// <param name="staffId"></param>
        /// <returns></returns>
        public static async Task<Models.DbModels.Org> GetOrgByStaffIdAsync(string staffId)
        {
            int orgId = await _dbContext.OrgStaff
                .Where(w => w.staffId == staffId)
                .Select(s => s.orgId)
                .FirstOrDefaultAsync();

            if (orgId == 0) return null;
                
            return await DbOrgServ.GetOrgByIdAsync(orgId);
        }

        /// <summary>
        /// Get Staff by Organization
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public static async Task<OrgStaff[]> GetOrgStaffByOrgId(int orgId)
        {
            return await _dbContext.OrgStaff.Where(w => w.orgId == orgId).ToArrayAsync();
        }
    }
}