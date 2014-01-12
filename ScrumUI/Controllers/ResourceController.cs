using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ScrumUI.Controllers
{
    public class ResourceController : Controller
    {
        private ScrumContext db = new ScrumContext();

        //
        // GET: /Resource/

        public ActionResult Index()
        {
            return View(db.Resources.ToList());
        }

        //
        // GET: /Resource/Details/5

        public ActionResult Details(int id = 0)
        {
            Resource resource = db.Resources.Find(id);
            if (resource == null)
            {
                return HttpNotFound();
            }
            return View(resource);
        }

        //
        // GET: /Resource/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Resource/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Resource resource)
        {
            if (ModelState.IsValid)
            {
                db.Resources.Add(resource);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(resource);
        }

        //
        // GET: /Resource/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Resource resource = db.Resources.Find(id);
            if (resource == null)
            {
                return HttpNotFound();
            }
            return View(resource);
        }

        //
        // POST: /Resource/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Resource resource)
        {
            if (ModelState.IsValid)
            {
                resource.ResourceSkills =
                    db.ResourceSkills.Where(item => item.ResourceID == resource.ResourceId).ToList();
           //     CascadeEditHelper rhh = new CascadeEditHelper(resource);
                resource.ProductivityIndex = CascadeEditHelper.ResourceProductivityPercent(resource);

                db.Entry(resource).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(resource);
        }

        //
        // GET: /Resource/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Resource resource = db.Resources.Find(id);
            if (resource == null)
            {
                return HttpNotFound();
            }
            return View(resource);
        }

        //
        // POST: /Resource/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Resource resource = db.Resources.Find(id);
            db.Resources.Remove(resource);
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