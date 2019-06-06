using System.Threading.Tasks;

namespace AspPlanApp.Services.DbHelpers
{
    /// <summary>
    /// Interface to work with Category data 
    /// </summary>
    public interface IDbCategory
    {
        Task<Models.DbModels.Category[]> GetCatListAsync();
    }
}