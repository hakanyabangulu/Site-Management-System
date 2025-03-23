using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Site.Models;

namespace Site.Controllers
{
    public class AidatController : Controller
    {
        private site_dbEntities4 db = new site_dbEntities4  ();

        
        public ActionResult Aidat()
        {
            ViewBag.ShowNavbarLinks = true;
            List<Aidat> aidatlar = db.Aidat.SqlQuery("SELECT * FROM Aidat ORDER BY DaireID, CAST(SUBSTRING(AidatAyi, 1, 2) AS INT) ASC").ToList();



            return View(aidatlar);
        }


        public ActionResult Update(int id)
        {
            ViewBag.ShowNavbarLinks = true;


            var aidat = db.Database.SqlQuery<Aidat>("SELECT * FROM Aidat WHERE AidatID = @Id", new SqlParameter("@Id", id)).SingleOrDefault();



            if (aidat == null)
            {
                return HttpNotFound();
            }

            return View(aidat);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(Aidat updatedAidat)
        {
            if (ModelState.IsValid)
            {
                var aidat = db.Aidat.Find(updatedAidat.AidatID);
                if (aidat != null)
                {
                    db.Database.ExecuteSqlCommand(
                        "UPDATE Aidat SET AidatAyi = @p0, AidatTutari = @p1, OdemeDurumu = @p2 WHERE AidatID = @p3",
                        updatedAidat.AidatAyi, updatedAidat.AidatTutari, updatedAidat.OdemeDurumu, updatedAidat.AidatID);

                    return RedirectToAction("Aidat");
                }
            }
            return View(updatedAidat);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Aidat aidat, int? daireID)
        {
            if (aidat != null && daireID.HasValue && ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(aidat.AidatAyi) || aidat.AidatTutari == 0)
                {
                    ModelState.AddModelError("", "Aidat ayı ve tutarı boş olamaz.");
                    return View(aidat);
                }

                string sqlQuery = "INSERT INTO Aidat (DaireID, AidatAyi, AidatTutari, OdemeDurumu) " +
                                  "VALUES (@DaireID, @AidatAyi, @AidatTutari, @OdemeDurumu )";

                object odemeDurumuValue = string.IsNullOrEmpty(aidat.OdemeDurumu) ? "Ödenmedi" : (object)aidat.OdemeDurumu;

                db.Database.ExecuteSqlCommand(
                    sqlQuery,
                    new SqlParameter("@DaireID", daireID.Value),
                    new SqlParameter("@AidatAyi", aidat.AidatAyi),
                    new SqlParameter("@AidatTutari", aidat.AidatTutari),
                    new SqlParameter("@OdemeDurumu", odemeDurumuValue)
                );

                return RedirectToAction("Aidat", "Aidat");
            }

            return View(aidat);
        }







    }
}
