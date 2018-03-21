

namespace TestFinal.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web;

    public partial class GlavnaTabela
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GlavnaTabela()
        {
            this.Kontakts = new HashSet<Kontakt>();
        }


        public int IDPreduzeca { get; set; }
        [StringLength(30, ErrorMessage = "Naziv preduzeća može imati najviše 30 karaktera.")]
        [Display(Name = "Naziv preduzeća")]
        [Required(ErrorMessage = "Neophodno popuniti naziv preduzeća.")]

        public string NazivPreduzeca { get; set; }



        [Display(Name = "Adresa")]
        [Required(ErrorMessage = "Neophodno popuniti polje za adresu.")]
        [StringLength(40, ErrorMessage = "Adresa može imati najviše 40 karaktera.")]
        public string AdresaRegistracijePreduzeca { get; set; }

        [StringLength(20, ErrorMessage = "Opština može imati najvise 20 karaktera.")]
        [Display(Name = "Opština")]
        [Required(ErrorMessage = "Neophodno popuniti polje za opštinu.")]
        public string Opstina { get; set; }

        [StringLength(15, ErrorMessage = "Matični broj može imati najvise 15 karaktera.")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Podatak mora biti broj.")]
        [Display(Name = "Matični broj")]
        [Required(ErrorMessage = "Neophodno popuniti polje za matični broj.")]
        public string MaticniBrojPreduzeca { get; set; }



        [StringLength(30, ErrorMessage = "PIB može imati najvise 30 karaktera.")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Podatak mora biti broj.")]
        [Display(Name = "PIB")]
        [Required(ErrorMessage = "Neophodno popuniti polje za PIB.")]
        public string PIB { get; set; }



        [StringLength(30, ErrorMessage = "Broj računa može imati najvise 30 karaktera.")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Podatak mora biti broj.")]
        [Display(Name = "Broj računa")]
        [Required(ErrorMessage = "Neophodno popuniti polje za broj računa.")]
        public string BrRacuna { get; set; }


        [StringLength(50, ErrorMessage = "Veb stranica može imati najvise 50 karaktera.")]

        [Display(Name = "Veb stranica")]
        [Required(ErrorMessage = "Neophodno popuniti polje za veb sajt.")]
        public string WebStranica { get; set; }
        [Display(Name = "Pečat")]

        public byte[] Fotografija { get; set; }



        public HttpPostedFileBase ImageFile { get; set; }


        [StringLength(250, ErrorMessage = "Beleška možeimati najvise 250 karaktera.")]
        [Display(Name = "Beleška")]
        [Required(ErrorMessage = "Neophodno popuniti polje za belešku.")]
        public string Beleska { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Kontakt> Kontakts { get; set; }
    }
}
