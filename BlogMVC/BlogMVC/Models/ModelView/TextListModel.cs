using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogMVC.Models.ModelView
{
    public class TextListModel
    {
        public string title { get; set; }
        public string text { get; set; }
        public int kategori { get; set; }
        public List<tblText> texts { get; set; }
        public int pageCount { get; set; }
        public int thisPage { get; set; }

    }
}