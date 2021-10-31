using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChildCareSystem.ViewModels
{
    public class UserProfileViewModel
    {
        public string Email { get; set; }

        [Required(ErrorMessage = "Name can not be empty.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Address can not be empty.")]
        public string Address { get; set; }
       
        [Required(ErrorMessage = "Phone number can not be empty.")]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Current password can not be empty.")]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        
    }
}
