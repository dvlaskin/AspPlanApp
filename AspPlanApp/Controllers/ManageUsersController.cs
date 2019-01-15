﻿using System;
using System.Linq;
using System.Threading.Tasks;
using AspPlanApp.Models.DbModels;
using AspPlanApp.Models.ManageUsersViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AspPlanApp.Controllers
{
    [Authorize]
    public class ManageUsersController : Controller
    {
        private UserManager<User> _userManager;

        public ManageUsersController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        
        
        [HttpGet]
        public IActionResult Index()
        {
            
            return View(_userManager.Users.ToList());
        }
        
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            
            EditUserViewModel model = new EditUserViewModel
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
        public async Task<IActionResult> Edit(EditUserViewModel model)
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