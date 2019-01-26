using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Identity;

namespace AspPlanApp.Models.DbModels
{
    /// <summary>
    /// Organization Model
    /// </summary>
    public class Org
    {
        
        public int orgId { get; set; }
        public string orgName { get; set; }
        public string owner { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string phoneNumber { get; set; }

    }
}