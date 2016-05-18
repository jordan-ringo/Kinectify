using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kinectify.Models;
using Kinectify.Extensions;

namespace Kinectify.Controllers
{
    public class UserProgramsController : Controller
    {
        private KinectifyContext db = new KinectifyContext();

        // GET: UserPrograms
        public ActionResult Index()
        {
			//var userPrograms = db.UserPrograms.Include(u => u.UserProfile);
			//return View(userPrograms.ToList());

			int currentUserProfileID = MySession.Current.UserProfileID;

			var userPrograms = db.UserPrograms.SqlQuery(
				"SELECT * FROM dbo.UserPrograms WHERE UserProfileID = " + currentUserProfileID +
				"ORDER BY Name").ToList();

			

			return View(userPrograms);
        }

        // GET: UserPrograms/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserProgram userProgram = db.UserPrograms.Find(id);
            if (userProgram == null)
            {
                return HttpNotFound();
            }
            return View(userProgram);
        }

        // GET: UserPrograms/Create
        public ActionResult Create()
        {
            ViewBag.UserProfileID = new SelectList(db.UserProfiles, "ID", "UserName");
            return View();
        }

        // POST: UserPrograms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,UserProfileID,Name,ImageURL,DateLastUsed")] UserProgram userProgram)
        {
            if (ModelState.IsValid)
            {
                db.UserPrograms.Add(userProgram);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserProfileID = new SelectList(db.UserProfiles, "ID", "UserName", userProgram.UserProfileID);
            return View(userProgram);
        }

        // GET: UserPrograms/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserProgram userProgram = db.UserPrograms.Find(id);
            if (userProgram == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserProfileID = new SelectList(db.UserProfiles, "ID", "UserName", userProgram.UserProfileID);
            return View(userProgram);
        }

        // POST: UserPrograms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserProfileID,Name,ImageURL,DateLastUsed")] UserProgram userProgram)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userProgram).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserProfileID = new SelectList(db.UserProfiles, "ID", "UserName", userProgram.UserProfileID);
            return View(userProgram);
        }

        // GET: UserPrograms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserProgram userProgram = db.UserPrograms.Find(id);
            if (userProgram == null)
            {
                return HttpNotFound();
            }
            return View(userProgram);
        }

        // POST: UserPrograms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserProgram userProgram = db.UserPrograms.Find(id);
            db.UserPrograms.Remove(userProgram);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
