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
        private readonly UserManager<User> _userManager;
        private IDbOrg _dbOrg;
        private DbOrgStaffServ _dbOrgStaffServ;
        
        
        public ReservController(
            UserManager<User> userManager, 
            IDbOrg dbOrg
        )
        {
            _userManager = userManager;
            _dbOrg = dbOrg;
        }
        

        [HttpGet]
        public async Task<IActionResult> OrgCalendar(int OrgId, DateTime dateCal)
        {
            if (OrgId == 0 || dateCal == DateTime.MinValue) return RedirectToAction("Index", "Home"); 
            
            Models.DbModels.Org orgInfo = await _dbOrg.GetOrgByIdAsync(OrgId);
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