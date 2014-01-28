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
        private readonly int MAX_HOURS = 30;
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
                if (task.Resource != null)
                {
                    EstimateTask(task);
                }
                else task.RealHours = task.EstimatedHours;
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
                task.SuggestedResourceName = GetSugestedResourceName(task.AssignedTo.GetValueOrDefault(0),
                    task.EstimatedHours);
            }
            ViewBag.AssignedTo = new SelectList(db.Resources, "ResourceId", "FullName", task.AssignedTo);


            ViewBag.UserStoryId = new SelectList(db.UserStories, "UserStoryId", "Title", task.UserStoryId);
            return View(task);

        }

        private string GetSugestedResourceName(int resourceId, int estimatedHours)
        {

            if (resourceId != 0)
            {
                Resource currentResource = db.Resources.Where(item => item.ResourceId == resourceId).Single();
                List<Resource> resources = db.Resources.Where(item => item.ResourceId != resourceId).ToList();


                List<Resource> availableResources = new List<Resource>();

                foreach (var resource in resources)
                {
                    List<Task> assignedTasks =
                        db.Tasks.Where(item => item.AssignedTo == resource.ResourceId).ToList();
                    int sum = assignedTasks.Sum(item => item.EstimatedHours);

                    if (sum + estimatedHours <= MAX_HOURS)
                        availableResources.Add(resource);
                }


                availableResources.Sort((item1, item2) => item1.ProductivityIndex.CompareTo(item2.ProductivityIndex));

                if (availableResources.Any() &&
                    availableResources.Last().ProductivityIndex > currentResource.ProductivityIndex)
                    return availableResources.Last().FullName;
                else
                    return "No suggestion";
            }
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
                if (task.Resource != null)
                {
                    task.Resource = db.Resources.Where(item => item.ResourceId == task.AssignedTo).FirstOrDefault();
                    EstimateTask(task);
                }
                else
                {
                    task.RealHours = task.EstimatedHours;
                }
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
            if (param.ResourceID != 0)
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
            else return Json(true);
        }


        [HttpGet]
        public ActionResult AutoAssignTasks()
        {
            Dictionary<int, int> Solution;
            using (ScrumContext dbContext = new ScrumContext())
            {
                List<Task> allTasksList = dbContext.Tasks.ToList();
                List<Resource> allResourcesList = dbContext.Resources.ToList();

                if (allTasksList.Sum(item => item.EstimatedHours) > MAX_HOURS * (allResourcesList.Count() + 1))
                    throw new Exception("Auto assigner overflow");


                Solution = new Dictionary<int, int>();
                //add AsssignedTo as 0 for the moment
                foreach (var task in allTasksList)
                {
                    Solution.Add(task.TaskId, 0);
                }

                allTasksList = allTasksList.OrderByDescending(item => item.EstimatedHours).ToList();
                allResourcesList = allResourcesList.OrderByDescending(item => item.ProductivityIndex).ToList();

                foreach (var task in allTasksList)
                {
                    foreach (var resource in allResourcesList)
                    {
                        int assignedHours = GetTotalAssignedHoursForResource(resource.ResourceId, Solution, allTasksList);
                        if (assignedHours + task.EstimatedHours <= MAX_HOURS)
                        {
                            Resource res =
                                allResourcesList.SingleOrDefault(item => item.ResourceId == Solution[task.TaskId]);

                            if (res == null || (res.ProductivityIndex < resource.ProductivityIndex))
                            {
                                Solution[task.TaskId] = resource.ResourceId;
                                break;
                            }

                        }
                    }
                }
            }

            using (ScrumContext dbContext = new ScrumContext())
            {
                int unassigned = Solution.Count(item => item.Value == 0);
                if (unassigned > 0)
                    throw new Exception("Unable to assign all tasks, please assign manually ");
                else
                {
                    foreach (var taskId in Solution.Keys)
                    {
                        Task task = dbContext.Tasks.Find(taskId);
                        task.AssignedTo = Solution[taskId];
                        dbContext.Entry(task).State = EntityState.Modified;
                        dbContext.SaveChanges();

                    }
                }
            }

            return View("Index", db.Tasks.ToList());
        }

        private int GetTotalAssignedHoursForResource(int resourceId, Dictionary<int, int> solution, List<Task> taskList)
        {
            int sum = 0;
            foreach (var key in solution.Keys)
            {
                if (solution[key] == resourceId)
                {
                    Task task = taskList.Single(item => item.TaskId == key);
                    sum += task.EstimatedHours;
                }
            }

            return sum;
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