using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TestFinal.Models;

namespace TestFinal.Controllers
{

    //Kontrola kojoj samo admin ima pristup
    [Authorize(Roles =RoleName.Admin)]
    public class KorisniciController : Controller
    {
        //Kontekstna promenljiva za  rad sa podacima baze Korisnici - identity
        private ApplicationDbContext db = new ApplicationDbContext();   
        
        //Metoda kojom prikazujemo podatke o postojecim korisnicima nase aplikacije
        public ViewResult Index()
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            if ((UserManager.FindByName("admin") == null) && (UserManager.FindByEmail("admin@admin.com") == null))
            {
                var user = new ApplicationUser();
                user.UserName = "admin";
                user.Email = "admin@admin.com";

                string userPWD = "admin1";

                var chkUser = UserManager.Create(user, userPWD);

                //Add default User to Role Admin   
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Admin");

                }
            }
            //Sql upit za uparivanje podataka o korisnicima sa njihovim pravima pristupa
            var sql = @"
            SELECT AspNetUsers.UserName, AspNetUsers.PasswordHash, AspNetUsers.Email,AspNetUsers.Id, AspNetRoles.Name As Role
            FROM AspNetUsers 
            LEFT JOIN AspNetUserRoles ON  AspNetUserRoles.UserId = AspNetUsers.Id 
            LEFT JOIN AspNetRoles ON AspNetRoles.Id = AspNetUserRoles.RoleId";
           
            var result = db.Database.SqlQuery<UserWithRoles>(sql).ToList();
            return View(result);        
        }


       //Metoda za brisanje izabranog korisnika
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        //Metoda za potvrdu brisanja izabranog korisnika
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            try
            {
                ApplicationUser applicationUser = db.Users.Find(id);
                db.Users.Remove(applicationUser);
                db.SaveChanges();

            }
            catch 
            {

                return RedirectToAction("NotFound", "Error");
            }
            return RedirectToAction("Index");
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
