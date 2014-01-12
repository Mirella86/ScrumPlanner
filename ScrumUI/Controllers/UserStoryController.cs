using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ScrumUI.Controllers
{
    public class UserStoryController : Controller
    {
        private ScrumContext db = new ScrumContext();

        //
        // GET: /UserStory/

        public ActionResult Index()
        {
            return View(db.UserStories.ToList());
        }

        //
        // GET: /UserStory/Details/5

        public ActionResult Details(int id = 0)
        {
            UserStory userstory = db.UserStories.Find(id);
            if (userstory == null)
            {
                return HttpNotFound();
            }
            return View(userstory);
        }

        //
        // GET: /UserStory/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /UserStory/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserStory userstory)
        {
            if (ModelState.IsValid)
            {
                db.UserStories.Add(userstory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(userstory);
        }

        //
        // GET: /UserStory/Edit/5

        public ActionResult Edit(int id = 0)
        {
            UserStory userstory = db.UserStories.Find(id);
            if (userstory == null)
            {
                return HttpNotFound();
            }
            return View(userstory);
        }

        //
        // POST: /UserStory/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserStory userstory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userstory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userstory);
        }
        
        //
        // GET: /UserStory/Delete/5

        public ActionResult Delete(int id = 0)
        {
            UserStory userstory = db.UserStories.Find(id);
            if (userstory == null)
            {
                return HttpNotFound();
            }
            return View(userstory);
        }

        //
        // POST: /UserStory/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserStory userstory = db.UserStories.Find(id);
            db.UserStories.Remove(userstory);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}