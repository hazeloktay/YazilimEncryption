using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlogMVC.Models.ModelView
{
    public class TextAddModel
    {
        public string title { get; set; }
        public string text { get; set; }
        public int kategori { get; set; }
        // İlk açılışta dropdown'a kategorileri eklemek için List oluşturuyoruz.
        public List<tblCategories> categori { get; set; }
        //Yukarıda ki list'i DropDown'da kullanabilmek için IEnumerable türünde
        // bir SelectedListItem oluşturuyoruz. 
        public IEnumerable<SelectListItem> CategoriItems
        {
            get { return new SelectList(categori, "ID", "name"); }
        }
    }
}