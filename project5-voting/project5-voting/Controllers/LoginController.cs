﻿using project5_voting.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using System.Web;
using System.Web.Mvc;
using MailKit.Net.Smtp;
using Microsoft.Ajax.Utilities;
using MimeKit;

namespace project5_voting.Controllers
{
    public class LoginController : Controller
    {
        ElectionsEntities2 db = new ElectionsEntities2();


        public ActionResult firststeplogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult firststeplogin(USER user)
        {

            var logged_user = db.USERS.FirstOrDefault(u => u.email == user.email && u.NationalID == user.NationalID);


            if (logged_user == null)
            {
                ViewBag.massage = "يرجى التأكد من صحة الرقم الوطني الخاص بك.";
                return View();
            }
           

            if (user.password.ToLower() == "password" && logged_user.password == "password")
            {
                Random rand = new Random();

                string randomNumber = Convert.ToString(rand.Next(10000000, 100000000)); // Upper bound is exclusive
                logged_user.password = randomNumber;
                db.Entry(logged_user).State = EntityState.Modified;
                db.SaveChanges();
                try
                {
                    string fromEmail = "ayahaldomi@gmail.com";
                    string fromName = "test";
                    string subjectText = "subject";
                    string messageText = $@"
                    <html>
                    <body dir='rtl'>
                        <h2>مرحباً!</h2>
                        <p>شكراً لانضمامك إلينا. للبدء، يرجى استخدام كلمة المرور المؤقتة التالية لضبط حسابك:</p>
                        <p><strong>كلمة المرور: {randomNumber}</strong></p>
                        <p>إذا كانت لديك أي أسئلة أو تحتاج إلى مساعدة إضافية، لا تتردد في الاتصال بفريق الدعم لدينا.</p>
                        <p>مع أطيب التحيات,<br>فريق الدعم</p>
                    </body>
                    </html>";
                    string toEmail = "election2024jordan@gmail.com";
                    string smtpServer = "smtp.gmail.com";
                    int smtpPort = 465; // Port 465 for SSL

                    string smtpUsername = "election2024jordan@gmail.com";
                    string smtpPassword = "zwht jwiz ivfr viyt"; // Ensure this is correct

                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress(fromName, fromEmail));
                    message.To.Add(new MailboxAddress("", toEmail));
                    message.Subject = subjectText;
                    message.Body = new TextPart("html") { Text = messageText };

                    using (var client = new SmtpClient())
                    {
                        client.Connect(smtpServer, smtpPort, true); // Use SSL
                        client.Authenticate(smtpUsername, smtpPassword);
                        client.Send(message);
                        client.Disconnect(true);
                    }

                    ViewBag.Message = "Email sent successfully.";

                    // Try to get the existing cookie
                    HttpCookie existingCookie = Request.Cookies["logedInUser"];
                    HttpCookie existingCookie1 = Request.Cookies["electionArea"];

                    if (existingCookie != null)
                    {
                        // Cookie exists, update its value
                        existingCookie.Value = logged_user.NationalID;
                        existingCookie.Expires = DateTime.Now.AddHours(1); // Set the expiration time
                        Response.Cookies.Set(existingCookie); // Update the cookie

                        existingCookie1.Value = logged_user.election_area;
                        existingCookie1.Expires = DateTime.Now.AddHours(1); // Set the expiration time
                        Response.Cookies.Set(existingCookie1); // Update the cookie
                    }
                    else
                    {
                        // Cookie does not exist, create a new one
                        HttpCookie newCookie = new HttpCookie("logedInUser", logged_user.NationalID);
                        newCookie.Expires = DateTime.Now.AddHours(1); // Set the expiration time
                        Response.Cookies.Add(newCookie); // Add the new cookie to the response

                        HttpCookie newCookie1 = new HttpCookie("electionArea", logged_user.election_area);
                        newCookie1.Expires = DateTime.Now.AddHours(1); // Set the expiration time
                        Response.Cookies.Add(newCookie1); // Add the new cookie to the response
                    }

                    return RedirectToAction("PasswordReset");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "An unexpected error occurred. Please try again later.";
                    Console.WriteLine("Exception message: " + ex.Message);
                    Console.WriteLine("Stack trace: " + ex.StackTrace);
                    return View("Index");
                }
            }
            else if (user.password == logged_user.password)
            {
                HttpCookie existingCookie = Request.Cookies["logedInUser"];
                HttpCookie existingCookie1 = Request.Cookies["electionArea"];

                if (existingCookie != null)
                {
                    // Cookie exists, update its value
                    existingCookie.Value = logged_user.NationalID;
                    existingCookie.Expires = DateTime.Now.AddHours(1); // Set the expiration time
                    Response.Cookies.Set(existingCookie); // Update the cookie

                    existingCookie1.Value = logged_user.election_area;
                    existingCookie1.Expires = DateTime.Now.AddHours(1); // Set the expiration time
                    Response.Cookies.Set(existingCookie1); // Update the cookie
                }
                else
                {
                    // Cookie does not exist, create a new one
                    HttpCookie newCookie = new HttpCookie("logedInUser", logged_user.NationalID);
                    newCookie.Expires = DateTime.Now.AddHours(1); // Set the expiration time
                    Response.Cookies.Add(newCookie); // Add the new cookie to the response

                    HttpCookie newCookie1 = new HttpCookie("electionArea", logged_user.election_area);
                    newCookie1.Expires = DateTime.Now.AddHours(1); // Set the expiration time
                    Response.Cookies.Add(newCookie1); // Add the new cookie to the response
                }
                return RedirectToAction("electionArea");
            }
            else
            {
                ViewBag.Title = "please enter valed info";
                return View();
            }


        }

        // GET: PasswordReset
        public ActionResult PasswordReset()
        {
            return View();
        }
        [HttpPost]
        public ActionResult PasswordReset(string TempPassword)
        {

            var logedInUser = Request.Cookies["logedInUser"];

            var nationalId = logedInUser.Value; // Extract NationalID from cookie value
            var logged_user = db.USERS.FirstOrDefault(u => u.NationalID == nationalId);
            if (TempPassword == logged_user.password)
            {
                return RedirectToAction("ResetPassword");
            }
            return View();
        }

        public ActionResult ResetPassword()
        {
            return View();
        }
        // POST: ResetPassword
        [HttpPost]
        public ActionResult ResetPassword(string NewPassword, string ConfirmPassword)
        {
            // Check if the new passwords match
            if (NewPassword != ConfirmPassword)
            {
                ViewBag.Message = "Passwords do not match.";
                return View("aaaaaaaaaaa");
            }

            // Extract the user ID from the cookie
            HttpCookie logedInUser = Request.Cookies["logedInUser"];
            if (logedInUser == null || string.IsNullOrEmpty(logedInUser.Value))
            {
                ViewBag.Message = "User session has expired or is invalid.";
                return View("bbbbbbbbbbbb");
            }

            // Assuming the cookie value is the user ID
            long userId;
            if (!long.TryParse(logedInUser.Value, out userId))
            {
                ViewBag.Message = "Invalid user session.";
                return View("cccccccc");
            }

            // Find the user by ID

            var nationalId = logedInUser.Value; // Extract NationalID from cookie value
            var logged_user = db.USERS.FirstOrDefault(u => u.NationalID == nationalId);
            if (logged_user == null)
            {
                ViewBag.Message = "User not found.";
                return View("ddddddd");
            }

            // Update the user's password
            logged_user.password = NewPassword;
            db.Entry(logged_user).State = EntityState.Modified;
            db.SaveChanges();

            // Set success message
            ViewBag.Message = "Password has been successfully reset.";
            return RedirectToAction("electionArea");
        }

        //////////////////////////////////////////////////////////////////////
        ///

        public ActionResult electionArea()
        {

            return View(db.electionAreas.ToList());

        }

        public ActionResult localOrparty()
        {

            return View(db.USERS.ToList());

        }

        [HttpPost]
        public ActionResult localOrparty(string hi)
        {
            HttpCookie logedInUser = Request.Cookies["logedInUser"];
            var nationalId = logedInUser.Value; // Extract NationalID from cookie value
            var logged_user = db.USERS.FirstOrDefault(u => u.NationalID == nationalId);


            if (logged_user.localYouth == 0)
            {
                logged_user.whileLocalVote = 1;
                db.Entry(logged_user).State = EntityState.Modified;
                db.SaveChanges();
            }
            if (logged_user.partyVote == 0)
            {
                logged_user.whilePartyVote = 1;
                db.Entry(logged_user).State = EntityState.Modified;
                db.SaveChanges();
            }

            return View("index");

        }

        public ActionResult partyVoting()
        {

            var partyLists = db.partyLists.ToList(); // Get list of partyLists
            return View(partyLists); // Pass the list to the view

        }
        [HttpPost]
        public ActionResult partyVoting(int? selectedPartyId)
        {
            var partyVote = db.partyLists.FirstOrDefault(u => u.id == selectedPartyId);
            HttpCookie logedInUser = Request.Cookies["logedInUser"];
            var nationalId = logedInUser.Value; // Extract NationalID from cookie value
            var logged_user = db.USERS.FirstOrDefault(u => u.NationalID == nationalId);


            if (selectedPartyId == null)
            {
                logged_user.whilePartyVote = 1;
                db.Entry(logged_user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("localORparty");
            }
            partyVote.counter = partyVote.counter + 1;
            db.Entry(partyVote).State = EntityState.Modified;
            db.SaveChanges();


            logged_user.partyVote = 1;
            db.Entry(logged_user).State = EntityState.Modified;
            db.SaveChanges();



            // Get list of partyLists
            return RedirectToAction("localORparty"); // Pass the list to the view

        }

        public ActionResult localVoting()
        {
            HttpCookie electionArea = Request.Cookies["electionArea"];

            // Filter and group candidates by list where status is "approved"
            var candidatesGrouped = db.localLists
            .Where(l => l.status == "1" && l.electionDistrict == electionArea.Value)
            .Select(l => new LocalCandidatesGroupedViewModel
            {
                ListName = l.listName,
                Candidates = db.localCandidates
                    .Where(c => c.listKey == l.id)
                    .ToList()
            })
            .ToList();

            return View(candidatesGrouped);

        }

        [HttpPost]
        public ActionResult localVoting(string selectedList, long[] selectedCandidates)
        {
            HttpCookie logedInUser = Request.Cookies["logedInUser"];
            var nationalId = logedInUser.Value; // Extract NationalID from cookie value
            var logged_user = db.USERS.FirstOrDefault(u => u.NationalID == nationalId);

            var selectedListDetails = db.localLists.FirstOrDefault(l => l.listName == selectedList);
            selectedListDetails.counter = selectedListDetails.counter + 1;
            db.Entry(selectedListDetails).State = EntityState.Modified;
            db.SaveChanges();

            if (selectedListDetails != null)
            {
                // Increment the counter for each selected candidate
                foreach (var candidateId in selectedCandidates)
                {
                    var candidate = db.localCandidates
                        .FirstOrDefault(c => c.id == candidateId);



                    candidate.counter = candidate.counter + 1;
                    db.Entry(candidate).State = EntityState.Modified;
                    db.SaveChanges();

                }
                logged_user.localYouth = 1;
                db.Entry(logged_user).State = EntityState.Modified;
                db.SaveChanges();


            }
            else
            {
                logged_user.whileLocalVote = 1;
                db.Entry(logged_user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("localORparty");
            }
            return View("index");

        }

        public ActionResult LoginAdmin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LoginAdmin(Admin model)
        {
            if (ModelState.IsValid)
            {
                var admin = db.Admins
                    .Where(a => a.email == model.email && a.temporaryPassword == model.temporaryPassword)
                    .SingleOrDefault();

                if (admin != null)
                {
                    // تسجيل الدخول ناجح، يمكنك التوجيه إلى صفحة أخرى أو إنشاء جلسة
                    Session["Admin"] = admin;
                    return RedirectToAction("IndexAdmin", "partyLists"); // أو أي صفحة تريد الانتقال إليها بعد تسجيل الدخول
                }
                else
                {
                    // البيانات غير صحيحة
                    ModelState.AddModelError("", "البريد الإلكتروني أو كلمة المرور غير صحيحة");
                }
            }

            return View(model);
        }

    }
}