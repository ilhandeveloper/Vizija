

namespace TestFinal.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class KontaktMail
    {
        public int IDMail { get; set; }

        public int IDOsoba { get; set; }


        [StringLength(25, ErrorMessage = "Oznaka posla može imati najvise 25 karaktera.")]
        [Display(Name = "Tip adrese")]
        [Required(ErrorMessage = "Neophodno popuniti polje za tip adrese (Privatna, poslovna, itd.)")]
        public string OznakaPosla { get; set; }

        [StringLength(30, ErrorMessage = "Email može imati najvise 30 karaktera.")]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Neophodno popuniti polje za email.")]
        [EmailAddress(ErrorMessage = "Podatak mora biti Email adresa.")]
        public string Adresa { get; set; }

        public virtual Kontakt Kontakt { get; set; }
    }
}
