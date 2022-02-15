using BlogMVC.Models;
using BlogMVC.Models.ModelView.Blog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlogMVC.Controllers
{
    public class BlogController : Controller
    {
        // GET: Blog
        public ActionResult Index()
        {
            BlogTextModel model = new BlogTextModel();

            using (BlogMvcEntities db = new BlogMvcEntities())
            {
                // Son 10 yazıyı çekiyoruz.
                model.texts = db.tblText.Take(10).ToList();

                foreach (var item in model.texts)
                {
                    //yazıların uzunluğu 300 karakterden fazlaysa sadece 300
                    //karakterini alıyoruz.
                    if (item.Description.Length >= 300)
                    {
                        item.Description = item.Description.Substring(0, 300);
                    }
                }

                model.thisPage = 1;

                //Yazıları 10'ar 10'ar aldığımızda toplam kaç sayfa listeleyeceğimizi
                //hesaplıyoruz. 
                if (db.tblText.ToList().Count() % 10 != 0)
                {
                    model.pageCount = model.texts.Count() / 10 + 1;
                }
                else
                {
                    model.pageCount = model.texts.Count() / 10;
                }
            }
            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Hakkımızda";

            return View();
        }


        public ActionResult Contact()
            {
                ViewBag.Message = "İletişim Sayfası";

                return View();
            }

        public ActionResult Page(int id)
        {
            BlogTextModel model = new BlogTextModel();
            List<tblText> texts = new List<tblText>();
            using (BlogMvcEntities db = new BlogMvcEntities())
            {
                var text = db.tblText.Include("tblCategories").Include("tblAdmins").ToList();

                if (id == 1)
                {
                    if (text.Count() < 10)
                    {
                        for (int i = 0; i < text.Count(); i++)
                        {
                            texts.Add(text[i]);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            texts.Add(text[i]);
                        }
                    }
                }
                else
                {
                    //seçilen sayfadan itibaren kalen yazı sayısı
                    //Örneğin 34 yazı var diyelim biz 2 sayfa geldik 
                    //ilk 10 yazı gösterilmiş olduğu için geriye 24 yazı kalır.
                    //Bu değişken ile o değeri tutuyoruz ve 10'dan büyükse döngüyü 10 kere dönecek şekilde ayarlıyoruz.
                    //Değilse kalan kısım kadar ilerlemesini sağlıyoruz.

                    int pageCount = text.Count() - (id - 1) * 10;
                    int startPage = id * 10 - 10;
                    if (pageCount >= 10)
                    {
                        for (int i = startPage; i < startPage + 10; i++)
                        {
                            texts.Add(text[i]);
                        }
                    }
                    else
                    {
                        for (int i = startPage; i < startPage + pageCount; i++)
                        {
                            texts.Add(text[i]);
                        }
                    }
                }

                model.texts = texts;
                model.thisPage = id;

                if (text.Count() % 10 != 0)
                {
                    model.pageCount = text.Count() / 10 + 1;
                }
                else
                {
                    model.pageCount = text.Count() / 10;
                }
            }
            return View(model);
        }


        public ActionResult Text(int id)
        {
            BlogTextItemModel model = new BlogTextItemModel();
            using (BlogMvcEntities db = new BlogMvcEntities())
            {
                model.text = db.tblText.Include("tblAdmins").Where(t => t.ID == id).FirstOrDefault();

                if (model != null)
                {
                    ViewBag.Error = null;
                    model.commets = db.tblCommet.Where(c => c.TextID == id).ToList();
                    return View(model);
                }
                else
                {
                    ViewBag.Error = "Yazı bulunamadı";
                    return View();
                }
            }
        }

        [HttpPost]
        public JsonResult AddCommet(BlogTextItemModel model)
        {

            using (BlogMvcEntities db = new BlogMvcEntities())
            {
                if (model.commet.Email == null || model.commet.Commet == null || model.commet.Name == null)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    db.tblCommet.Add(model.commet);
                    db.SaveChanges();
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
        }
    }
}