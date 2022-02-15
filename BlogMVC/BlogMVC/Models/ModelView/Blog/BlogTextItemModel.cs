using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogMVC.Models.ModelView.Blog
{
    public class BlogTextItemModel
    {
        public tblText text { get; set; }
        public List<tblCommet> commets { get; set; }
        public tblCommet commet { get; set; }
    }
}