using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace TestFinal.Models
{
    public class UserWithRoles
    {

        public string UserName { get; set; }

        [Required]
        public string Role { get; set; }

        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string Id { get; set; }

    }
}