using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kinectify.Models;

namespace Kinectify.Controllers
{
    public class VoiceCommandsController : Controller
    {
        private KinectifyContext db = new KinectifyContext();

        // GET: VoiceCommands
        public ActionResult Index()
        {
            var voiceCommands = db.VoiceCommands.Include(v => v.UserProfile).Include(v => v.UserProgram);
            return View(voiceCommands.ToList());
        }

        // GET: VoiceCommands/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VoiceCommand voiceCommand = db.VoiceCommands.Find(id);
            if (voiceCommand == null)
            {
                return HttpNotFound();
            }
            return View(voiceCommand);
        }

        // GET: VoiceCommands/Create
        public ActionResult Create()
        {
            ViewBag.UserProfileID = new SelectList(db.UserProfiles, "ID", "UserName");
            ViewBag.UserProgramID = new SelectList(db.UserPrograms, "ID", "Name");
            return View();
        }

        // POST: VoiceCommands/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,UserProfileID,UserProgramID,Keyword,Phrase,Action")] VoiceCommand voiceCommand)
        {
            if (ModelState.IsValid)
            {
                db.VoiceCommands.Add(voiceCommand);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserProfileID = new SelectList(db.UserProfiles, "ID", "UserName", voiceCommand.UserProfileID);
            ViewBag.UserProgramID = new SelectList(db.UserPrograms, "ID", "Name", voiceCommand.UserProgramID);
            return View(voiceCommand);
        }

        // GET: VoiceCommands/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VoiceCommand voiceCommand = db.VoiceCommands.Find(id);
            if (voiceCommand == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserProfileID = new SelectList(db.UserProfiles, "ID", "UserName", voiceCommand.UserProfileID);
            ViewBag.UserProgramID = new SelectList(db.UserPrograms, "ID", "Name", voiceCommand.UserProgramID);
            return View(voiceCommand);
        }

        // POST: VoiceCommands/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserProfileID,UserProgramID,Keyword,Phrase,Action")] VoiceCommand voiceCommand)
        {
            if (ModelState.IsValid)
            {
                db.Entry(voiceCommand).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserProfileID = new SelectList(db.UserProfiles, "ID", "UserName", voiceCommand.UserProfileID);
            ViewBag.UserProgramID = new SelectList(db.UserPrograms, "ID", "Name", voiceCommand.UserProgramID);
            return View(voiceCommand);
        }

        // GET: VoiceCommands/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VoiceCommand voiceCommand = db.VoiceCommands.Find(id);
            if (voiceCommand == null)
            {
                return HttpNotFound();
            }
            return View(voiceCommand);
        }

        // POST: VoiceCommands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            VoiceCommand voiceCommand = db.VoiceCommands.Find(id);
            db.VoiceCommands.Remove(voiceCommand);
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
