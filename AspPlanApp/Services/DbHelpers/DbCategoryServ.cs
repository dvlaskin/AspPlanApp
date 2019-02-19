using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspPlanApp.Models;
using Microsoft.EntityFrameworkCore;

namespace AspPlanApp.Services.DbHelpers
{
    public class DbCategoryServ
    {
        private static AppDbContext _dbContext;
        
        public DbCategoryServ(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Get array all categories
        /// </summary>
        /// <returns></returns>
        public static async Task<Models.DbModels.Category[]> GetCatListAsync()
        {
            return await _dbContext.Category.ToArrayAsync();
        }
    }
}