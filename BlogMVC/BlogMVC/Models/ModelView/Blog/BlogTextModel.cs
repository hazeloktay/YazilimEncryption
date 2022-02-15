using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogMVC.Models.ModelView.Blog
{
    public class BlogTextModel
    {
        public List<tblText> texts { get; set; }
        public int thisPage { get; set; }
        public int pageCount { get; set; }

    }
}