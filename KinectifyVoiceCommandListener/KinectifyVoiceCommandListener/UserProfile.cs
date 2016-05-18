using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectifyVoiceCommandListener
{
	class UserProfile
	{

		public UserProfile()
		{
			UserName = "New User";
		}

		public int ID { get; set; }
		public string UserName { get; set; }
		public string KinectName { get; set; }
		public string ImageURL { get; set; }
		public string VoiceURL { get; set; }
		public bool VoiceActive { get; set; }
	}
}
