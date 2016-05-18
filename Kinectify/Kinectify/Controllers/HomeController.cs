using Kinectify.Extensions;
using Kinectify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Kinectify.Controllers
{
	public class HomeController : Controller
	{
		private KinectifyContext db = new KinectifyContext();


		public ActionResult Index(int? id)
		{
			if ((id == null) && (MySession.Current.UserProfileID == -1))
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			else if (id != null)
			{
				MySession.Current.UserProfileID = (int)id;
			}

			int currentUserProfileID = MySession.Current.UserProfileID;

			ViewBag.UserProfile = db.UserProfiles.Find(currentUserProfileID);


			ViewBag.UserPrograms = db.UserPrograms.SqlQuery(
				"SELECT TOP 6 * FROM dbo.UserPrograms WHERE UserProfileID = " + currentUserProfileID +
				"ORDER BY DateLastUpdated").ToArray();

			ViewBag.VoiceCommands = db.VoiceCommands.SqlQuery(
				"SELECT TOP 6 * FROM dbo.VoiceCommands WHERE UserProfileID = " + currentUserProfileID +
				"ORDER BY DateLastUpdated").ToArray();

			return View();
		}


		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			UserProfile userProfile = db.UserProfiles.Find(id);
			if (userProfile == null)
			{
				return HttpNotFound();
			}
			return View(userProfile);
		}


		public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}
	}
}