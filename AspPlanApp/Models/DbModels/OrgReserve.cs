using System;

namespace AspPlanApp.Models.DbModels
{
    /// <summary>
    /// Organization Reserved calendar Model
    /// </summary>
    public class OrgReserve
    {
        public int resId { get; set; }
        public int orgId { get; set; }
        public string userId { get; set; }
        public int orgStaffId { get; set; }
        public DateTime dateFrom { get; set; }
        public DateTime dateTo { get; set; }
        public bool isConfirm { get; set; }
        public string Comment { get; set; }
    }
}