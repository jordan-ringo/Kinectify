using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Kinectify.Models
{
	public class UserProgram
	{
		[Key]
		public int ID { get; set; }

		[ForeignKey("UserProfile")]
		public int UserProfileID { get; set; }

		[MaxLength(128)]
		[StringLength(128, MinimumLength = 3)]
		public string Name { get; set; }

		[MaxLength(2083)]
		[StringLength(2083)]
		public string ImageURL { get; set; }

		private DateTime _date = DateTime.Now;
		public DateTime DateLastUpdated
		{
			get { return _date; }
			set { _date = value; }
		}

		public virtual UserProfile UserProfile { get; set; }
		public virtual ICollection<VoiceCommand> VoiceCommands { get; set; }
	}
}