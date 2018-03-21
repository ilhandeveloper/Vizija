using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using TestFinal.Models;

namespace TestFinal.Controllers
{

  

    
   
    public class AccountController :Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;


        //Konteksna promenljiva za konekciju sa bazom za korisnike
        ApplicationDbContext context;

        public AccountController()
        {

            context = new ApplicationDbContext();
        }


        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }



        //Metoda za logovanje, kojoj mogu pristupiti svi korisnici
        [AllowAnonymous]
        public ViewResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

    

        //Metoda za proveravanje validacije unosa prilikom prijave i pokretanja sesije u slucaju uspeha
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


           
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:       
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Uneli ste pogrešno korisničko ime ili lozinku.");
                    return View(model);
            }
        }




        //Metoda za prihvatanje podataka za kreiranje novih korisnika, dostupna samo Adminu
       [Authorize(Roles = RoleName.Admin)]
      //  [AllowAnonymous]
        public ViewResult Register()
        {

            //ViewBag za izlistavanje rola
            ViewBag.Name = new SelectList(context.Roles.ToList(), "Name", "Name");
            return View();
        }




        //Metoda za kreiranje korisnika
        [HttpPost]
        [ValidateAntiForgeryToken]
      
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Za validan unos podataka, ubacivanje korisnika u bazu
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                   
                    //Linija za povezivanje korisnika sa njegovom rolom
                    await this.UserManager.AddToRoleAsync(user.Id, model.UserRoles);                     
                    return RedirectToAction("Index", "Korisnici");
                }
                ViewBag.Name =  new SelectList(context.Roles.ToList(), "Name", "Name");

                
                AddErrors(result);
            } 
            return View(model);
        }





        //Metoda za prihvatanje podataka za izmenu podataka o korisniku, dostupno samo adminu
        [Authorize(Roles = RoleName.Admin)]
        public ActionResult EditRegister(string id)
        {

            //Pronalazenje konkretnog korisnika na osnovu id-a
            ApplicationUser applicationUser = context.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }

            ViewBag.Name = new SelectList(context.Roles.ToList(), "Name", "Name");

            //Izvlacenje podataka za generisanje View-a za pregled konkretnog korisnika
            var oldUser = UserManager.FindById(applicationUser.Id);
            var oldRoleId = oldUser.Roles.SingleOrDefault().RoleId;
            var oldRoleName = context.Roles.SingleOrDefault(r => r.Id == oldRoleId).Name;

        

            var rezultat2 = context.Users.Where(n => n.Id == id).Select(n => n.UserName).FirstOrDefault();
            var rezultat3 = context.Users.Where(n => n.Id == id).Select(n => n.Email).FirstOrDefault();
            var rezultat4 = context.Users.Where(n => n.Id == id).Select(n => n.PasswordHash).FirstOrDefault();
       
            ViewBag.UserName = rezultat2;
            ViewBag.Email = rezultat3;
            ViewBag.Password = rezultat4;
            List<string> listaRola = new List<string>() { "Admin", "RWE", "RE" };
            List<string> lista = new List<string>();
            lista.Add(oldRoleName);

            for (int i = 0; i < 3; i++)
            {
                if (lista[0].ToString() == listaRola[i].ToString())
                    continue;
                else
                    lista.Add(listaRola[i]);
            }
            //Lista rola
            ViewBag.Name = new SelectList(lista);
            return View(applicationUser);
        }

        
        //Metoda za izmenu podataka o korisniku
        [HttpPost]        
        [ValidateAntiForgeryToken]
        public  ActionResult  EditRegister([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] ApplicationUser applicationUser,FormCollection frm)
        {
            //dodatna provera za izmenu podataka o korisniku i  handlovanje mogucih nepredvidjenih akcija
            try
            {
                var addr = new System.Net.Mail.MailAddress(applicationUser.Email);

                if (applicationUser.Email.Length > 30)
                    ModelState.AddModelError("Email", "Mejl mora biti kratak");

                if (applicationUser.UserName.Length > 20)
                    ModelState.AddModelError("Username", "Korisničko ime mora biti duže od 20 karaktera.");

                if (ModelState.IsValid)
                {

                    applicationUser.SecurityStamp = Guid.NewGuid().ToString();

                    var oldUser = UserManager.FindById(applicationUser.Id);
                    var oldRoleId = oldUser.Roles.SingleOrDefault().RoleId;
                    var oldRoleName = context.Roles.SingleOrDefault(r => r.Id == oldRoleId).Name;

                    try
                    {
                        UserManager.RemoveFromRole(applicationUser.Id, oldRoleName);
                        UserManager.AddToRole(applicationUser.Id, frm[5]);
                    }
                    catch
                    {
                        UserManager.AddToRole(applicationUser.Id, oldRoleName);
                    }
                    context.Entry(applicationUser).State = EntityState.Modified;
                    context.SaveChanges();
                    return RedirectToAction("Index", "Korisnici");
                }
                //U slucaju greske vrati se na ponovnu izmenu
                return RedirectToAction("EditRegister", "Account", applicationUser.Id);
            }
            catch
            {
                return RedirectToAction("EditRegister", "Account", applicationUser.Id);
            }          
        
        }



        //Metoda za prihvatanje podataka o novoj lozinki, dostupno samo adminu
        [Authorize(Roles = RoleName.Admin)]
        public ViewResult ResetUserPassword(string userId, string UserName)
        {
            ViewBag.Username = UserName.ToString();
            ViewBag.UserId = userId.Trim().ToString();
            return View();
        }


        //Metoda koja cuva izmene o lozinki
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetUserPassword(ResetUserPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());
                if (userManager.HasPassword(model.UserId))
                {
                    
                        userManager.RemovePassword(model.UserId);
                        userManager.AddPassword(model.UserId, model.Password);
                }
                return RedirectToAction("Index","Korisnici");
            }         
            return RedirectToAction("Index", "Korisnici");
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        //Metode ispod sluze za pomoc u radu sa sesijama, identity tehnologija
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}