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
    public class KontaktController : Controller
    {
        //Kontekstna promenljiva za rad sa bazom o kontaktima
        private Vizija db = new Vizija();
        //Staticna promenljiva za cuvanje  podatka o konkretnom preduzecu
        static int pom = 0;

        //Metoda za prikazivanje podataka o svim kontaktima konkretnog preduzeca - ID
        public ViewResult Index(int id)
        {
          
            var kontakts = db.Kontakts.Include(k => k.GlavnaTabela).Where(k=> k.IDPreduzeca == id);
            
            ViewBag.Id = id;
            //Staticno cuvanje podatka o konkretnom preduzecu
            pom = id;
            //ViewBag za renderovanje naziva preduzeca
            ViewBag.Pom = db.GlavnaTabelas.Where(k => k.IDPreduzeca == id).Select(k => k.NazivPreduzeca).FirstOrDefault();

            if (User.IsInRole(RoleName.RE))
                return View("ReadIndex", kontakts.ToList());
            return View(kontakts.ToList());
        }


        //Metoda za prikaz podataka o konkretnom kontaktu
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kontakt kontakt = db.Kontakts.Find(id);
            if (kontakt == null)
            {
                return HttpNotFound();
            }

            //Korisnik RE ima uskracen pogled, ne moze da menja podatke
            if (User.IsInRole(RoleName.RE))
                return View("ReadDetails", kontakt);
            return View(kontakt);
        }

        
        //Metoda za uzimanje podataka za stvaranje novog kontakta konkretnog preduzeca
        [Authorize(Roles = RoleName.RWE + "," + RoleName.Admin)]
        public ActionResult Create(int id)
        {
            ViewBag.IDPreduzeca = new SelectList(db.GlavnaTabelas, "IDPreduzeca", "NazivPreduzeca");
            ViewBag.Id = id;
            return View();
        }

       
        //Metoda za kreiranje novog kontakta konkretnog preduzeca
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDOsoba,IDPreduzeca,Ime,Prezime,RadnoMesto")] Kontakt kontakt, int id)
        {
            //Rucno handlovanje unosa nezeljenih podataka
            try
            {
                if (ModelState.IsValid)
                {
                    kontakt.IDPreduzeca = id;
                    db.Kontakts.Add(kontakt);
                    db.SaveChanges();
                    return RedirectToAction("Index", new { id = id });
                }
            }
            catch 
            {
                return RedirectToAction("NotFound", "Error");               
            }
           

            ViewBag.IDPreduzeca = new SelectList(db.GlavnaTabelas, "IDPreduzeca", "NazivPreduzeca", kontakt.IDPreduzeca);
            return View(kontakt);
        }

        //Metoda za prihvatanje podataka o izmeni konkretnog kontakta
        [Authorize(Roles = RoleName.RWE + "," + RoleName.Admin)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Kontakt kontakt = db.Kontakts.Find(id);

            if (kontakt == null)
                return HttpNotFound();
            
            ViewBag.IDPreduzeca = new SelectList(db.GlavnaTabelas, "IDPreduzeca", "NazivPreduzeca", kontakt.IDPreduzeca);
            return View(kontakt);
        }

   
        //Metoda za vrsenje izmene podataka o konkretnom kontaktu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDOsoba,IDPreduzeca,Ime,Prezime,RadnoMesto")] Kontakt kontakt)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Pomocna promenljiva za uspesan povratak na listu kontakata konkretnog preduzeca
                    var rez = db.Kontakts.Where(k => k.IDPreduzeca == pom).Select(k => k.IDPreduzeca).FirstOrDefault();
                    kontakt.IDPreduzeca = rez;

                    db.Entry(kontakt).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index", new { id = kontakt.IDPreduzeca });
                }
            }
            catch 
            {
                return RedirectToAction("NotFound", "Error");
            }
           
            ViewBag.IDPreduzeca = new SelectList(db.GlavnaTabelas, "IDPreduzeca", "NazivPreduzeca", kontakt.IDPreduzeca);
            return View(kontakt);
        }

        //Metoda za brisanje kontakta
        [Authorize(Roles = RoleName.RWE + "," + RoleName.Admin)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            Kontakt kontakt = db.Kontakts.Find(id);
            if (kontakt == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id = id;
            return View(kontakt);
        }

        //Metoda za potvrdu o brisanju odredjenog kontakta, azuriranje baze
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
           
            Kontakt kontakt = db.Kontakts.Find(id);
            try
            {
                db.Kontakts.Remove(kontakt);
            }
                catch
            {
               return RedirectToAction("NotFound", "Error");
            }
            db.SaveChanges();
            return RedirectToAction("Index", new { id = kontakt.IDPreduzeca });
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
