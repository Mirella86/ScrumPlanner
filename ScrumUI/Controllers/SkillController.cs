using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ScrumUI.Controllers
{
    public class SkillController : Controller
    {
        private ScrumContext db = new ScrumContext();

        //
        // GET: /Skill/

        public ActionResult Index()
        {
            return View(db.Skills.ToList());
        }

        //
        // GET: /Skill/Details/5

        public ActionResult Details(int id = 0)
        {
            Skill skill = db.Skills.Find(id);
            if (skill == null)
            {
                return HttpNotFound();
            }
            return View(skill);
        }

        //
        // GET: /Skill/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Skill/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Skill skill)
        {
            if (ModelState.IsValid)
            {
                db.Skills.Add(skill);
                foreach (var resource in db.Resources)
                {
                    ResourceSkill resourceSkills = new ResourceSkill();
                    resourceSkills.Skill = skill;
                    resourceSkills.Resource = resource;
                    resourceSkills.SkillValue = 0;

                    db.ResourceSkills.Add(resourceSkills);
                }


                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(skill);
        }

        //
        // GET: /Skill/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Skill skill = db.Skills.Find(id);
            if (skill == null)
            {
                return HttpNotFound();
            }
            return View(skill);
        }

        //
        // POST: /Skill/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Skill skill)
        {
            if (ModelState.IsValid)
            {
                db.Entry(skill).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(skill);
        }

        //
        // GET: /Skill/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Skill skill = db.Skills.Find(id);
            if (skill == null)
            {
                return HttpNotFound();
            }
            return View(skill);
        }

        //
        // POST: /Skill/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Skill skill = db.Skills.Find(id);

            //remove related respource skills
            IEnumerable<ResourceSkill> resourceSkillToDelete =
                db.ResourceSkills.Where(rs => rs.SkillID == skill.SkillID).ToList();
            foreach (var resourceSkill in resourceSkillToDelete)
            {
                db.ResourceSkills.Remove(resourceSkill);
            }

            db.Skills.Remove(skill);

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