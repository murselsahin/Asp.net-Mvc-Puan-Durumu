using PuanDurumu.Models;
using PuanDurumu.Models.Managers;
using PuanDurumu.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PuanDurumu.Controllers
{
    public class HomeController : Controller
    {
        private DatabaseContext db = new DatabaseContext();
        // GET: Home
        public ActionResult Index()
        {
            List<Takimlar> listTakimlar = db.Takimlar.ToList();
            List<TakimDurumu> listTakimDurumu = new List<TakimDurumu>();

            foreach (var item in listTakimlar)
            {

                TakimDurumu takimDurumu = new TakimDurumu()
                {
                    Takim_Id = item.Id,
                    TakimAdi = item.Adi,

                };

                AtilanYenilenGol atilanYenilenGol = db.ExecuteGetAtilanYenilenGolSP(item.Id);

                takimDurumu.AtilanGol = atilanYenilenGol.AtilanGol;
                takimDurumu.YenilenGol = atilanYenilenGol.YenilenGol;

                PuanGalibiyetMaglubiyetBeraberlik pgmb = db.ExecuteGetPuanGalibiyetMaglubiyetBeraberlikSP(item.Id);

                takimDurumu.Puan = pgmb.Puan;
                takimDurumu.Galibiyet = pgmb.Galibiyet;
                takimDurumu.Maglubiyet = pgmb.Maglubiyet;
                takimDurumu.Beraberlik = pgmb.Beraberlik;



                listTakimDurumu.Add(takimDurumu);
            }

            IndexViewModel model = new IndexViewModel()
            {
                ListTakimDurumu = listTakimDurumu.OrderByDescending(x => x.Puan).ThenByDescending(x => x.Averaj).ThenByDescending(x => x.AtilanGol).ToList()
            };



            return View(model);
        }

        public ActionResult SkorGir()
        {


            SkorGirViewModel model = new SkorGirViewModel()
            {
                EvSahibi_Id = -1,
                Deplasman_Id = -1,
                TakimlarData = new SelectList(db.Takimlar.OrderBy(x => x.Adi).ToList(), "Id", "Adi")
            };

            if (TempData["Sonuc"] != null)
                ViewBag.Sonuc = TempData["Sonuc"];
            if (TempData["Hata"] != null)
                ViewBag.Hata = TempData["Hata"];

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SkorGir(SkorGirViewModel model)
        {
            model.TakimlarData = new SelectList(db.Takimlar.OrderBy(x => x.Adi).ToList(), "Id", "Adi");


            Takimlar evSahibiTakim = db.Takimlar.Where(x => x.Id == model.EvSahibi_Id).FirstOrDefault();
            Takimlar deplasmanTakim = db.Takimlar.Where(x => x.Id == model.Deplasman_Id).FirstOrDefault();

            if (evSahibiTakim != null && deplasmanTakim != null)
            {
                model.Maclar.DeplasmanTakimi = deplasmanTakim;
                model.Maclar.EvSahibiTakimi = evSahibiTakim;
                db.Maclar.Add(model.Maclar);
                int sonuc = db.SaveChanges();
                if (sonuc > 0)
                    TempData["Sonuc"] = true;
                else
                    TempData["Sonuc"] = false;
            }
            else
            {
                TempData["Hata"] = true;
            }



            return RedirectToAction("SkorGir");


            // return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TakimEkle(SkorGirViewModel model)
        {
            if (model.Takim.Adi != null && model.Takim.Adi != "")
            {
                db.Takimlar.Add(model.Takim);
                db.SaveChanges();
            }


            return RedirectToAction("SkorGir");
        }

        public ActionResult Detay(int? id)
        {
            return View();
        }

        public JsonResult GetDeplasmanTakimi(int id)
        {
            int evSahibi_Id = id;


            List<Takimlar> result = db.ExecuteGetDesplasmanTakimiSP(id);


            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetTakimDetay(int id)
        {
            int takimId = id;

            List<Maclar> listMaclar = db.Maclar.Where(x => x.EvSahibiTakimi.Id == id || x.DeplasmanTakimi.Id == id).OrderBy(x => x.Tarih).ToList();

            List<TakimDetay> result = new List<TakimDetay>();

            foreach (var item in listMaclar)
            {
                TakimDetay takimDetay = new TakimDetay()
                {
                    Tarih = item.Tarih.ToShortDateString(),
                    Sonuc = item.EvSahibiTakimi.Adi + " " + item.EvSkor + " - " + item.DeplasmanSkor + " " + item.DeplasmanTakimi.Adi
                };
                result.Add(takimDetay);

            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }

}
