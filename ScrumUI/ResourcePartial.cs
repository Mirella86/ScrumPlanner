using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScrumUI
{
    public partial class Resource
    {
        public string FullName { get { return FirstName + " " + LastName; } }
    }
}