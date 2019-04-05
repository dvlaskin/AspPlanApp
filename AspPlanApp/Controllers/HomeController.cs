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
        private readonly IDbCategory _dbCategory;
        
        public HomeController(IDbCategory dbCategory)
        {
            _dbCategory = dbCategory;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Categories = await _dbCategory.GetCatListAsync();
            
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
