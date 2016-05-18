using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectifyVoiceCommandListener
{
	class VoiceCommand
	{

		public int ID { get; set; }
		public int UserProfileID { get; set; }
		public int UserProgramID { get; set; }
		public string Keyword { get; set; }
		public string Phrase { get; set; }
		public string Action { get; set; }

	}
}
