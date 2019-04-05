using System.Threading.Tasks;
using AspPlanApp.Models;
using Microsoft.EntityFrameworkCore;

namespace AspPlanApp.Services.DbHelpers
{
    public class DbCategory : IDbCategory
    {
        private readonly AppDbContext _dbContext;
        
        public DbCategory(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Get array all categories
        /// </summary>
        /// <returns></returns>
        public async Task<Models.DbModels.Category[]> GetCatListAsync()
        {
            return await _dbContext.Category.ToArrayAsync();
        }
    }
}