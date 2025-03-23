using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Site.Models;

namespace Site.Controllers
{
    public class BlokController : Controller
    {
        private site_dbEntities4 db = new site_dbEntities4();

        public ActionResult Blok()
        {
            ViewBag.ShowNavbarLinks = true;
            List<Blok> bloklar = db.Blok.SqlQuery("SELECT * FROM Blok").ToList();
            return View(bloklar);
        }

        public ActionResult Update(int id)
        {
            ViewBag.ShowNavbarLinks = true;

            string query = "SELECT * FROM Blok WHERE BlokID = @BlokID";
            SqlParameter parameter = new SqlParameter("@BlokID", id);
            Blok blokToUpdate = db.Blok.SqlQuery(query, parameter).FirstOrDefault();

            if (blokToUpdate == null)
            {
                return HttpNotFound();
            }

            return View(blokToUpdate);
        }

        [HttpPost]
        public ActionResult Update(Blok updatedBlok)
        {
            if (ModelState.IsValid)
            {
              
                string sqlQuery = "UPDATE Blok SET YoneticiDaireID = @YoneticiDaireID WHERE BlokID = @BlokID";

                db.Database.ExecuteSqlCommand(
                    sqlQuery,
                    new SqlParameter("@YoneticiDaireID", updatedBlok.YoneticiDaireID),
                    new SqlParameter("@BlokID", updatedBlok.BlokID)
                );

                
                if (updatedBlok.YoneticiDaireID != null)
                {
                    int yoneticiDaireID = updatedBlok.YoneticiDaireID.Value;
                    UpdateYoneticiBilgileri(yoneticiDaireID);
                }

                return RedirectToAction("Blok");
            }
            return View(updatedBlok);
        }

        private void UpdateYoneticiBilgileri(int yoneticiDaireID)
        {
            
            var daire = db.Mülkiyet.FirstOrDefault(d => d.DaireID == yoneticiDaireID);

            if (daire != null)
            {
               
                string sqlQuery = "UPDATE Blok SET YoneticiAdiSoyadi = @YoneticiAdiSoyadi, YoneticiTelefonu = @YoneticiTelefonu WHERE YoneticiDaireID = @YoneticiDaireID";

                db.Database.ExecuteSqlCommand(
                    sqlQuery,
                    new SqlParameter("@YoneticiAdiSoyadi", daire.AdiSoyadi),
                    new SqlParameter("@YoneticiTelefonu", daire.Telefon),
                    new SqlParameter("@YoneticiDaireID", yoneticiDaireID)
                );
            }
        }
    }
}
