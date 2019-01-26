namespace AspPlanApp.Models.DbModels
{
    /// <summary>
    /// Organization staff Model
    /// </summary>
    public class OrgStaff
    {
        public int orgStaffId { get; set; }
        public int orgId { get; set; }
        public int staffId { get; set; }
    }
}