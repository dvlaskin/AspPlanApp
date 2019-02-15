using System.Collections.Generic;

namespace AspPlanApp.Models.ManageUsersViewModels
{
    interface IEditStaff : IEditClient
    {
        int OrgId { get; set; }
        string OrgName { get; set; }
    }
    
    public class EditStaffViewModel : IEditStaff
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
        public int OrgId { get; set; }
        public string OrgName { get; set; }
    }
}