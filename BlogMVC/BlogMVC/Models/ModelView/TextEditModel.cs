using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogMVC.Models.ModelView
{
    public class TextEditModel
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public tblText text { get; set; }
    }
}