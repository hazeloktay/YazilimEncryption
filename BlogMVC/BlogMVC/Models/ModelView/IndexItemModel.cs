using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogMVC.Models.ModelView
{
    public class IndexItemModel
    {
        public List<tblText> texts { get; set; }
        public List<tblCommet> commets { get; set; }
    }
}