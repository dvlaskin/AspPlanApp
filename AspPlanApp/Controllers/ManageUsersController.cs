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
                return RedirectToAction("EditOwner", "ManageUsers", new {id = userId});
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

            var dbServ = new ManageUsersServ(_dbContext, _config);

            EditOwnerViewModel viewModel = await dbServ.GetOwnerInfoAcync(owner);
            
            return View(viewModel);
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
                User user = await _userManager.FindByIdAsync(model.Id);

                if (user != null)
                {
                    user.Email = model.Email;
                    user.UserName = model.Name;
                    
                    IdentityResult result = await _userManager.UpdateAsync(user);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }

                        return View();
                    }

                    if (
                        !string.IsNullOrEmpty(model.OldPassword) &&
                        !string.IsNullOrEmpty(model.NewPassword) &&
                        !string.IsNullOrEmpty(model.ConfirmNewPassword)
                    )
                    {
                        bool checkPassResult = await _userManager.CheckPasswordAsync(user, model.OldPassword);

                        if (checkPassResult)
                        {
                            IdentityResult changePassResult =
                                await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                            if (!changePassResult.Succeeded)
                            {
                                foreach (var error in result.Errors)
                                {
                                    ModelState.AddModelError(string.Empty, error.Description);
                                }

                                return View();
                            }
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Password was not changed.");
                            return View();
                        }
                    }
                    
                    return RedirectToAction("Index");
                }
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
            }
            return RedirectToAction("Index");
        }
    }
}