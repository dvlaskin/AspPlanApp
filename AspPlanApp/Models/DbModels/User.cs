using Microsoft.AspNetCore.Identity;

namespace AspPlanApp.Models.DbModels
{
    /// <summary>
    /// App User Model
    /// </summary>
    public class User : IdentityUser
    {
        public int CompId { get; set; }
    }
}