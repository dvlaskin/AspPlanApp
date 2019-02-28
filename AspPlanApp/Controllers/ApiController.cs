using System.Collections.Generic;
using System.Threading.Tasks;
using AspPlanApp.Models;
using AspPlanApp.Models.DbModels;
using AspPlanApp.Models.ManageUsersViewModels;
using AspPlanApp.Services.DbHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AspPlanApp.Controllers
{
    public class ApiController : Controller
    {
        private UserManager<User> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private AppDbContext _dbContext;
        private IConfiguration _config;
        private DbOrgServ _dbOrgServ;
        
        public ApiController(
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

        /// <summary>
        /// Поиск организации по названию
        /// </summary>
        /// <param name="strOrg"></param>
        /// <returns></returns>
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
    }
}