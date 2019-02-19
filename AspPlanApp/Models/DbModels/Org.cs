namespace AspPlanApp.Models.DbModels
{

    public interface IEditOrg
    {
        int orgId { get; set; }
        string orgName { get; set; }
        string owner { get; set; }
        string address { get; set; }
        string city { get; set; }
        string country { get; set; }
        string phoneNumber { get; set; }
        int category { get; set; }
    }
    
    /// <summary>
    /// Organization Model
    /// </summary>
    public class Org : IEditOrg
    {
        
        public int orgId { get; set; }
        public string orgName { get; set; }
        public string owner { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string phoneNumber { get; set; }
        public int category { get; set; }
    }
}