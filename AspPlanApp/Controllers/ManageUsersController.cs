using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AspPlanApp.Models;
using AspPlanApp.Models.DbModels;
using AspPlanApp.Models.ManageUsersViewModels;
using AspPlanApp.Services.DbHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Configuration;

namespace AspPlanApp.Controllers
{
    [Authorize]
    public class ManageUsersController : Controller
    {
        private UserManager<User> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private AppDbContext _dbContext;
        private IConfiguration _config;
        private ManageUsersServ _userSrv;
        private ManageOrgServ _orgServ;
        private ManageOrgStaffServ _orgStaffServ;

        public ManageUsersController(
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
        }
        
        
        [HttpGet]
        public IActionResult Index()
        {
            var user = User;

            string userId = _userManager.GetUserId(user);

            /* todo: закончить роутинг корректировки пользователей
              в зависимости от роли 
              - для админа все
              - для владельца - персонал
              - для персонала и пользователей - сразу ред. свои данные
             */
            
            if (user.IsInRole(AppRoles.Admin))
            {
                return View(_userManager.Users.ToList());
            }
            else if (user.IsInRole(AppRoles.Owner))
            {
                return RedirectToAction(nameof(EditOwner), "ManageUsers", new {id = userId});
            }
            else
            {
                return RedirectToAction("EditClient", "ManageUsers", new { id = userId});
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> EditOwner(string id)
        {
            User owner = await _userManager.FindByIdAsync(id);

            if (owner == null)
            {
                return NotFound();
            }
           
            var userSrv = new ManageUsersServ(_dbContext, _config, _userManager, _roleManager);
            
            EditOwnerViewModel viewModel = await userSrv.GetOwnerInfoAcync(owner);                                                
           
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditOwner(EditOwnerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userSrv = GetUsersServ();
                var updClientResult = await userSrv.ClientUpdate(model);

                if (!updClientResult.isSuccess)
                {
                    foreach (var err in updClientResult.errors)
                    {
                        ModelState.AddModelError(string.Empty, err);
                    }
                    
                    return View();
                }

                var orgSrv = GetOrgServ();
                foreach (var orgItem in model.Orgs)
                {
                    var updOrgResult = await orgSrv.OrgUpdate(orgItem);
                    if (!updOrgResult.isSuccess)
                    {
                        foreach (var err in updOrgResult.errors)
                        {
                            ModelState.AddModelError(string.Empty, err);
                        }
                        
                        return View();
                    }
                }
                
            }
            
            return RedirectToAction(nameof(EditOwner), "ManageUsers", new { id = model.Id });
        }

        [HttpGet]
        public async Task<IActionResult> EditClient(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            
            EditClientViewModel model = new EditClientViewModel
            {
                Id = user.Id, 
                Name = user.UserName,
                Email = user.Email, 
                NewPassword = string.Empty,
                OldPassword = string.Empty,
                ConfirmNewPassword = string.Empty
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditClient(EditClientViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userSrv = GetUsersServ();
                var updClientResult = await userSrv.ClientUpdate(model);

                if (!updClientResult.isSuccess)
                {
                    foreach (var err in updClientResult.errors)
                    {
                        ModelState.AddModelError(string.Empty, err);
                    }
                    
                    return View();
                }              
                
                return RedirectToAction("Index");
            }
            
            return View();
        }
        
        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, result.Errors.ToString());
                }
            }
            
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<JsonResult> RemoveStaff(int orgId, string staffId)
        {
            var user = User;
            string userId = _userManager.GetUserId(user);
            var orgStaffServ = GetOrgStaffServ();
            bool removeResult = await orgStaffServ.RemoveStaffFromOrg(userId, orgId, staffId);
            return Json (new { result = removeResult });
        }


        private ManageUsersServ GetUsersServ()
        {
            if (_userSrv == null)
                _userSrv = new ManageUsersServ(_dbContext, _config, _userManager, _roleManager);
            return _userSrv;
        }

        private ManageOrgServ GetOrgServ()
        {
            if (_orgServ == null)
                _orgServ = new ManageOrgServ(_dbContext, _config, _userManager, _roleManager);
            return _orgServ;
        }
        
        private ManageOrgStaffServ GetOrgStaffServ()
        {
            if (_orgStaffServ == null)
                _orgStaffServ = new ManageOrgStaffServ(_dbContext, _config, _userManager, _roleManager);
            return _orgStaffServ;
        }
    }
}