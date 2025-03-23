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
    public class FaturaController : Controller
    {
        private site_dbEntities4 db = new site_dbEntities4();

        public ActionResult Fatura()
        {
            ViewBag.ShowNavbarLinks = true;
            List<Fatura> fatura = db.Fatura.SqlQuery("SELECT * FROM Fatura ORDER BY DaireID, CAST(SUBSTRING(Ay, 1, 2) AS INT) ASC").ToList();

            return View(fatura.ToList());
        }

        public ActionResult Update(int id)
        {
            ViewBag.ShowNavbarLinks = true;

            Fatura faturaToUpdate = db.Fatura.SqlQuery("SELECT * FROM Fatura WHERE FaturaID = @p0", id).FirstOrDefault();

            if (faturaToUpdate == null)
            {
                return HttpNotFound();
            }

            return View(faturaToUpdate);
        }

        [HttpPost]
        public ActionResult Update(Fatura faturaUpdate)
        {
            if (ModelState.IsValid)
            {
                string sqlQuery = "UPDATE Fatura SET FaturaTipi = @FaturaTipi, Ay = @Ay, Tutar = @Tutar WHERE FaturaID = @FaturaID";

                db.Database.ExecuteSqlCommand(
                    sqlQuery,
                    new SqlParameter("@FaturaTipi", faturaUpdate.FaturaTipi ?? (object)DBNull.Value),
                    new SqlParameter("@Ay", faturaUpdate.Ay ?? (object)DBNull.Value),
                    new SqlParameter("@Tutar", faturaUpdate.Tutar == 0 ? (object)DBNull.Value : faturaUpdate.Tutar),
                    new SqlParameter("@FaturaID", faturaUpdate.FaturaID)
                );

                return RedirectToAction("Fatura", "Fatura");
            }
            return View(faturaUpdate);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Fatura fatura)
        {
            if (ModelState.IsValid)
            {
                string sqlQuery = "INSERT INTO Fatura (DaireID, Ay, Tutar, FaturaTipi) " +
                                  "VALUES (@DaireID, @Ay, @Tutar, @FaturaTipi)";

                db.Database.ExecuteSqlCommand(
                    sqlQuery,
                    new SqlParameter("@DaireID", fatura.DaireID),
                    new SqlParameter("@Ay", fatura.Ay ?? (object)DBNull.Value),
                    new SqlParameter("@Tutar", fatura.Tutar == 0 ? (object)DBNull.Value : fatura.Tutar), // Non-nullable float
                    new SqlParameter("@FaturaTipi", fatura.FaturaTipi ?? (object)DBNull.Value)
                );

                return RedirectToAction("Fatura", "Fatura");
            }

            return View(fatura);
        }



        public ActionResult Delete(int id)
        {
            string query = "SELECT * FROM Fatura WHERE FaturaID = @FaturaID";
            SqlParameter parameter = new SqlParameter("@FaturaID", id);
            var fatura = db.Fatura.SqlQuery(query, parameter).SingleOrDefault();

            if (fatura == null)
            {
                return HttpNotFound();
            }

            return View(fatura);
        }

       



        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var fatura = db.Fatura.Find(id);
            if (fatura != null)
            {
                string sqlQuery = "DELETE FROM Fatura WHERE FaturaID = @FaturaID";

                db.Database.ExecuteSqlCommand(
                    sqlQuery,
                    new SqlParameter("@FaturaID", id)
                );
            }

            return RedirectToAction("Fatura");
        }




    }
}
