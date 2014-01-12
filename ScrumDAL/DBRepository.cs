using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrumDAL
{
    public static class DBRepository
    {

        private static ScrumContext _repository=new ScrumContext();
     //---------------------Tasks------------------------------------
 
        public static void SaveTask(Task task)
        {
            _repository.Tasks.Add(task);
            _repository.SaveChanges();
        }

        public static IEnumerable<Task> GetAllTasks()
        {

            return _repository.Tasks.ToList();
        }

    //----------------------Resources--------------------//
        public static IEnumerable<Resource> GetResources()
        {
            return _repository.Resources.ToList();
        }
    }
}
