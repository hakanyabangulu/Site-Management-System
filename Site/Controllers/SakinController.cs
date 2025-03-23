using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Site.Models;

namespace Site.Controllers
{
    public class SakinController : Controller
    {
        private site_dbEntities4 db = new site_dbEntities4();

        public ActionResult Sakin()
        {
            ViewBag.ShowNavbarLinks = true;
            var sakin = db.Sakin.SqlQuery("SELECT * FROM Sakin Order by DaireID").ToList();
            return View(sakin);
        }

        public ActionResult Update(int id)
        {
            ViewBag.ShowNavbarLinks = true;
            string query = "SELECT * FROM Sakin WHERE SakinID = @p0";
            Sakin SakinToUpdate = db.Sakin.SqlQuery(query, new SqlParameter("@p0", id)).FirstOrDefault();

            if (SakinToUpdate == null)
            {
                return HttpNotFound();
            }

            return View(SakinToUpdate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(Sakin updatedSakin)
        {
            ViewBag.ShowNavbarLinks = true;
            if (ModelState.IsValid)
            {
                
                string query = @"
        UPDATE Sakin
        SET AdiSoyadi = @AdiSoyadi,
            Telefon = @Telefon
        WHERE SakinID = @SakinID";

                
                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@AdiSoyadi", updatedSakin.AdiSoyadi ?? (object)DBNull.Value),
            new SqlParameter("@Telefon", updatedSakin.Telefon ?? (object)DBNull.Value),
            new SqlParameter("@SakinID", updatedSakin.SakinID)
                };

                int rowsAffected = db.Database.ExecuteSqlCommand(query, parameters);

                
                if (rowsAffected > 0)
                {
                    return RedirectToAction("Sakin", "Sakin");
                }
                else
                {
                    return HttpNotFound();
                }
            }

            return View(updatedSakin);
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.ShowNavbarLinks = true;
            return View(new Sakin());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Sakin sakin)
        {
            ViewBag.ShowNavbarLinks = true;
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        string insertQuery = @"
                    INSERT INTO Sakin (DaireID, AdiSoyadi, Telefon)
                    VALUES (@p0, @p1, @p2)";

                        
                        db.Database.ExecuteSqlCommand(
                            insertQuery,
                            new SqlParameter("@p0", sakin.DaireID),
                            new SqlParameter("@p1", sakin.AdiSoyadi),
                            new SqlParameter("@p2", sakin.Telefon)
                        );

                        
                        string updateQuery = @"
                    UPDATE Mülkiyet
                    SET KisiSayisi = KisiSayisi + 1
                    WHERE DaireID = @p0";

                        db.Database.ExecuteSqlCommand(
                            updateQuery,
                            new SqlParameter("@p0", sakin.DaireID)
                        );

                       
                        transaction.Commit();

                        return RedirectToAction("Sakin", "Sakin");
                    }
                    catch (Exception ex)
                    {
                        
                        transaction.Rollback();
                        ModelState.AddModelError("", "Sakin eklenirken bir hata oluştu.");
                        return View(sakin);
                    }
                }
            }
            return View(sakin);
        }


        public ActionResult Delete(int? id)

        {
            ViewBag.ShowNavbarLinks = true;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Sakin sakin = db.Sakin.SqlQuery("SELECT * FROM Sakin WHERE SakinID = @p0", id).FirstOrDefault();
            if (sakin == null)
            {
                return HttpNotFound();
            }

            return View(sakin);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            
            string updateQuery = @"
        UPDATE Mülkiyet
        SET KisiSayisi = KisiSayisi - 1
        WHERE DaireID = (
            SELECT TOP 1 DaireID
            FROM Sakin
            WHERE SakinID = @p0
        )";

            db.Database.ExecuteSqlCommand(updateQuery, new SqlParameter("@p0", id));

            
            string deleteQuery = "DELETE FROM Sakin WHERE SakinID = @p0";
            db.Database.ExecuteSqlCommand(deleteQuery, new SqlParameter("@p0", id));

            return RedirectToAction("Sakin");
        }
        
    }
}
