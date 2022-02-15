using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogMVC.Models.ModelView
{
    public class AdminListModel
    {

        public string Name { get; set; }
        public string Password { get; set; }
        public List<tblAdmins> admins { get; set; }
        public tblStatus status { get; set; }
        public tblAdmins admin { get; set; }
    }
}