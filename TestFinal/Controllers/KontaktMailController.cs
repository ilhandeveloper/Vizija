using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TestFinal.Models;

namespace TestFinal.Controllers
{
    public class KontaktMailController :Controller
    {
        //Kontekstna promenljiva za rad sa bazom mejlova kontakta
        private Vizija db = new Vizija();
        //Promenljiva za prenos podataka sa metode na metodu
        static int pom = 0;
        //Metoda za prikazivanje podataka o svim mejlovima konkretnog kontakta - ID
        public ViewResult Index(int id)
        {

            pom = id;
            var kontaktss = db.KontaktMails.Include(k => k.Kontakt).Where(k => k.IDOsoba == id);
            //ViewBag-ovi za renderovanje podataka o kontaktu koji ima vise mejlova
            ViewBag.Id = db.Kontakts.Where(k => k.IDOsoba == id).Select(k => k.IDPreduzeca).FirstOrDefault();
            ViewBag.ImeOsobe = db.Kontakts.Where(k => k.IDOsoba == id).Select(k => k.Ime).FirstOrDefault();
            ViewBag.PrezimeOsobe = db.Kontakts.Where(k => k.IDOsoba == id).Select(k => k.Prezime).FirstOrDefault();

            //Redirekcija za korisnika sa pravom pristupa RE, koji nema pravo kreiranja, brisanja i izmene
            if (User.IsInRole(RoleName.RE))
                return View("ReadIndex", kontaktss.ToList());
            return View(kontaktss.ToList());
        }

        //Metoda za prikaz mejlova odredjenog kontakta
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KontaktMail kontaktMail = db.KontaktMails.Find(id);
            if (kontaktMail == null)
            {
                return HttpNotFound();
            }
            //Redirekcija za korisnika sa pravom pristupa RE
            if (User.IsInRole(RoleName.RE))
                return View("ReadDetails", kontaktMail);
            return View(kontaktMail);
        }

        
        //Metoda za uzimanje podataka za kreiranje novog mejla konkretnog kontakta, pravo pristupa imaju Admin i RWE
        [Authorize(Roles = RoleName.RWE + "," + RoleName.Admin)]
        public ViewResult Create(int id)
        {
            ViewBag.IDOsoba = new SelectList(db.Kontakts, "IDOsoba", "Ime");
            ViewBag.Id = db.Kontakts.Where(k => k.IDPreduzeca == id).Select(k => k.IDOsoba).FirstOrDefault();
            return View();
        }

        //Metoda za kreiranje novog mejla odredjenog korisnika
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDMail,IDOsoba,OznakaPosla,Adresa")] KontaktMail kontaktMail)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var rez = db.Kontakts.Where(k => k.IDOsoba == pom).Select(k => k.IDOsoba).First();
                    kontaktMail.IDOsoba = rez;
                    db.KontaktMails.Add(kontaktMail);
                    db.SaveChanges();
                    return RedirectToAction("Index", new { id = kontaktMail.IDOsoba });
                }
            }
            catch 
            {
                return RedirectToAction("NotFound", "Error");
            }
            
            ViewBag.IDOsoba = new SelectList(db.Kontakts, "IDOsoba", "Ime", kontaktMail.IDOsoba);
            return View(kontaktMail);
        }

       
        //Metoda za prihvatanje podataka o izmeni odredjenog mejla za konkretnog kontakta
        [Authorize(Roles = RoleName.RWE + "," + RoleName.Admin)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KontaktMail kontaktMail = db.KontaktMails.Find(id);
            if (kontaktMail == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDOsoba = new SelectList(db.Kontakts, "IDOsoba", "Ime", kontaktMail.IDOsoba);
           
            return View(kontaktMail);
        }

        //Metoda za izmenu mejla konkretnog kontakta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDMail,IDOsoba,OznakaPosla,Adresa")] KontaktMail kontaktMail)
        {

            //Handlovanje nezeljenih situacija
            try
            {
                if (ModelState.IsValid)
                {
                    var rez = db.Kontakts.Where(k => k.IDOsoba == pom).Select(k => k.IDOsoba).FirstOrDefault();
                    kontaktMail.IDOsoba = rez;
                    db.Entry(kontaktMail).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index", new { id = kontaktMail.IDOsoba });
                }
            }
            catch 
            {
                return RedirectToAction("NotFound", "Error");
            }
           
           ViewBag.IDOsoba = new SelectList(db.Kontakts, "IDOsoba", "Ime", kontaktMail.IDOsoba);
           return View(kontaktMail);
        }
        
        //Metoda za brisanje mejla izabranog kontakta
        [Authorize(Roles = RoleName.RWE + "," + RoleName.Admin)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KontaktMail kontaktMail = db.KontaktMails.Find(id);
            if (kontaktMail == null)
            {
                return HttpNotFound();
            }
            return View(kontaktMail);
        }

        // Potvrda brisanja izabranog kontakta
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //Handlovanje nezeljenih inputa
            try
            {
                KontaktMail kontaktMail = db.KontaktMails.Find(id);
                db.KontaktMails.Remove(kontaktMail);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = kontaktMail.IDOsoba });

            }
          catch
            {
                return RedirectToAction("NotFound", "Error");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
