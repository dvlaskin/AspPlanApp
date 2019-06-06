namespace AspPlanApp.Models.ReservViewModels
{
    public class UserCalendarViewModel : ReservBase
    {
        public int resId { get; set; }

        public bool isOwner { get; set; }
    }
}