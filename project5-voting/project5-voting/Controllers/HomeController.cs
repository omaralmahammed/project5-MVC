using project5_voting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace project5_voting.Controllers
{
    public class HomeController : Controller
    {

        ElectionsEntities2 db = new ElectionsEntities2();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SelectList()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Search(string Search)
        {
            if (!string.IsNullOrEmpty(Search))
            {
                TempData["voterDetails"] = Search;
            }

            return RedirectToAction("VotersDetails");
        }

        public ActionResult VotersDetails()
        {
            var voter = TempData["voterDetails"] as string;

            if (!string.IsNullOrEmpty(voter))
            {
                var checkInputs = db.USERS.Where(model => model.NationalID == voter).FirstOrDefault();

                if (checkInputs != null)
                {
                    ViewBag.Error = true;
                    return View(checkInputs);
                }
                else
                {
                    ViewBag.Error = false;
                }
            }

            return View();
        }

    }
}