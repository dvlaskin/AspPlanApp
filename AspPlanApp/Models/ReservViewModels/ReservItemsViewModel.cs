using System;

namespace AspPlanApp.Models.ReservViewModels
{
    public class ReservItemsViewModel
    {
        public int ordId { get; set; }
        public string orgName { get; set; }
        public string staffName { get; set; }
        public bool isMy { get; set; }
        public DateTime dateFrom { get; set; }
        public DateTime dateTo { get; set; }
        public bool isConfirm { get; set; }
        public string comment { get; set; }
    }
}