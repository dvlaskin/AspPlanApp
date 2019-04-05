using System.Threading.Tasks;
using AspPlanApp.Models.DbModels;

namespace AspPlanApp.Services.DbHelpers
{
    /// <summary>
    /// Interface to work with Org Staff data 
    /// </summary>
    public interface IDbOrgStaff
    {
        Task<bool> AddNewStaff(string userId, int orgId);
        Task<bool> ChangeOrgForStaff(string userId, int orgId);
        Task<bool> RemoveStaffFromOrgAsync(string ownerId, int orgId, string staffId);
        Task<bool> ConfirmNewStaffAsync(string ownerId, int orgId, string staffId);
        Task<OrgStaff> GetOrgStaffAsync(int orgId, string staffId);
        Task<Models.DbModels.Org> GetOrgByStaffIdAsync(string staffId);
        Task<OrgStaff[]> GetOrgStaffByOrgId(int orgId);
    }
}