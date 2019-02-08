using System.Collections.Generic;
using AspPlanApp.Models.DbModels;

namespace AspPlanApp.Models.ManageUsersViewModels
{
    public interface IEditOwner : IEditClient
    {
        IEnumerable<DbModels.Org> Orgs { get; set; } 
        
        IEnumerable<OwnerOrgStaffViewModel> Staff { get; set; }
    }
    
    public class EditOwnerViewModel : EditClientViewModel
    {
        public List<DbModels.Org> Orgs { get; set; } 
        
        public List<OwnerOrgStaffViewModel> Staff { get; set; }
    }
}