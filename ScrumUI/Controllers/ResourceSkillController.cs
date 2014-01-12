using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ScrumUI.Controllers
{
    public class ResourceSkillController : Controller
    {
        private ScrumContext db = new ScrumContext();

        //
        // GET: /ResourceSkill/

        public ActionResult Index()
        {
            var resourceskills = db.ResourceSkills.Include(r => r.Resource).Include(r => r.Skill);
            return View(resourceskills.ToList());
        }

        //
        // GET: /ResourceSkill/Details/5

        public ActionResult Details(int id = 0)
        {
            ResourceSkill resourceskill = db.ResourceSkills.Find(id);
            if (resourceskill == null)
            {
                return HttpNotFound();
            }
            return View(resourceskill);
        }

        //
        // GET: /ResourceSkill/Create

        public ActionResult Create()
        {
            ViewBag.ResourceID = new SelectList(db.Resources, "ResourceId", "FirstName");
            ViewBag.SkillID = new SelectList(db.Skills, "SkillID", "SkillName");
            return View();
        }

        //
        // POST: /ResourceSkill/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ResourceSkill resourceskill)
        {
            if (ModelState.IsValid)
            {
                db.ResourceSkills.Add(resourceskill);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ResourceID = new SelectList(db.Resources, "ResourceId", "FirstName", resourceskill.ResourceID);
            ViewBag.SkillID = new SelectList(db.Skills, "SkillID", "SkillName", resourceskill.SkillID);
            return View(resourceskill);
        }

        //
        // GET: /ResourceSkill/Edit/5

        public ActionResult Edit(int id = 0)
        {
            ResourceSkill resourceskill = db.ResourceSkills.Find(id);
            if (resourceskill == null)
            {
                return HttpNotFound();
            }
            ViewBag.ResourceID = new SelectList(db.Resources, "ResourceId", "FirstName", resourceskill.ResourceID);
            ViewBag.SkillID = new SelectList(db.Skills, "SkillID", "SkillName", resourceskill.SkillID);
            return View(resourceskill);
        }

        //
        // POST: /ResourceSkill/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ResourceSkill resourceskill)
        {
            if (ModelState.IsValid)
            {
                var skillValue = resourceskill.SkillValue;

                resourceskill.SkillValue = skillValue;
                db.Entry(resourceskill).State = EntityState.Modified;
                db.SaveChanges();
                CascadeEditHelper.CascadeEditProductivityPercentAndRealHours(resourceskill.ResourceID);

                return RedirectToAction("Index", "Resource", new { area = "", id = resourceskill.ResourceID });
            }
            ViewBag.ResourceID = new SelectList(db.Resources, "ResourceId", "FirstName", resourceskill.ResourceID);
            ViewBag.SkillID = new SelectList(db.Skills, "SkillID", "SkillName", resourceskill.SkillID);

            return RedirectToAction("Index", "Resource", new { area = "", id = resourceskill.ResourceID}); 
        }

        //
        // GET: /ResourceSkill/Delete/5

        public ActionResult Delete(int id = 0)
        {
            ResourceSkill resourceskill = db.ResourceSkills.Find(id);
            if (resourceskill == null)
            {
                return HttpNotFound();
            }
            return View(resourceskill);
        }

        //
        // POST: /ResourceSkill/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ResourceSkill resourceskill = db.ResourceSkills.Find(id);
            db.ResourceSkills.Remove(resourceskill);
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