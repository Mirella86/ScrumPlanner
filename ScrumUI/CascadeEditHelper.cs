using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ScrumUI
{
    public static class CascadeEditHelper
    {

        public static decimal ResourceProductivityPercent(Resource resource)
        {
            if (resource.ResourceSkills != null)
            {
                int sumOfValues = 0;
                int totalSum = 0;
                foreach (var resourceSkill in resource.ResourceSkills)
                {
                    if (resourceSkill.SkillValue != 0)
                    {
                        sumOfValues += resourceSkill.SkillValue * resourceSkill.Skill.SkillWeight;
                        totalSum += resourceSkill.Skill.SkillWeight * 5; //5 is max value;
                    }
                }

                using (ScrumContext dbContext = new ScrumContext())
                {
                    //     var resource = dbContext.Resources.Find(Resource.ResourceId);
                    resource.ProductivityIndex = Math.Round((decimal)sumOfValues * 100 / totalSum, 3);
                    dbContext.SaveChanges();
                }
                return resource.ProductivityIndex;
            }
            return 0;
        }

        public static int RealHoursOfTaskByAssignedResource(Resource resource, Task task)
        {
            return Convert.ToInt32(task.EstimatedHours + task.EstimatedHours * (1 - ResourceProductivityPercent(resource) / 100));
        }

        //when modifying Resource Skills value
        public static void CascadeEditProductivityPercentAndRealHours(int resourceId)
        {
            using (ScrumContext dbContext = new ScrumContext())
            {
                Resource resource = dbContext.Resources.Where(item => item.ResourceId == resourceId).Single();
                resource.ResourceSkills = dbContext.ResourceSkills.Where(item => item.ResourceID == resource.ResourceId).ToList();
                resource.ProductivityIndex = ResourceProductivityPercent(resource);

                IEnumerable<Task> resourceTasks = dbContext.Tasks.Where(item => item.Resource.ResourceId == resource.ResourceId).ToList();

                foreach (var task in resourceTasks)
                {
                    task.RealHours = RealHoursOfTaskByAssignedResource(resource, task);

                }
                dbContext.SaveChanges();
            }
        }
    }
}