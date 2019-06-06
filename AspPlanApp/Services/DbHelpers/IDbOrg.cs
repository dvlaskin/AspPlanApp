using System.Collections.Generic;
using System.Threading.Tasks;
using AspPlanApp.Models.DbModels;

namespace AspPlanApp.Services.DbHelpers
{
    /// <summary>
    /// Interface to work with Org data 
    /// </summary>
    public interface IDbOrg
    {
        Task<(bool isSuccess, IEnumerable<string> errors)> OrgUpdateAsync(IEditOrg updOrg);

        bool CompareOrgs(IEditOrg updOrg, ref Models.DbModels.Org sourceOrg);

        Task<Models.DbModels.Org> GetOrgByIdAsync(int orgId);

        List<Models.DbModels.Org> GetOrgsByOwner(string ownerId);
        
        Task<IEnumerable<Models.DbModels.Org>> SearchOrgName(string strOrgName);

        Task<bool> AddNewOrg(Models.DbModels.Org org);

        Task<bool> DeleteOrg(string userId, int orgId);
    }
}