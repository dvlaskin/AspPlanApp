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
        private DbOrgStaffServ _dbOrgStaffServ;
        
        
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
            
            if (_dbOrgStaffServ == null)
                _dbOrgStaffServ = new DbOrgStaffServ(_dbContext, _config, _userManager, _roleManager);
        }

        [HttpGet]
        public async Task<IActionResult> OrgCalendar(int OrgId, DateTime dateCal)
        {
            if (OrgId == 0 || dateCal == DateTime.MinValue) return RedirectToAction("Index", "Home"); 
            
            Models.DbModels.Org orgInfo = await DbOrgServ.GetOrgByIdAsync(OrgId);
            string orgName = string.Empty;
            if (orgInfo != null)
            {
                orgName = orgInfo.orgName;
            }
            
            List<StaffInfo> staffInfo = new List<StaffInfo>();

            var orgStaff = await DbOrgStaffServ.GetOrgStaffByOrgId(OrgId);
            if (orgStaff.Length > 0)
            {
                foreach (var staff in orgStaff)
                {
                    var staffUser = await _userManager.FindByIdAsync(staff.staffId);
                    staffInfo.Add(new StaffInfo()
                    {
                        orgStaffId = staff.orgStaffId,
                        staffName = staffUser.UserName
                    });
                }
            }
            
            return View(new OrgDateViewModel()
            {
                orgId = OrgId, 
                dateCal = dateCal, 
                orgName = orgName,
                staffInfo = staffInfo.ToArray()
            });
        }

    }
}