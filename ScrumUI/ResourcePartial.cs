using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScrumUI
{
    public partial class Resource
    {
        public string FullName { get { return FirstName + " " + LastName; } }
        public int AssignedHours
        {
            get
            {
                using (ScrumContext db = new ScrumContext())
                {
                    List<Task> tasks = db.Tasks.Where(item => item.AssignedTo == this.ResourceId).ToList();
                    int sum = tasks.Sum(item => item.EstimatedHours);
                    //     resource.AssignedHours = sum;
                    return sum;
                }
            }
            set
            {
                AssignedHours = value;
            }
        }
    }
}