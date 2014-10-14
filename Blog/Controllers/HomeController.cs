using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blog.Controllers
{
    public class HomeController : Controller
    {
        // GET: Accueil
        public ActionResult Index()
        {
            bool afficheBandeau = true;
            if (Request.Cookies["autoriser_analyse"] != null)
            {
                afficheBandeau = Request.Cookies.Get("autoriser_analyse").Value == "vrai" ? false : true;
            }
            ViewBag.DisplayCookie = afficheBandeau;

            return View(); 
        }

        //
        // GET: /About/
        public ActionResult About()
        {
            return View();
        }

        //
        // GET : /Cookies?accept=1
        public ActionResult Cookies(int accept)
        {
            string autorizeCookies = "faux";

            //Accepte le cookie
            if (accept == 1)
            {
                autorizeCookies = "vrai";
            }
            
            Response.Cookies.Set(new HttpCookie("autoriser_analyse", autorizeCookies) { Expires = DateTime.MaxValue });

            return View("Index");
        }
    }
}