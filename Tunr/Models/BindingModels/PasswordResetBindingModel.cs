using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tunr.Models.BindingModels
{
    public class PasswordResetBindingModel
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string ResetToken { get; set; }
        [Required]
        [DisplayName("New Password")]
        public string NewPassword { get; set; }
        [Required]
        [Compare("NewPassword")]
        [DisplayName("Confirm New Password")]
        public string ConfirmNewPassword { get; set; }
    }
}