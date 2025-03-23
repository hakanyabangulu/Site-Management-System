using Microsoft.Ajax.Utilities;
using Site.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Site.Controllers
{

    public class HomeController : Controller

    {
        site_dbEntities4 db = new site_dbEntities4();
        public ActionResult Index()
        {
            ViewBag.ShowNavbarLinks = false;
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(users model, string username, string password)
        {
            var loginUser = db.users.SqlQuery("SELECT * FROM users WHERE username = @name AND password = @password",
                new SqlParameter("@name", username),
                new SqlParameter("@password", password)
            ).FirstOrDefault();

            if (loginUser != null && loginUser.userID == 1)
            {
                ViewBag.ShowNavbarLinks = true;
                return RedirectToAction("Page");
            }
            else if (loginUser == null)
            {
                ViewBag.Error = "Kullanıcı adı veya şifre yanlış.";
                return View("Login");
            }
            else
            {
                return RedirectToAction("User");
            }
        }

        public ActionResult User()
        {
            return View();
        }
        public ActionResult Page()
        {
            ViewBag.ShowNavbarLinks = true;
            return View();
        }
        public ActionResult Daire()
        {
            var daireler = db.Daire.ToList();
            return View(daireler);
        }

        public ActionResult Aidat(int daireID)
        {
            ViewBag.ShowNavbarLinks = false;


            var aidatlar = db.Database.SqlQuery<Aidat>("SELECT * FROM Aidat WHERE DaireID = @daireID ORDER BY DaireID, CAST(SUBSTRING(AidatAyi, 1, 2) AS INT) ASC", new SqlParameter("@daireID", daireID)).ToList();




            if (!aidatlar.Any())
            {
                ViewBag.Message = "Bu daireye ait aidat bilgisi bulunmamaktadır.";
            }

            return View(aidatlar);
        }



        public ActionResult Odeme(int? AidatID)
        {
            if (!AidatID.HasValue)
            {

                return HttpNotFound();
            }

            var aidat = db.Aidat.FirstOrDefault(a => a.AidatID == AidatID.Value);
            if (aidat == null)
            {
                return HttpNotFound();
            }

            return View(aidat);
        }
        /*private void SendEmail(string toEmail, string subject, string body)
        {
            var smtpClient = new SmtpClient("smtp.office365.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("your-email@outlook.com", "your-password"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("your-email@outlook.com"),
                Subject = subject,
                Body = body,
                IsBodyHtml = false, 
            };

            mailMessage.To.Add(toEmail);

            smtpClient.Send(mailMessage);
        }


        [HttpPost]
        public ActionResult Send3DSecureCode(int AidatID)
        {
            var aidat = db.Aidat.FirstOrDefault(a => a.AidatID == AidatID);
            if (aidat == null)
            {
                return HttpNotFound();
            }

            
            var randomCode = new Random().Next(100000, 999999).ToString();

           
            TempData["SecureCode"] = randomCode;
            TempData["AidatID"] = AidatID;

           
            try
            {
                string userEmail = "hakanyabangulu@hotmail.com"; 
                SendEmail(userEmail, "3D Secure Doğrulama Kodu", $"Doğrulama Kodunuz: {randomCode}");

                return Json(new { success = true, message = "Doğrulama kodu e-posta adresinize gönderildi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"E-posta gönderilemedi: {ex.Message}" });
            }
        }


        [HttpPost]
        public ActionResult OdemeTamamla(int AidatID, string HolderName, string CardNumber, string ExpireMonth, string ExpireYear, string CV2, string SecureCode)
        {
            var storedCode = TempData["SecureCode"] as string;

            
            if (storedCode == null || SecureCode != storedCode)
            {
                TempData["ErrorMessage"] = "Girilen doğrulama kodu geçersiz.";
                return RedirectToAction("Odeme", new { AidatID });
            }

            var aidat = db.Aidat.FirstOrDefault(a => a.AidatID == AidatID);
            if (aidat == null)
            {
                return HttpNotFound();
            }

            
            aidat.OdemeDurumu = "Ödendi (Simüle)";
            db.SaveChanges();

            
            Console.WriteLine($"Ödeme Başarılı! Kart Sahibi: {HolderName}, Kart No: {CardNumber}");

            
            return RedirectToAction("Dekont", new { AidatID = aidat.AidatID });
        }*/
        [HttpPost]
        public ActionResult OdemeTamamla(int AidatID, string HolderName, string CardNumber, string ExpireMonth, string ExpireYear, string CV2)
        {

            var aidat = db.Aidat.FirstOrDefault(a => a.AidatID == AidatID);
            if (aidat == null)
            {
                return HttpNotFound();
            }


            aidat.OdemeDurumu = "Ödendi";
            db.SaveChanges();


            return RedirectToAction("Dekont", new { AidatID = aidat.AidatID, HolderName, CardNumber, ExpireMonth, ExpireYear, CV2 });
        }




        public ActionResult Dekont(int AidatID, string HolderName, string CardNumber, string ExpireMonth, string ExpireYear, string CV2)
        {

            var aidat = db.Aidat.FirstOrDefault(a => a.AidatID == AidatID);
            if (aidat == null)
            {
                return HttpNotFound();
            }


            ViewBag.HolderName = HolderName;
            ViewBag.CardNumber = CardNumber.Substring(CardNumber.Length - 4);
            ViewBag.ExpireMonth = ExpireMonth;
            ViewBag.ExpireYear = ExpireYear;
            ViewBag.CV2 = CV2;


            return View(aidat);
        }

        public ActionResult Fatura()
        {
            ViewBag.ShowNavbarLinks = false;
            var fatura = db.Fatura.SqlQuery("Select * From Fatura ORDER by DaireID").ToList(); ;
            return View(fatura.ToList());
        }
        public ActionResult Otopark()
        {
            ViewBag.ShowNavbarLinks = false;
            var otopark = db.Otopark.SqlQuery("Select * from Otopark ORDER by DaireID");
            return View(otopark.ToList());
        }
        public ActionResult Yönetici()
        {
            ViewBag.ShowNavbarLinks = false;
            var yönetici = db.Blok.SqlQuery("Select * from Blok ORDER by BlokID").ToList();
            return View(yönetici.ToList());

        }
        public ActionResult Dilek()
        {
            ViewBag.ShowNavbarLinks = true;
            return View();
        }
        /*private static List<Message> Messages = new List<Message>();
        

        [HttpPost]
        public ActionResult Mesaj(string name, string email, string message)
        {
            // Yeni mesajı listeye ekle
            Messages.Add(new Message
            {
                Name = name,
                Email = email,
                Content = message,
                CreatedAt = DateTime.Now,
                Status = "Bekliyor"
            });

            TempData["Success"] = "Mesajınız başarıyla gönderildi.";
            return RedirectToAction("User");
        }
        public ActionResult AdminMesaj()
        {
            return View(Messages); // Admin mesajları listeleyebilir
        }


    }*/

        // Mesaj modelini burada tanımlıyoruz
        /*public class Message
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string Content { get; set; }
            public DateTime CreatedAt { get; set; }
            public string Status { get; set; }
        }*/
    }












}