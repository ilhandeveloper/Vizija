//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.EntityFramework;
//using Microsoft.AspNet.Identity.Owin;
//using Microsoft.Owin;
//using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace TestFinal.Models
{
  
    
  
    public class LoginViewModel
    {

        [Required(ErrorMessage = "Polje za korisničko ime mora biti popunjeno.")]
        [Display(Name = "Korisničko ime")]
        [StringLength(25, ErrorMessage = "Korisničko ime može imati najvise 25 karaktera.")]
        public string UserName { get; set; }



        [Required(ErrorMessage = "Polje za lozinku mora biti popunjeno.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [Display(Name ="UserRoles")]
        public string UserRoles { get; set; }


        public string Id { get; set; }


        [Required(ErrorMessage = "Polje za Email mora biti popunjeno.")]
        [EmailAddress(ErrorMessage ="Nije validna Email adresa.")]
        [StringLength(30, ErrorMessage = "Email može imati najvise 30 karaktera.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Polje za korisničko ime mora biti popunjeno.")]
        [StringLength(20, ErrorMessage = "Username može imati najvise 20 karaktera.")]
        [Display(Name = "Korisničko ime")]
        public string UserName { get; set; }



        [Required(ErrorMessage = "Polje za lozinku mora biti popunjeno.")]
        [StringLength(100, ErrorMessage = "{0} mora biti bar {2} karaktera dug.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Lozinka")]
        public string Password { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "Potvrdi lozinku")]
        [Compare("Password", ErrorMessage = "Lozinke se ne poklapaju.")]
        public string ConfirmPassword { get; set; }
    }

   
}
