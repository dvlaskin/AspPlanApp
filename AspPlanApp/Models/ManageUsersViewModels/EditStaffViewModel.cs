using System.Collections.Generic;

namespace AspPlanApp.Models.ManageUsersViewModels
{
    interface IEditStaff : IEditClient
    {
        int OrgId { get; set; }
        string OrgName { get; set; }
    }
    
    public class EditStaffViewModel : EditClientViewModel, IEditStaff
    {
        public int OrgId { get; set; }
        public string OrgName { get; set; }
    }
}