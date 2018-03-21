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
    public class KontaktTelefonsController : Controller
    {
        //Kontekstna promenljiva za rad sa podacima telefon baze
        private Vizija db = new Vizija();

        //Promenljiva koja nam je pomogla da prosledimo pravi ID u Create akciju
        static int pom = 0;
        

        //Metoda za prikaz liste telefona odredjenog kontakta
        public ViewResult Index(int id)
        {
            var kontaktTelefons = db.KontaktTelefons.Include(k => k.Kontakt).Where(k=> k.IDOsoba == id);
            pom = id;
            //ViewBag-ovi za prenos podataka  o odredjenom kontaktu
            ViewBag.Idiot = db.Kontakts.Where(k => k.IDOsoba == id).Select(k => k.IDPreduzeca).FirstOrDefault();
            ViewBag.ImeOsobe = db.Kontakts.Where(k => k.IDOsoba == id).Select(k => k.Ime).FirstOrDefault();
            ViewBag.PrezimeOsobe = db.Kontakts.Where(k => k.IDOsoba == id).Select(k => k.Prezime).FirstOrDefault();

            if (User.IsInRole(RoleName.RE))
                return View("ReadIndex", kontaktTelefons.ToList());
            return View(kontaktTelefons.ToList());
        }

        //Metoda za prikaz detalja o izabranom telefonu
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KontaktTelefon kontaktTelefon = db.KontaktTelefons.Find(id);
            if (kontaktTelefon == null)
            {
                return HttpNotFound();
            }

            if (User.IsInRole(RoleName.RE))
                return View("ReadDetails", kontaktTelefon);
            return View(kontaktTelefon);
        }

        //Metoda za kreiranje telefona za izabranog kontakta. Ovim metodama mogu pristupiti admin i RWE korisnik
        [Authorize(Roles = RoleName.RWE + "," + RoleName.Admin)]
        public ViewResult Create(int id)
        {
            ViewBag.IDOsoba = new SelectList(db.Kontakts, "IDOsoba", "Ime");
            ViewBag.Id = db.Kontakts.Where(k => k.IDPreduzeca == id).Select(k => k.IDOsoba).FirstOrDefault();
            return View();
        }
        //Metoda  kreiranja telefona za izabranog kontakta 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDTel,IDOsoba,OznakaTelefona,BrojTelefona,Lokal")] KontaktTelefon kontaktTelefon)
        {
            //Try nam omogucava da uhvatimo nezeljene greske prilikom koriscenja create metode
            try
            {
                if (ModelState.IsValid)
                {
                    var rez = db.Kontakts.Where(k => k.IDOsoba == pom).Select(k => k.IDOsoba).FirstOrDefault();
                    kontaktTelefon.IDOsoba = rez;
                    db.KontaktTelefons.Add(kontaktTelefon);
                    db.SaveChanges();
                    return RedirectToAction("Index", new { id = kontaktTelefon.IDOsoba });
                }

            }
            catch 
            {
                return RedirectToAction("NotFound", "Error");
            }
           
            ViewBag.IDOsoba = new SelectList(db.Kontakts, "IDOsoba", "Ime", kontaktTelefon.IDOsoba);
            return View(kontaktTelefon);
        }

        //Metoda koja dobija podatke o izmeni odredjenog telefona
        [Authorize(Roles = RoleName.RWE + "," + RoleName.Admin)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KontaktTelefon kontaktTelefon = db.KontaktTelefons.Find(id);
            if (kontaktTelefon == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDOsoba = new SelectList(db.Kontakts, "IDOsoba", "Ime", kontaktTelefon.IDOsoba);
            return View(kontaktTelefon);
        }
        
        //Metoda koja omogucava menjanje podataka o telefonu odredjenog kontakta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDTel,IDOsoba,OznakaTelefona,BrojTelefona,Lokal")] KontaktTelefon kontaktTelefon)
        {
            if (ModelState.IsValid)
            {              
                try
                {
                    //LINQ upit koji nam omogucava da uzmemo ID selektovanog Kontakta, tu dolazi u pomoc staticna promenljiva pom
                    var rez = db.Kontakts.Where(k => k.IDOsoba == pom).Select(k => k.IDOsoba).FirstOrDefault();
                    kontaktTelefon.IDOsoba = rez;
                    db.Entry(kontaktTelefon).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch 
                {
                    return RedirectToAction("NotFound", "Error");
                }              
                return RedirectToAction("Index", new { id = kontaktTelefon.IDOsoba });
            }
            ViewBag.IDOsoba = new SelectList(db.Kontakts, "IDOsoba", "Ime", kontaktTelefon.IDOsoba);
            return View(kontaktTelefon);
        }

        //Metoda koja brise telefon izabranog kontakta
        [Authorize(Roles = RoleName.RWE + "," + RoleName.Admin)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KontaktTelefon kontaktTelefon = db.KontaktTelefons.Find(id);
            if (kontaktTelefon == null)
            {
                return HttpNotFound();
            }
            return View(kontaktTelefon);
        }

        //Metoda kojom potvrdjujemo brisanje izabranog telefona
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            { 
            KontaktTelefon kontaktTelefon = db.KontaktTelefons.Find(id);
            db.KontaktTelefons.Remove(kontaktTelefon);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = kontaktTelefon.IDOsoba });
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
