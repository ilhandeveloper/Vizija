using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TestFinal.Controllers
{
    //Globalna  kontrola za handlovanje gresaka
    public class ErrorController : Controller
    {
        //Zajednicki prikaz ekrana za greske
        public ViewResult NotFound()
        {
            return View();
        }
    }
}