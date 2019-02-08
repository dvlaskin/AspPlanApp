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
    public class ManageOrgServ
    {
        private AppDbContext _dbContext;
        private IConfiguration _config;
        private UserManager<User> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public ManageOrgServ(AppDbContext dbContext,
            IConfiguration config,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManger)
        {
            _dbContext = dbContext;
            _config = config;
            _userManager = userManager;
            _roleManager = roleManger;
        }


        public async Task<(bool isSuccess, IEnumerable<string> errors)> OrgUpdate(IEditOrg updOrg)
        {
            var result = (isSuccess: false, errors: new List<string>());
            
            if (updOrg == null)
            {
                result.errors.Add("Org is empty!");

                return result;
            }


            var org = await _dbContext.Org.Where(w => w.orgId == updOrg.orgId).FirstOrDefaultAsync();
        
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

        private bool CompareOrgs(IEditOrg updOrg, ref Models.DbModels.Org sourceOrg)
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

            return result;
        }
    }
}