using System.ComponentModel.DataAnnotations;

namespace AspPlanApp.Models.ManageUsersViewModels
{
    public class EditUserViewModel
    {
        
        public string Id { get; set; }
        
        [Required]
        [Display(Name = "Name")]
        [MinLength(3, ErrorMessage = "The Name must be at least 3 and at max 25 characters long.")]
        [MaxLength(25, ErrorMessage = "The Name must be at least 3 and at max 25 characters long.")]
        public string Name { get; set; }
        
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        [DataType(DataType.Password)]
        [Display(Name = "Old Password")]
        public string OldPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmNewPassword { get; set; }
    }
}