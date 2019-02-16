using System;
using System.Collections.Generic;
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
        private DbUsersServ _dbUsersServ;
        private DbOrgServ _dbOrgServ;
        private DbOrgStaffServ _dbOrgStaffServ;

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

            DbServInitialization();
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
            else if (user.IsInRole(AppRoles.Staff))
            {
                return RedirectToAction(nameof(EditStaff), "ManageUsers", new {id = userId});
            }
            else
            {
                return RedirectToAction(nameof(EditClient), "ManageUsers", new { id = userId});
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
           
            EditOwnerViewModel viewModel = await DbUsersServ.GetOwnerInfoAsync(owner);                                                
           
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditOwner(EditOwnerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var updClientResult = await DbUsersServ.ClientUpdateAsync(model);

                if (!updClientResult.isSuccess)
                {
                    foreach (var err in updClientResult.errors)
                    {
                        ModelState.AddModelError(string.Empty, err);
                    }
                    
                    return View();
                }

                foreach (var orgItem in model.Orgs)
                {
                    var updOrgResult = await DbOrgServ.OrgUpdateAsync(orgItem);
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
        public async Task<IActionResult> EditStaff(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            Models.DbModels.Org orgData = await DbOrgStaffServ.GetOrgByStaffIdAsync(user.Id);
            
            EditStaffViewModel model = new EditStaffViewModel()
            {
                Id = user.Id, 
                Name = user.UserName,
                Email = user.Email, 
                NewPassword = string.Empty,
                OldPassword = string.Empty,
                ConfirmNewPassword = string.Empty,
                OrgId = 0,
                OrgName = string.Empty
            };

            if (orgData != null)
            {
                model.OrgId = orgData.orgId;
                model.OrgName = orgData.orgName;
            }
            
            
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditStaff(EditStaffViewModel model)
        {
            if (ModelState.IsValid)
            {
                var updClientResult = await DbUsersServ.ClientUpdateAsync(model);

                if (!updClientResult.isSuccess)
                {
                    foreach (var err in updClientResult.errors)
                    {
                        ModelState.AddModelError(string.Empty, err);
                    }
                    
                    return View();
                }

                var updOrgResult = await DbOrgStaffServ.ChangeOrgForStaff(model.Id, model.OrgId);
                if (!updOrgResult)
                {
                    ModelState.AddModelError(string.Empty, "Company has not changed.");
                    return View();
                }
            }
            
            return RedirectToAction(nameof(EditStaff), "ManageUsers", new { id = model.Id });
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
                var updClientResult = await DbUsersServ.ClientUpdateAsync(model);

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
            bool removeResult = await DbOrgStaffServ.RemoveStaffFromOrgAsync(userId, orgId, staffId);
            return Json (new { result = removeResult });
        }

        [HttpPost]
        public async Task<JsonResult> ConfirmNewStaff(int orgId, string staffId)
        {
            var user = User;
            string userId = _userManager.GetUserId(user);
            bool removeResult = await DbOrgStaffServ.ConfirmNewStaffAsync(userId, orgId, staffId);
            return Json(new { result = removeResult });
        }

        [HttpGet]
        public async Task<JsonResult> SearchOrgByName(string strOrg)
        {
            var orgArray = await DbOrgServ.SearchOrgName(strOrg);
            List<SearchOrgViewModel> orgInfo = new List<SearchOrgViewModel>();
            foreach (var item in orgArray)
            {
                orgInfo.Add(new SearchOrgViewModel()
                    {
                        orgId = item.orgId,
                        orgName = item.orgName,
                        orgInfo = string.Format($"{item.city} / {item.address}")
                    }
                );
            }

            return Json(orgInfo);
        }

        private void DbServInitialization()
        {
            if (_dbUsersServ == null)
                _dbUsersServ = new DbUsersServ(_dbContext, _config, _userManager, _roleManager);
            if (_dbOrgServ == null)
                _dbOrgServ = new DbOrgServ(_dbContext, _config, _userManager, _roleManager);
            if (_dbOrgStaffServ == null)
                _dbOrgStaffServ = new DbOrgStaffServ(_dbContext, _config, _userManager, _roleManager);
        }
    }
}