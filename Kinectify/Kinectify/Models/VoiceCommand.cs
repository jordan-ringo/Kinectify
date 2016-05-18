using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Kinectify.Models
{
	public class VoiceCommand
	{
		[Key]
		public int ID { get; set; }

		[ForeignKey("UserProfile")]
		public int UserProfileID { get; set; }

		[ForeignKey("UserProgram")]
		public int UserProgramID { get; set; }

		[MaxLength(128)]
		[StringLength(128, MinimumLength = 3)]
		public string Keyword { get; set; }

		[MaxLength(128)]
		[StringLength(128, MinimumLength = 3)]
		public string Phrase { get; set; }

		[MaxLength(1024)]
		[StringLength(1024, MinimumLength = 1)]
		public string Action { get; set; }

		private DateTime _date = DateTime.Now;
		public DateTime DateLastUpdated
		{
			get { return _date; }
			set { _date = value; }
		}


		public virtual UserProfile UserProfile { get; set; }
		public virtual UserProgram UserProgram { get; set; }
	}
}