using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kinectify.Extensions
{
	public class MySession
	{
		// private constructor
		private MySession()
		{
			UserProfileID = -1;
		}

		// Gets the current session.
		public static MySession Current
		{
			get
			{
				MySession session =
				  (MySession)HttpContext.Current.Session["__MySession__"];
				if (session == null)
				{
					session = new MySession();
					HttpContext.Current.Session["__MySession__"] = session;
				}
				return session;
			}
		}

		// **** add your session properties here, e.g like this:
		//public string Property1 { get; set; }
		public int UserProfileID { get; set; }
	}
}