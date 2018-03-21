//using System;
//using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Web;

namespace TestFinal.Models
{
    public class ResetUserPasswordViewModel
    {
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "User ID")]
        public string UserId { get; set; }


        [Required(ErrorMessage = "Neophodno je uneti lozinku.")]
        [StringLength(100, ErrorMessage = "{0} mora biti bar {2} karaktera dug.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Lozinka")]
        public string Password { get; set; }



        public string Code { get; set; }
    }
}