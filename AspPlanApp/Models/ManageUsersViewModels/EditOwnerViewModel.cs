using System.Collections.Generic;
using AspPlanApp.Models.DbModels;

namespace AspPlanApp.Models.ManageUsersViewModels
{
    public class EditOwnerViewModel : EditClientViewModel
    {
        public IEnumerable<DbModels.Org> Orgs { get; set; } 
        
        public IEnumerable<OwnerOrgStaffViewModel> Staff { get; set; }
    }
}