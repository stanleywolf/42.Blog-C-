using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using _42.Blog.Models;

namespace _42.Blog.Controllers
{
    public class ArticleController : Controller
    {
        // GET: Article
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            using (var db = new BlogDbContext())
            {
                var articles = db.Articles.Include(a => a.Author).ToList();
                return View(articles);
            }
            
        }

        //Get Article/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (var db = new BlogDbContext())
            {
                var article = db.Articles
                    .Where(i => i.Id == id)
                    .Include(x => x.Author)
                    .First();
                if (article == null)
                {
                    return HttpNotFound();
                }
                return View(article);
            }
        }

        //GET: Article/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        //Post: Article/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Article article)
        {
            if (ModelState.IsValid)
            {
                using (var db = new BlogDbContext())
                {
                    var authorId = db.Users
                        .Where(x => x.UserName == this.User.Identity.Name)
                        .First()
                        .Id;
                    if (authorId == null)
                    {
                        return HttpNotFound();
                    }
                    article.AuthorId = authorId;

                    db.Articles.Add(article);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            return View(article);
        }

        //GET: Article/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (var db = new BlogDbContext())
            {
                var atricle = db.Articles
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .First();

                if (!IsUserAuthorizedToEdit(atricle))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                if (atricle == null)
                {
                    return HttpNotFound();
                }
                return View(atricle);
            }            
        }

        //Post: Article/Delete
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (var db = new BlogDbContext())
            {
                var article = db.Articles
                    .Where(x => x.Id == id)
                    .Include(x => x.Author)
                    .First();
                        
                    if (article == null)
                    {
                        return HttpNotFound();
                    }

                    db.Articles.Remove(article);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                
            }         
        }

        //Get:Article/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (var db = new BlogDbContext())
            {
                var article = db.Articles
                    .Where(x => x.Id == id)
                    .First();
                if (article == null)
                {
                    return HttpNotFound();
                }
                var model = new ArticleViewModel();
                model.Id = article.Id;
                model.Title = article.Title;
                model.Content = article.Content;

                return View(model);
            }
        }

        //Post: Article/Edit
        [HttpPost]
        public ActionResult Edit(ArticleViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new BlogDbContext())
                {
                    var article = db.Articles
                    .FirstOrDefault(a => a.Id == model.Id);

                    article.Title = model.Title;
                    article.Content = model.Content;

                    db.Entry(article).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }


        private bool IsUserAuthorizedToEdit(Article article)
        {
            bool isAdmin = this.User.IsInRole("Admin");
            bool isAuthor = article.IsAuthor(this.User.Identity.Name);

            return isAuthor || isAdmin;
        }
    }
}