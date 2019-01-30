using AspPlanApp.Models.DbModels;

namespace AspPlanApp.Models.ManageUsersViewModels
{
    public class OwnerOrgStaffViewModel : OrgStaff
    {
        public string userName { get; set; }
        public string emailUser { get; set; }
    }
}