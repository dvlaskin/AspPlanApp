using System.Collections.Generic;
using AspPlanApp.Models.DbModels;

namespace AspPlanApp.Models.ManageUsersViewModels
{
    public interface IEditOwner : IEditClient
    {
        List<DbModels.Org> Orgs { get; set; } 
        
        List<OwnerOrgStaffViewModel> Staff { get; set; }
    }
    
    public class EditOwnerViewModel : EditClientViewModel, IEditOwner
    {
        public List<DbModels.Org> Orgs { get; set; } 
        
        public List<OwnerOrgStaffViewModel> Staff { get; set; }
    }
}