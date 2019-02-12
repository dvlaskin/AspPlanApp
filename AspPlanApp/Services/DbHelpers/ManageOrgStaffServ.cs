using System.Linq;
using System.Threading.Tasks;
using AspPlanApp.Models;
using AspPlanApp.Models.DbModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AspPlanApp.Services.DbHelpers
{
    public class ManageOrgStaffServ
    {
        private AppDbContext _dbContext;
        private IConfiguration _config;
        private UserManager<User> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public ManageOrgStaffServ(AppDbContext dbContext,
            IConfiguration config,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManger)
        {
            _dbContext = dbContext;
            _config = config;
            _userManager = userManager;
            _roleManager = roleManger;
        }

        public async Task<bool> RemoveStaffFromOrg(string ownerId, int orgId, string staffId)
        {
            bool result = false;

            Models.DbModels.Org org = await _dbContext.Org
                .Where(w => w.orgId == orgId)
                .FirstOrDefaultAsync();

            if (org.owner != ownerId) 
                return result;
            
            OrgStaff orgStaff = await _dbContext.OrgStaff
                .Where(w => w.orgId == orgId && w.staffId == staffId)
                .FirstOrDefaultAsync();

            if (orgStaff == null) 
                return result;
            
            _dbContext.Entry(orgStaff).State = EntityState.Deleted;
            int remResult = await _dbContext.SaveChangesAsync();

            if (remResult == 1)
                result = true;
            
            return result;
        }
    }
}