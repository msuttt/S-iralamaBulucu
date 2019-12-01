using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models.Entities;
using HtmlAgilityPack;
using System.Net;
using System.Text;

namespace WebApplication4.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            List<Product> ProductList;
            using(var db=new SortingFinderContext())
            {
                ProductList = db.Products.ToList();
            }
            return View(ProductList);
        }
        public Uri Uri;
        string getHtml;
        public ActionResult CreateToProduct(Product product)
        {
            string getUrl=product.Url;

            char[] array = new char[getUrl.Length];
            string pCode=string.Empty;

            string Xpath="//*[@id = 'contentProDetail'] / div / div[3] / div[2] / div[1] / div / h1";

              for(int i = 0; i <getUrl.Length; i++)
            {
                array[i] = getUrl[i];          
            }

              for(int j = 0; j <array.Length; j++)
            {
                if (j >= array.Length - 9)
               {
                    if (array[j].ToString()=="P")
                    {
                        pCode+= " ";
                    }
                    pCode+= array[j];
               }
            }

            //*[@id="contentMain"]/div/section[2]/ul/li[6]/a/h4  anasayfadan ürün ismi alma          
            //*[@id="contentProDetail"]/div/div[3]/div[2]/div[1]/div/h1  ürün sayfasından ismi alma
            //https://urun.n11.com/elektrikli-motosiklet/supersoco-tc-elektrikli-motosiklet-P299678081

            try
            {
                Uri = new Uri(getUrl);
            }
            catch(UriFormatException)
            {
                TempData["format"] = "Format yanlış";
            }
            catch (ArgumentNullException)
            {
                TempData["boş"] = "Argüman hoş geldi";
            }

            WebClient webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;

            try
            {
                getHtml = webClient.DownloadString(Uri);
            }
            catch(WebException)
            {
                TempData["webHata"] = "WebException hatası";
            }

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(getHtml);


          
            using (var db = new SortingFinderContext())
            {
                product.ProductName = doc.DocumentNode.SelectSingleNode(Xpath).InnerText;
                product.Code = pCode;
                db.Products.Add(product);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

      
        [HttpGet]
        public ActionResult CreateToWord(Word word)
        {

            using (var db = new SortingFinderContext())
            {
                var product = db.Products.ToList();

                ViewBag.AllProducts = product;
                ViewBag.productSelectList = new SelectList(product, "Id", "ProductName");
            }
           
            return View();
        }
        [HttpPost]
        public ActionResult CreateToWord(Word word, int productId, WordControl control)
        {
           
            
            int sayfaNumarasi = 0;
            int sayfadakiSira = 0;
            int genelSiralama = 0;

            string urunlink = string.Empty;
            using (var dbc=new SortingFinderContext())
            {
                urunlink = dbc.Products.SingleOrDefault(m => m.Id == productId).Url;
            }

            HtmlAgilityPack.HtmlDocument doc = new HtmlWeb().Load(urunlink);
            string categoryLinkOfProduct = doc.DocumentNode.SelectSingleNode("//*[@id='breadCrumb']/ul/li[3]/a").GetAttributeValue("href",string.Empty);

            HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlWeb().Load(categoryLinkOfProduct);

         
            string[] pages = new string[10];

           for(int i = 1; i < 11; i++)
            {
                try
                {
                    pages[i - 1] = htmlDocument.DocumentNode.SelectSingleNode("//*[@class='pagination']/a[" + i + "]").GetAttributeValue("href", string.Empty);

                }
                catch(NullReferenceException)
                {
                    continue;
                }

            }
         
            string klm=string.Empty;
            HtmlAgilityPack.HtmlDocument sayfaNumarasıLink;
            for (int x = 1; x <= pages.Length; x++)
            {

                try
                {
                     sayfaNumarasıLink = new HtmlWeb().Load(pages[x - 1]);

                }
                catch(ArgumentNullException)
                {
                    continue;
                }
              

                for (int li = 1; li <= 28; li++)
                {
                    
                    try
                    {
                     
                        klm = sayfaNumarasıLink.DocumentNode.SelectSingleNode("//*[@id='view']/ul/li["+li+"]/div/div[1]/a/h3").InnerText;
                       

                    }
                    catch(NullReferenceException)
                    {
                        continue;
                    }


                    using (var con = new SortingFinderContext())
                    {
                        
                        control.WordComeFromSite = klm;
                        con.WordControls.Add(control);
                        con.SaveChanges();
                    }

                    bool VarMi;
                    using (var connec=new SortingFinderContext())
                    {
                         VarMi = connec.WordControls.Any(m=>m.WordComeFromSite.Contains(word.TheWord));
                    }

                    if (VarMi)
                    {
                        sayfaNumarasi = x;
                        sayfadakiSira = li;
                        genelSiralama = ((x - 1) * 28) + li;


                        using (var dbCon = new SortingFinderContext())
                        {
                            var wordEntry = dbCon.Entry(control);
                            wordEntry.State = EntityState.Deleted;
                            dbCon.SaveChanges();
                        }

                        goto atla;
                    }

                    using (var dbCon = new SortingFinderContext())
                    {
                        var wordEntry = dbCon.Entry(control);
                        wordEntry.State = EntityState.Deleted;
                        dbCon.SaveChanges();
                    }

                    //    char[] sitedenGelenKelime = new char[klm.Length];
                    //        char[] KullanıcınınGirdigi = new char[word.TheWord.Length];

                    //        for (int c = 0; c < sitedenGelenKelime.Length; c++)
                    //        {
                    //            sitedenGelenKelime[c] = klm[c];
                    //        }
                    //        for (int d = 0; d < KullanıcınınGirdigi.Length; d++)
                    //        {
                    //            KullanıcınınGirdigi[d] = word.TheWord[d];
                    //        }

                    //char[] kelime = new char[KullanıcınınGirdigi.Length];
                    //for (int e = 0; e < sitedenGelenKelime.Length; e = e + KullanıcınınGirdigi.Length)
                    //{
                    //    for (int f = 0; f < KullanıcınınGirdigi.Length; f++)
                    //    {
                    //        if (KullanıcınınGirdigi[f] == sitedenGelenKelime[e])
                    //        {
                    //            kelime[f] = sitedenGelenKelime[e];
                    //        }

                    //    }
                    //}


                    //int kıyaslamaSayisi=0;
                    //for(int j = 0; j < KullanıcınınGirdigi.Length; j++)
                    //{
                    //   if (kelime[j] == KullanıcınınGirdigi[j])
                    //    {
                    //        kıyaslamaSayisi++;
                    //    }
                    //}
                    //            if (kıyaslamaSayisi==kelime.Length)
                    //            {
                    //                sayfaNumarasi = x;
                    //                sayfadakiSira = li;
                    //                genelSiralama = ((x - 1) * 28) + li;

                    //                goto atla;

                    //            }

                }

            }


            atla:    using (var db = new SortingFinderContext())
            {
               
                int kelimeSayisi = db.Words.Count();
                       

                if (kelimeSayisi < 50)
                {                                                     
                        word.ProductId = productId;
                        word.AddingTime = DateTime.Now;
                        word.Page = sayfaNumarasi;
                        word.SortOfPage = sayfadakiSira;
                        word.GeneralSort = genelSiralama;                   

                    db.Words.Add(word);                 
                    db.SaveChanges();
                }
                else
                {
                    TempData["kelimeSınırı"] = "50'den fazla kelime giremezsiniz";
                }
               
            }
            return RedirectToAction("Index");
        }

        public ActionResult DeleteFromWord(Word word)
        {
            using(var db=new SortingFinderContext())
            {
                
                var wordEntry=db.Entry(word);
                wordEntry.State = EntityState.Deleted;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
     
        public ActionResult DeleteFromProduct(Product product,int id)
        {
           
            using (var db = new SortingFinderContext())
            {
                List<Word> words= db.Words.Where(m => m.ProductId == id).ToList();
              for(int i = 0; i < words.Count; i++)
                {
                    db.Words.Remove(words[i]);
                }

                var pEntry = db.Entry(product);
                pEntry.State = EntityState.Deleted;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
  
}
