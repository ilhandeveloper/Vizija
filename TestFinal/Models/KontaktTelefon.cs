

namespace TestFinal.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class KontaktTelefon
    {

        public int IDTel { get; set; }
        public int IDOsoba { get; set; }


        [StringLength(25, ErrorMessage = "Oznaka telefona može imati najvise 25 karaktera.")]
        [Display(Name = "Tip")]
        [Required(ErrorMessage = "Neophodno popuniti polje za tip (Kancelarija, službeni,itd.)")]
        public string OznakaTelefona { get; set; }


        [StringLength(15, ErrorMessage = "Broj telefona može imati najvise 15 karaktera.")]
        [Display(Name = "Broj telefona")]
        [Required(ErrorMessage = "Neophodno popuniti polje za broj telefona.")]
        public string BrojTelefona { get; set; }



        [StringLength(30, ErrorMessage = "Lokal može imati najvise 30 karaktera.")]
        [Display(Name = "Lokal")]
        [Required(ErrorMessage = "Neophodno popuniti polje za lokal.")]
        public string Lokal { get; set; }

        public virtual Kontakt Kontakt { get; set; }
    }
}
