using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AspPlanApp.Models;
using AspPlanApp.Services.DbHelpers;
using Microsoft.AspNetCore.Mvc;


namespace AspPlanApp.Controllers
{
    public class HomeController : Controller
    {
        private AppDbContext _dbContext;
        private DbCategoryServ _dbCategoryServ;
        
        public HomeController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            
            DbServInitialization();
        }
        
        private void DbServInitialization()
        {
//            _dbUsersServ = new DbUsersServ(_dbContext, _config, _userManager, _roleManager);
//            _dbOrgServ = new DbOrgServ(_dbContext, _config, _userManager, _roleManager);
//            _dbOrgStaffServ = new DbOrgStaffServ(_dbContext, _config, _userManager, _roleManager);
            
            if (_dbCategoryServ == null)
            _dbCategoryServ = new DbCategoryServ(_dbContext);
            
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Categories = await DbCategoryServ.GetCatListAsync();
            
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
