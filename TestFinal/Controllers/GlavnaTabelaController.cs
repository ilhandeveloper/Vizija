using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using TestFinal.Models;

namespace TestFinal.Controllers
{
    [SessionState(SessionStateBehavior.Default)]
    public class GlavnaTabelaController :Controller
    {
        //Pomocna promenjliva za rad za slikama, prenos podataka sa metode na metodu
        static byte[] pom;
        public void InitializeController(RequestContext context)
        {
            base.Initialize(context);
        }
        
        //Konteksta promenljiva za rad sa bazom o formularima
        private Vizija db = new Vizija();

        
        public ViewResult Index()
        {
            //Redirekcija prikaza na osnovu prava pristupa
            if (User.IsInRole(RoleName.Admin) || User.IsInRole(RoleName.RWE))
            {
                return View(db.GlavnaTabelas.ToList());
            }
            return View("ReadIndex",db.GlavnaTabelas.ToList());
        }
    
        //Metoda za izlistavanje podataka o formularima preduzeca
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GlavnaTabela glavnaTabela = db.GlavnaTabelas.Find(id);
            if (glavnaTabela == null)
            {
                return HttpNotFound();
            }
            
            //Redirekcija pogleda na podatke o formularima na osnovu prava pristupa
            if (User.IsInRole(RoleName.RE))
                return View("ReadDetails", glavnaTabela);
            return View(glavnaTabela);
        }


        //Metoda za prihvatanje podataka za kreiranje preduzeca
        [Authorize(Roles =RoleName.RWE+ ","+ RoleName.Admin)]
        public ActionResult Create()
        {
            return View();
        }

        
        //Metoda za kreiranje preduzeca
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDPreduzeca,NazivPreduzeca,AdresaRegistracijePreduzeca,Opstina,MaticniBrojPreduzeca,PIB,BrRacuna,WebStranica,Fotografija,Beleska")] GlavnaTabela glavnaTabela,HttpPostedFileBase image1)
        {
            
            if (ModelState.IsValid)
            {
                //Provera da li je uneta slika, u slucaju da nije postupiti pravilno
                if (image1!=null)
                {                
                    glavnaTabela.Fotografija = new byte[image1.ContentLength];
                    image1.InputStream.Read(glavnaTabela.Fotografija, 0, image1.ContentLength);
                }
                db.GlavnaTabelas.Add(glavnaTabela);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(glavnaTabela);
        }

        
        //Metoda za prihvatanje podataka o izmeni preduzeca
        [Authorize(Roles = RoleName.RWE + "," + RoleName.Admin)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GlavnaTabela glavnaTabela = db.GlavnaTabelas.Find(id);

            pom = glavnaTabela.Fotografija;

           
            if (glavnaTabela == null)
            {
                return HttpNotFound();
            }
            return View(glavnaTabela);
        }

        //Metoda za izmenu preduzeca
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDPreduzeca,NazivPreduzeca,AdresaRegistracijePreduzeca,Opstina,MaticniBrojPreduzeca,PIB,BrRacuna,WebStranica,Fotografija,Beleska")] GlavnaTabela glavnaTabela, HttpPostedFileBase image1)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //Prosirenje za editovanje slike
                    if (image1 != null)
                    {
                        glavnaTabela.Fotografija = new byte[image1.ContentLength];
                        image1.InputStream.Read(glavnaTabela.Fotografija, 0, image1.ContentLength);
                    }
                    else
                        glavnaTabela.Fotografija = pom;
                    db.Entry(glavnaTabela).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch
                {
                    return RedirectToAction("NotFound", "Error");
                }
                return RedirectToAction("Index");
            }
            return View(glavnaTabela);
        }



        //Metoda za brisanje konkretnog preduzeca
        [Authorize(Roles = RoleName.RWE + "," + RoleName.Admin)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GlavnaTabela glavnaTabela = db.GlavnaTabelas.Find(id);
            if (glavnaTabela == null)
            {
                return HttpNotFound();
            }
            return View(glavnaTabela);
        }

        //Metoda za potvrdu o brisanju
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //U slucaju pokusaja brisanja nepostojeceg, redirekcija na stranu za greske
            try
            {
                GlavnaTabela glavnaTabela = db.GlavnaTabelas.Find(id);
                db.GlavnaTabelas.Remove(glavnaTabela);
                db.SaveChanges();
                return RedirectToAction("Index");
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
