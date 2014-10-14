using Blog.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Slugify;

namespace Blog.Controllers
{
    public class ArticleController : Controller
    {
        /// <summary>
        /// Champ qui va permettre d'appeler des méthodes pour faire des actions sur notre fichier
        /// </summary>
        private readonly ArticleJSONRepository _repository;

        private static string[] AcceptedTypes = new string[] { "image/jpeg", "image/png" };
        private static string[] AcceptedExt = new string[] { "jpeg", "jpg", "png", "gif" };

        public readonly static int ARTICLEPERPAGE = 5;

        /// <summary>
        /// Constructeur par défaut, permet d'initialiser le chemin du fichier JSON
        /// </summary>
        public ArticleController()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "liste_article_tuto_full.json");
            _repository = new ArticleJSONRepository(path);
        }

        // GET: Article
        public ActionResult Index()
        {
            return View();
        }

        #region List sans pagination

        //// GET: List
        //public ActionResult List()
        //{
        //    try
        //    {
        //        List<Article> liste = _repository.GetAllListArticle().ToList();
        //        return View(liste);
        //    }
        //    catch
        //    {
        //        return View(new List<Article>());
        //    }
        //}

        #endregion

        // GET: List
        public ActionResult List(int page = 0)
        {
            try
            {
                List<Article> liste = _repository.GetListArticle(page * ARTICLEPERPAGE, ARTICLEPERPAGE).ToList();
                ViewBag.Page = page;
                return View(liste);
            }
            catch
            {
                return View(new List<Article>());
            }
        }

        //GET : Create
        public ActionResult Create()
        {
            return View();
        }

        //POST : Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ArticleCreation articleCreation)
        {
            if (!ModelState.IsValid)
            {
                return View(articleCreation);
            }

            string fileName = "";
            if (articleCreation.Image != null)
            {
                bool hasError = false;
                if (articleCreation.Image.ContentLength > 1024 * 1024)
                {
                    ModelState.AddModelError("Image", "Le fichier téléchargé est trop grand.");
                    hasError = true;
                }

                if (!AcceptedTypes.Contains(articleCreation.Image.ContentType)
                       || AcceptedExt.Contains(Path.GetExtension(articleCreation.Image.FileName).ToLower()))
                {
                    ModelState.AddModelError("Image", "Le fichier doit être une image.");
                    hasError = true;
                }

                try
                {
                    string fileNameFile = Path.GetFileName(articleCreation.Image.FileName);
                    fileName = new SlugHelper().GenerateSlug(fileNameFile);
                    string imagePath = Path.Combine(Server.MapPath("~/Content/Upload"), fileName);
                    articleCreation.Image.SaveAs(imagePath);
                }
                catch
                {
                    fileName = "";
                    ModelState.AddModelError("Image", "Erreur à l'enregistrement.");
                    hasError = true;
                }
                if (hasError)
                    return View(articleCreation);
            }

            Article article = new Article
            {
                Contenu = articleCreation.Contenu,
                Pseudo = articleCreation.Pseudo,
                Titre = articleCreation.Titre,
                ImageName = fileName
            };

            _repository.AddArticle(article);
            return RedirectToAction("List", "Article");
        }
    }
}