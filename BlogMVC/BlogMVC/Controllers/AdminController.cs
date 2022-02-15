using BlogMVC.Models;
using BlogMVC.Models.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BlogMVC.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            IndexItemModel model = new IndexItemModel();

            using (BlogMvcEntities db = new BlogMvcEntities())
            {
                model.texts = db.tblText.Include("tblAdmins").Include("tblCategories").Take(5).ToList();
                model.commets = db.tblCommet.Include("tblText").Take(5).ToList();
            }

            return View(model);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string pass)
        {
            using (BlogMvcEntities db = new BlogMvcEntities())
            {
                var admin = db.tblAdmins.Where(a => a.Username == username && a.Password == pass).FirstOrDefault();

                if (admin != null)
                {
                    Session["Username"] = username;
                    return RedirectToAction("Index", "Admin");

                }
                else
                {
                    ViewBag.Error = "Kullanıcı adı veya şifre hatalı.";
                    return View();
                }

            }


        }
        public ActionResult AdminAdd()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AdminAdd(string name, string username, string password, int status)
        {
            using (BlogMvcEntities db = new BlogMvcEntities())
            {
                tblAdmins adminInfo = new tblAdmins();

                //Kullanıcı adı ile daha önce admin oluşturulmuş mu kontrol ediyoruz.
                // İlk bulunan değer döndürülüyor. (FirstOrDefault() metoduyla) 
                //Where ile sorgu işlemini gerçekleştiriyoruz.

                tblAdmins isAdmin = db.tblAdmins.Where(a => a.Username == username).FirstOrDefault();

                //Eğer kullanıcı adı yoksa yeni bir kullanıcı oluşturuyoruz.
                // oluşturmuş olduğum adminInfo nesnesine eklenen değerleri atıyoruz.
                if (isAdmin == null)
                {
                    adminInfo.Name = name;
                    adminInfo.Username = username;
                    adminInfo.Password = password;
                    adminInfo.statusID = status;

                    //adminInfo nesnesini tblAdmins tablosuna yeni kullanıcı olarak ekliyoruz.
                    db.tblAdmins.Add(adminInfo);
                    //verinin tabloya kaydedilmesi için gereklidir. Yoksa veri tabloya kaydedilmez.
                    db.SaveChanges();

                    ViewBag.Success = "Kayıt işlemi başarılı. :)";
                }
                else
                {
                    ViewBag.Error = "Kullanıcı daha önce oluşturulmuş.";
                }



                return View();
            }
        }
        public ActionResult AdminList()
        {
            AdminListModel model = new AdminListModel();
            using (BlogMvcEntities db = new BlogMvcEntities())
            {
                //Çekilen verileri ToList() metoduyla List'e dönüştürdük.
                model.admins = db.tblAdmins.Include("tblStatus").ToList();
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult AdminRemove(int id)
        {

            var resultJson = "";
            using (BlogMvcEntities db = new BlogMvcEntities())
            {
                string username = Session["Username"].ToString();

                var admin = db.tblAdmins.Where(a => a.ID == id).FirstOrDefault();
                //login kontrollü yapıyoruz. Eğer giriş yapılmışsa giriş yapan kişi admin mi bunun kontrolünü yapıyoruz.
                if (username != null)
                {
                    var isAdmin = db.tblAdmins
                        .Include("tblStatus")
                        .Where(a => a.Username == username).FirstOrDefault();
                    if (isAdmin.tblStatus.Name != "Admin")
                    {
                        resultJson = "Yetkiniz bulunmamaktadır";
                        return Json(resultJson, JsonRequestBehavior.AllowGet);
                    }

                }
                // Giriş yapılmamışsa login'e yönlendiriyoruz.
                if (username == null)
                {
                    resultJson = "Lütfen giriş yapın... /n Yönlendiriliyorsunuz...";
                    return Json(resultJson, JsonRequestBehavior.AllowGet);
                }

                // Silinecek kişi admin mi kontrol ediyoruz. Eğer ki admin ise silme işlemini gerçekleştirmiyoruz.
                if (admin.statusID == 3)
                {
                    resultJson = "Bu kişi silinemez";
                    return Json(resultJson, JsonRequestBehavior.AllowGet);

                }
                // Ve tüm bu koşulları sağlıyorsa silme işlemini gerçekleştiriyoruz.
                else
                {
                    db.tblAdmins.Remove(admin);
                    db.SaveChanges();
                    resultJson = "Silme işlemi başarılı";
                    return Json(resultJson, JsonRequestBehavior.AllowGet);
                }
            }


        }

        [HttpPost]
        public JsonResult AdminEdit(AdminEditModel model)
        {
            using (BlogMvcEntities db = new BlogMvcEntities())
            {
                string username = Session["Username"].ToString();
                var isAdmin = db.tblAdmins.Include("tblStatus").Where(a => a.Username == username).FirstOrDefault();
                if (isAdmin.tblStatus.Name != "admin")
                {
                    return Json("Yetkiniz bulunmuyor.", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var user = db.tblAdmins.Where(a => a.ID == model.id).FirstOrDefault();
                    user.statusID = model.statu + 1;

                    // Add methodu kullanmıyoruz. Oluşturduğumuz nesnede ki değeri değiştirip ardından database'i 
                    //kaydetmemiz yeterli. :) Bu da EF'un bize sunduğu güzellik. :)
                    db.SaveChanges();

                    //İlk parametre ile objede gönderebilirsiniz.
                    return Json("İşlem başarılı.", JsonRequestBehavior.AllowGet);


                }
            }
        }
        private string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes
             = ASCIIEncoding.ASCII.GetBytes(toEncode);
            string returnValue
                  = Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }
        public ActionResult TextAdd()
        {
            //Burada model göndermemizin amacı kategori dropdown'ını doldurmak. 
            //Database'de kayıtlı kategorileri çekiyoruz. 
            using (BlogMvcEntities db = new BlogMvcEntities())
            {
                TextAddModel model = new TextAddModel();
                model.categori = db.tblCategories.ToList();

                return View(model);
            }
        }

        [HttpPost]
        public JsonResult TextAdd(TextAddModel model)
        {

            using (BlogMvcEntities db = new BlogMvcEntities())
            {

                tblText text = new tblText();
                tblAdmins admin = new tblAdmins();
                string username = Session["Username"].ToString();
                string data = "";

                //Giriş yapan kullanıcının bilgilerini çekiyoruz.
                admin = db.tblAdmins.Where(a => a.Username == username).FirstOrDefault();

                //text.Title = model.title;
                text.CategoryID = model.kategori;
                text.AdminID = admin.ID;
               text.Description = EncodeTo64(model.text);
                text.Title = EncodeTo64(model.title);



                //Yazıyı database'e ekliyoruz.
                db.tblText.Add(text);
                db.SaveChanges();

                data = "İşlem başarılı";

                return Json(data, JsonRequestBehavior.AllowGet);
            }

        }



        static public string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes
                = Convert.FromBase64String(encodedData);
            string returnValue =
               ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
            return returnValue;
        }
        public ActionResult TextList(int id)
        {
            TextListModel model = new TextListModel();
            List<tblText> texts = new List<tblText>();
            using (BlogMvcEntities db = new BlogMvcEntities())
            {
                var text = db.tblText.Include("tblAdmins").Include("tblCategories").ToList();

                if (id == 1)
                {
                    //Eğer ilk 10 yazıyı çekeceksem  karmaşıklık oluşturmadan ilk 10 değeri çekiyorum.
                    //Toplam yazı sayısı 10'dan az mı diye kontrol ediyorum
                    //Eğer az ise tüm yazıları çekiyorum.
                    if (text.Count() < 10)
                    {
                        for (int i = 0; i < text.Count(); i++)
                        {
                            texts.Add(text[i]);
                        }
                    }
                    //Eğer ki 10'dan fazla değer varsa sadece ilk 10 değeri alıyorum.
                    else
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            texts.Add(text[i]);
                        }
                    }

                }
                //Gelen id değeri 1 değilse aşağıdaki kod çalışacak.
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
                string Title = string.Empty;
                List<tblText> newText = new List<tblText>();
                foreach (var itemText in texts)
                {
                    model.text = DecodeFrom64(itemText.Title);
                    model.text = DecodeFrom64(itemText.Description);
                    newText.Add(new tblText()
                    {
                        Title = model.text,
                        Description = model.text
                    });
                }


                model.texts = newText;

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

        [HttpPost]
        public JsonResult RemoveText(int id)
        {
            var data = false;
            var username = "";
            username = Session["Username"].ToString();

            using (BlogMvcEntities db = new BlogMvcEntities())
            {
                var isAdmin = db.tblAdmins
                                        .Include("tblStatus")
                                        .Where(a => a.Username == username).FirstOrDefault();
                var removeText = db.tblText.Include("tblAdmins").Where(t => t.ID == id).FirstOrDefault();


                if (isAdmin.tblStatus.Name == "Admin")
                {
                    db.tblText.Remove(removeText);
                    db.SaveChanges();

                    data = true;
                }
                else
                {
                    data = false;
                }

            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TextEdit(int id)
        {
            using (BlogMvcEntities db = new BlogMvcEntities())
            {
                TextEditModel model = new TextEditModel();
                model.text = db.tblText.Where(t => t.ID == id).FirstOrDefault();

                return View(model);
            }
        }

        [HttpPost]
        public ActionResult TextUpdate(TextEditModel model)
        {
            using (BlogMvcEntities db = new BlogMvcEntities())
            {
                model.text = db.tblText.Where(t => t.ID == model.id).FirstOrDefault();
                model.text.Title =EncodeTo64(model.title);
                model.text.Description = EncodeTo64(model.description);

                db.SaveChanges();

                return Json("data", JsonRequestBehavior.AllowGet);
            }
        }

    }
}