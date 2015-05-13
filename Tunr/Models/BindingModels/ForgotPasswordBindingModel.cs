using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tunr.Models.BindingModels
{
    public class ForgotPasswordBindingModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}