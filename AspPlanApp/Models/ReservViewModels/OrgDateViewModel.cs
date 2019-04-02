using System;
using System.Collections;


namespace AspPlanApp.Models.ReservViewModels
{
    public class OrgDateViewModel
    {
        public int orgId { get; set; }
        public DateTime dateCal { get; set; }
        public string orgName { get; set; }
        public StaffInfo[] staffInfo { get; set; }
    }

    public class StaffInfo
    {
        public int orgStaffId { get; set; }
        public string staffName { get; set; }
    }
}