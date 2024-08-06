using project5_voting.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace project5_voting.Controllers
{
    public class ContactsController : Controller
    {
        ElectionsEntities2 db = new ElectionsEntities2();
        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult AdminContact()
        {
            return View(db.Contacts.ToList());
        }

        public ActionResult ContactDetails(int? id)
        {
            if (id == null)
            {   
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Contact contact = db.Contacts.Find(id);

            if (contact == null)
            {
                return HttpNotFound();
            }

            return View(contact);
        }

        public ActionResult Response(int? id)
        {
            Session["admin"] = "Omar";

            var admin_name = Session["admin"] as string;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Contact contact = db.Contacts.Find(id);
            contact.adminName = admin_name;

            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Response(Contact contact)
        {
            contact.responseDate = DateTime.Today;
            contact.responseTime = DateTime.Now.TimeOfDay;
            contact.status = "1";

            if (ModelState.IsValid)
            {
                db.Entry(contact).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AdminContact");
            }
            return View(contact);
        }

        public ActionResult ShowResponse(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = db.Contacts.Find(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }


        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = db.Contacts.Find(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        // POST: Contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Contact contact = db.Contacts.Find(id);
            db.Contacts.Remove(contact);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}