using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ScrumUI.Controllers
{
    public class TaskController : Controller
    {
        private ScrumContext db = new ScrumContext();
        private readonly int MAX_HOURS = 40;
        //
        // GET: /Task/

        public ActionResult Index()
        {
            var tasks = db.Tasks.Include(t => t.Resource).Include(t => t.UserStory);
            return View(tasks.ToList());
        }

        //
        // GET: /Task/Details/5

        public ActionResult Details(int id = 0)
        {
            Task task = db.Tasks.Find(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            return View(task);
        }

        //
        // GET: /Task/Create

        public ActionResult Create()
        {
            ViewBag.AssignedTo = new SelectList(db.Resources, "ResourceId", "FirstName");
            ViewBag.UserStoryId = new SelectList(db.UserStories, "UserStoryId", "Title");
            return View();
        }

        //
        // POST: /Task/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Task task)
        {
            if (ModelState.IsValid)
            {
                EstimateTask(task);

                db.Tasks.Add(task);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AssignedTo = new SelectList(db.Resources, "ResourceId", "FirstName", task.AssignedTo);
            ViewBag.UserStoryId = new SelectList(db.UserStories, "UserStoryId", "Title", task.UserStoryId);
            return View(task);
        }

        //
        // GET: /Task/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Task task = db.Tasks.Find(id);


            if (task == null)
            {
                return HttpNotFound();
            }
            else
            {
                task.SuggestedResourceName = GetSugestedResourceName(task.AssignedTo.GetValueOrDefault(0), task.EstimatedHours);
            }
            ViewBag.AssignedTo = new SelectList(db.Resources, "ResourceId", "FullName", task.AssignedTo);


            ViewBag.UserStoryId = new SelectList(db.UserStories, "UserStoryId", "Title", task.UserStoryId);
            return View(task);
        }

        private string GetSugestedResourceName(int resourceId, int estimatedHours)
        {
            Resource currentResource = db.Resources.Where(item => item.ResourceId == resourceId).Single();
            List<Resource> resources = db.Resources.Where(item => item.ResourceId != resourceId).ToList();


            List<Resource> availableResources = new List<Resource>();

            foreach (var resource in resources)
            {
                List<Task> assignedTasks = db.Tasks.Where(item => item.AssignedTo == resource.ResourceId).ToList();
                int sum = assignedTasks.Sum(item => item.EstimatedHours);

                if (sum + estimatedHours <= MAX_HOURS)
                    availableResources.Add(resource);
            }


            availableResources.Sort((item1, item2) => item1.ProductivityIndex.CompareTo(item2.ProductivityIndex));

            if (availableResources.Last().ProductivityIndex > currentResource.ProductivityIndex)
                return availableResources.Last().FullName;
            else

                return "No suggestion";

        }

        //
        // POST: /Task/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Task task)
        {
            if (ModelState.IsValid)
            {
                task.Resource = db.Resources.Where(item => item.ResourceId == task.AssignedTo).FirstOrDefault();
                EstimateTask(task);
                db.Entry(task).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AssignedTo = new SelectList(db.Resources, "ResourceId", "FirstName", task.AssignedTo);
            ViewBag.UserStoryId = new SelectList(db.UserStories, "UserStoryId", "Title", task.UserStoryId);
            return View(task);
        }

        //
        // GET: /Task/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Task task = db.Tasks.Find(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            return View(task);
        }

        //
        // POST: /Task/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Task task = db.Tasks.Find(id);
            db.Tasks.Remove(task);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        private int EstimateTask(Task task)
        {
            //        CascadeEditHelper rhh = new CascadeEditHelper(task);
            task.RealHours = CascadeEditHelper.RealHoursOfTaskByAssignedResource(task.Resource, task);
            return task.RealHours;
        }
        [HttpPost]
        public JsonResult GetEstimate(TaskParam param)
        {
            int resourceID = param.ResourceID;
            int estimatedHours = param.EstimatedHours;
            using (ScrumContext dbContext = new ScrumContext())
            {
                Resource resource = dbContext.Resources.Where(item => item.ResourceId == resourceID).Single();

                int realHours = CascadeEditHelper.RealHoursOfTaskByAssignedResource(resource,
                    new Task { EstimatedHours = estimatedHours });
                string suggestedName = GetSugestedResourceName(param.ResourceID, estimatedHours);


                return
                    Json(new SuggestedResult { RealHours = realHours, SuggestedName = suggestedName });
                //, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult IsAllowedToAssign(TaskParam param)
        {
            int resourceId = param.ResourceID;
            int estimatedHours = param.EstimatedHours;
            using (ScrumContext dbContext = new ScrumContext())
            {
                Resource resource = dbContext.Resources.Where(item => item.ResourceId == resourceId).Single();
                List<Task> tasks = dbContext.Tasks.Where(item => item.AssignedTo == resourceId).ToList();

                int sum = tasks.Sum(item => item.EstimatedHours);
                if (sum + estimatedHours > MAX_HOURS)
                    return Json(false);
                else return Json(true);
            }
        }
    }

    public class TaskParam
    {
        public int ResourceID { get; set; }
        public int EstimatedHours { get; set; }
    }

    public class SuggestedResult
    {
        public int RealHours { get; set; }
        public string SuggestedName { get; set; }
    }
}