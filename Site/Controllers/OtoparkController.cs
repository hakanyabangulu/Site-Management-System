using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Site.Models;

namespace Site.Controllers
{
    public class OtoparkController : Controller
    {
        private site_dbEntities4 db = new site_dbEntities4();

       
        public ActionResult Otopark()
        {
            ViewBag.ShowNavbarLinks = true;
            var otoparkList = db.Otopark.SqlQuery("SELECT * FROM Otopark ORDER BY DaireID").ToList();

            return View(otoparkList);
        }

       
        public ActionResult Create()
        {
            ViewBag.ShowNavbarLinks = true;
            return View();
        }

        [HttpPost]
        public ActionResult Create(Otopark otopark)
        {
            if (ModelState.IsValid)
            {
               
                var sqlExistingCarsCount = "SELECT COUNT(*) FROM Otopark WHERE DaireID = @p0";
                var existingCarsCount = db.Database.SqlQuery<int>(sqlExistingCarsCount, otopark.DaireID).Single();

                
                if (existingCarsCount >= 1)
                {
                    otopark.UcretliParkYeriTutari = 150;
                }
                else
                {
                    otopark.UcretliParkYeriTutari = 0;
                }

                
                var sqlInsertOtopark = "INSERT INTO Otopark (DaireID, AracSayisi, UcretliParkYeriTutari, Plaka) VALUES (@p0, @p1, @p2, @p3)";
                db.Database.ExecuteSqlCommand(sqlInsertOtopark, otopark.DaireID, otopark.AracSayisi, otopark.UcretliParkYeriTutari, otopark.Plaka);

                return RedirectToAction("Otopark");
            }

            return View(otopark);
        }

        public ActionResult Delete(int? id)
            
        {
            ViewBag.ShowNavbarLinks = true;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Otopark otopark = db.Otopark.SqlQuery("SELECT * FROM Otopark WHERE OtoparkID = @p0", id).FirstOrDefault();
            if (otopark == null)
            {
                return HttpNotFound();
            }

            return View(otopark);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            
            db.Database.ExecuteSqlCommand("DELETE FROM Otopark WHERE OtoparkID = @p0", id);
            return RedirectToAction("Otopark");
        }
        public ActionResult Update(int? id)
        {
            ViewBag.ShowNavbarLinks = true;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Otopark otopark = db.Otopark.SqlQuery("SELECT * FROM Otopark WHERE OtoparkID = @p0", id).FirstOrDefault();
            if (otopark == null)
            {
                return HttpNotFound();
            }

            return View(otopark);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(Otopark updatedOtopark)
        {
            if (ModelState.IsValid)
            {
                db.Database.ExecuteSqlCommand(
                    "UPDATE Otopark SET DaireID = @p0, Plaka = @p1, UcretliParkYeriTutari = @p2, AracSayisi = @p3 WHERE OtoparkID = @p4",
                    updatedOtopark.DaireID, updatedOtopark.Plaka, updatedOtopark.UcretliParkYeriTutari, updatedOtopark.AracSayisi, updatedOtopark.OtoparkID);

                return RedirectToAction("Otopark");
            }
            return View(updatedOtopark);
        }




    }
}
