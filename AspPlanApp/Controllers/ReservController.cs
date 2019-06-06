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
        private readonly IDbOrg _dbOrg;
        private readonly IDbOrgStaff _dbOrgStaff;
        private readonly IDbOrgReserv _dbOrgReserv;
        
        
        public ReservController(
            UserManager<User> userManager, 
            IDbOrg dbOrg,
            IDbOrgStaff dbOrgStaff,
            IDbOrgReserv dbOrgReserv
        )
        {
            _userManager = userManager;
            _dbOrg = dbOrg;
            _dbOrgStaff = dbOrgStaff;
            _dbOrgReserv = dbOrgReserv;
        }
        

        [HttpGet]
        public async Task<IActionResult> OrgCalendar(int OrgId, DateTime dateCal)
        {
            if (OrgId == 0 || dateCal == DateTime.MinValue) 
                return RedirectToAction("Index", "Home"); 
            
            Models.DbModels.Org orgInfo = await _dbOrg.GetOrgByIdAsync(OrgId);
            string orgName = string.Empty;
            if (orgInfo != null)
            {
                orgName = orgInfo.orgName;
            }
            
            List<StaffInfo> staffInfo = new List<StaffInfo>();

            var orgStaff = await _dbOrgStaff.GetOrgStaffByOrgId(OrgId);
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

        [HttpGet]
        public async Task<IActionResult> UserCalendar()
        {
            var user = User;
            string userId = _userManager.GetUserId(user);
            
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddNewEvent(int orgId,DateTime dateFrom,DateTime dateTo,int staff,string comm)
        {
            var user = User;
            string userId = _userManager.GetUserId(user);

            if (
                string.IsNullOrEmpty(userId) 
                || orgId == 0 
                || dateFrom == DateTime.MinValue 
                || dateTo == DateTime.MinValue
                )
                return RedirectToAction("Login", "Account");

            if (dateTo > dateFrom)
            {
                await _dbOrgReserv.AddNewEvent(userId, orgId, dateFrom, dateTo, staff, comm);
            }
                
            return RedirectToAction("OrgCalendar", "Reserv", new { orgId, dateCal = dateFrom });
        }

        public async Task<IActionResult> ConfirmReserveEvent(int resId, string currDateString)
        {
            DateTime currDate = DateTime.Now;
            DateTime.TryParse(currDateString, out currDate);
            
            // todo: реализовать подтверждение зарезервированного события

            return RedirectToAction("UserCalendar", "Reserv");
        }
    }
}