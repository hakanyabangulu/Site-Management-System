using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Site.Models;

namespace Site.Controllers
{
    public class DaireController : Controller
    {
        
        private site_dbEntities4 db = new site_dbEntities4();

        
        public ActionResult Daire()
        {
            ViewBag.ShowNavbarLinks = true;
           
            List<Daire> daireler = db.Daire.SqlQuery("Select * from Daire").ToList();
            return View(daireler);
        }


        public ActionResult Details(int id)
        {
            ViewBag.ShowNavbarLinks = true;
            Daire daire = db.Database.SqlQuery<Daire>("SELECT * FROM Daire WHERE DaireID = @DaireID", new SqlParameter("@DaireID", id)).FirstOrDefault();

            if (daire == null)
            {
                return HttpNotFound();
            }

            Doluluk doluluk = db.Database.SqlQuery<Doluluk>("SELECT * FROM Doluluk WHERE DaireID = @DaireID", new SqlParameter("@DaireID", id)).FirstOrDefault();

            ViewBag.DolulukBilgisi = doluluk != null ? ((bool)doluluk.DoluMu ? "Dolu" : "Boş") : "Doluluk bilgisi bulunamadı.";

            return View(daire);
        }
        public ActionResult EditDoluluk(int id)
        {
            ViewBag.ShowNavbarLinks = true;

            Doluluk doluluk = db.Database.SqlQuery<Doluluk>("SELECT * FROM Doluluk WHERE DaireID = @DaireID", new SqlParameter("@DaireID", id)).FirstOrDefault();

            if (doluluk == null)
            {
                return HttpNotFound();
            }

            return View(doluluk);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDoluluk(Doluluk doluluk)
        {
            ViewBag.ShowNavbarLinks = true;

            if (ModelState.IsValid)
            {
                string query = @"
            UPDATE Doluluk
            SET DoluMu = @DoluMu
            WHERE DaireID = @DaireID";

                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@DoluMu", doluluk.DoluMu),
            new SqlParameter("@DaireID", doluluk.DaireID)
                };

                db.Database.ExecuteSqlCommand(query, parameters);

                return RedirectToAction("Details", new { id = doluluk.DaireID });
            }

            return View(doluluk);
        }



        public ActionResult Bilgiler()
        {
            ViewBag.ShowNavbarLinks = true;
            List<Mülkiyet> mülkiyetler = db.Mülkiyet.SqlQuery("Select * from Mülkiyet").ToList();

            return View(mülkiyetler);
        }

        [HttpGet]
        public ActionResult Update(int id)
        {
            ViewBag.ShowNavbarLinks = true;

            var daireToUpdate = db.Database.SqlQuery<Mülkiyet>("SELECT * FROM Mülkiyet WHERE DaireID = @DaireID", new SqlParameter("@DaireID", id)).FirstOrDefault();

            if (daireToUpdate == null)
            {
                return HttpNotFound();
            }

            return View(daireToUpdate);
        }


        [HttpPost]
        public ActionResult Update(Mülkiyet updatedDaire)
        {
            ViewBag.ShowNavbarLinks = true;
            if (ModelState.IsValid)
            {
                string query = @"
        UPDATE Mülkiyet
        SET AdiSoyadi = @p0,
            Telefon = @p1,
            DaireTipi = @p2,
            KisiSayisi = @p3
        WHERE DaireID = @p4";

                db.Database.ExecuteSqlCommand(
                    query,
                    new SqlParameter("@p0", (object)updatedDaire.AdiSoyadi ?? DBNull.Value),
                    new SqlParameter("@p1", (object)updatedDaire.Telefon ?? DBNull.Value),
                    new SqlParameter("@p2", (object)updatedDaire.DaireTipi ?? DBNull.Value),
                    new SqlParameter("@p3", (object)updatedDaire.KisiSayisi ?? DBNull.Value),
                    new SqlParameter("@p4", updatedDaire.DaireID)
                );

                return RedirectToAction("Bilgiler");
            }
            return View(updatedDaire);
        }
        public ActionResult Aidat(int id)
        {
            ViewBag.ShowNavbarLinks = true;
            List<Aidat> aidatlar = db.Database.SqlQuery<Aidat>("SELECT * FROM Aidat WHERE DaireID = @p1 Order by AidatAyi", new SqlParameter("@p1", id)).ToList();
            return View(aidatlar);
        }
        public ActionResult Fatura(int id)
        {
            ViewBag.ShowNavbarLinks = true;
            var fatura = db.Fatura.SqlQuery("Select * From Fatura Where DaireID = @p1 ORDER by FaturaTipi", new SqlParameter("@p1", id)).ToList();
            return View(fatura);
        }
        public ActionResult Otopark(int id)
        {
            ViewBag.ShowNavbarLinks = true;
            var otopark = db.Database.SqlQuery<Otopark>("SELECT * FROM Otopark WHERE DaireID = @p1", new SqlParameter("@p1", id)).ToList();
            return View(otopark);
        }











    }
}
