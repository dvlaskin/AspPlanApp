using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AspPlanApp.Models;
using AspPlanApp.Models.DbModels;
using AspPlanApp.Models.ManageUsersViewModels;
using AspPlanApp.Services.DbHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
        private DbOrgReservServ _dbOrgReservServ;
        
        
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
            
            if (_dbOrgReservServ == null)
                _dbOrgReservServ = new DbOrgReservServ(_dbContext, _config, _userManager, _roleManager);
        }

        /// <summary>
        /// Поиск организации по названию
        /// </summary>
        /// <param name="strOrg"></param>
        /// <param name="catId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> SearchOrgByName(string strOrg, int catId=0)
        {
            var orgArray = await DbOrgServ.SearchOrgName(strOrg);
            if (catId != 0)
            {
                orgArray = orgArray.Where(w => w.category == catId).ToArray();
            }
            
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

        /// <summary>
        /// Получить перечень зарезервированных событий организации на неделе от даты
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="dateCal"></param>
        /// <returns></returns>
        public async Task<JsonResult> GetReserv(int orgId, DateTime dateCal)
        {
            if (orgId == 0 || dateCal == DateTime.MinValue)
            {
                return null;
            }
            
            var user = User;
            string userId = _userManager.GetUserId(user);

            var result = await DbOrgReservServ.GetOrgReservByWeek(orgId, dateCal, userId);
            
            return Json(result);
        }
    }
}