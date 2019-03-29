using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspPlanApp.Models;
using AspPlanApp.Models.DbModels;
using AspPlanApp.Models.ManageUsersViewModels;
using AspPlanApp.Models.ReservViewModels;
using AspPlanApp.Services.DbHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AspPlanApp.Controllers
{
    public class ReservController : Controller
    {
        private UserManager<User> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private AppDbContext _dbContext;
        private IConfiguration _config;
        private DbOrgServ _dbOrgServ;
        
        
        public ReservController(
            UserManager<User> userManager, 
            RoleManager<IdentityRole> roleManger,
            AppDbContext dbContext,
            IConfiguration config
        )
        {
            _userManager = userManager;
            _roleManager = roleManger;
            _dbContext = dbContext;
            _config = config;

            DbServInitialization();
        }
        
        private void DbServInitialization()
        {
            if (_dbOrgServ == null)
                _dbOrgServ = new DbOrgServ(_dbContext, _config, _userManager, _roleManager);
        }

        [HttpGet]
        public async Task<IActionResult> OrgCalendar(int OrgId = 0, DateTime dateCal = default(DateTime))
        {
            Models.DbModels.Org orgInfo = await DbOrgServ.GetOrgByIdAsync(OrgId);
            string orgName = string.Empty;
            if (orgInfo != null)
            {
                orgName = orgInfo.orgName;
            }
            
            return View(new OrgDateViewModel() { orgId = OrgId, dateCal = dateCal, orgName = orgName });
        }

    }
}