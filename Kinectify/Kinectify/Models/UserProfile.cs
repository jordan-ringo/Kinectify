using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Kinectify.Models
{
	public class UserProfile
	{
		public int ID { get; set; }

		[DisplayName("User Name")]
		[Required]
		[MaxLength(32)]
		[StringLength(32, MinimumLength = 3)]
		public string UserName { get; set; }

		[DisplayName("Kinect Name")]
		[DefaultValue("kinect")]
		[MaxLength(32)]
		[StringLength(32, MinimumLength = 3)]
		public string KinectName { get; set; }

		[DisplayName("Image URL")]
		[MaxLength(2083)]
		[StringLength(2083)]
		public string ImageURL { get; set; }

		[MaxLength(2083)]
		[StringLength(2083)]
		public string VoiceURL { get; set; }

		[DefaultValue(true)]
		public bool VoiceActive { get; set; }


		public virtual ICollection<UserProgram> UserPrograms { get; set; }
		
		public virtual ICollection<VoiceCommand> VoiceCommands { get; set; }
	}
}