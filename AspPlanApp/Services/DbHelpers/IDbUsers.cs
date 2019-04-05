using System.Collections.Generic;
using System.Threading.Tasks;
using AspPlanApp.Models.DbModels;
using AspPlanApp.Models.ManageUsersViewModels;

namespace AspPlanApp.Services.DbHelpers
{
    /// <summary>
    /// Interface to work with Users data 
    /// </summary>
    public interface IDbUsers
    {
        Task<EditOwnerViewModel> GetOwnerInfoAsync(User user);
        Task<(bool isSuccess, IEnumerable<string> errors)> ClientUpdateAsync(IEditClient updUser);
        Task<List<UsersListViewModel>> GetUserListAsync();
    }
}