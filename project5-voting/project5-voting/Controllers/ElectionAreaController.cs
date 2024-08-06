using project5_voting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace project5_voting.Controllers
{
    public class ElectionAreaController : Controller
    {
        ElectionsEntities2 db = new ElectionsEntities2();

        public ActionResult localANDparty()
        {
            return View();
        }

        public ActionResult areaName()
        {

            return View(db.electionAreas.ToList());
        }

        public ActionResult listsDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var localList = db.localLists.Where(x => x.id == id).ToList();

            return View(localList);
        }

        public ActionResult CandidatesName(int? id)
        {
            var CandidatesNames = db.localCandidates.Where(x => x.id == id).ToList();

            return View(CandidatesNames);
        }



        public ActionResult partyName()
        {
            return View(db.partyLists.ToList());
        }


        public ActionResult PatryCandidatesName(int? id)
        {
            var PartyCandidatesNames = db.partyCandidates.Where(x => x.partyId == id).ToList();

            return View(PartyCandidatesNames);
        }
    }
}