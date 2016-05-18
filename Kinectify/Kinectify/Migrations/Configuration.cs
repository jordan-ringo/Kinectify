namespace Kinectify.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
	using Kinectify.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<Kinectify.Models.KinectifyContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Kinectify.Models.KinectifyContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

			context.UserProfiles.AddOrUpdate
			(u => u.UserName,
				new UserProfile
				{
					UserName = "III RINGO III",
					KinectName = "Computer"
				},
				new UserProfile
				{
					UserName = "Sephy79",
					KinectName = "Kinectify"
				}
			);


			context.UserPrograms.AddOrUpdate
			(p => p.Name,
				new UserProgram
				{
					UserProfileID = 1,
					Name = "Computer",
					DateLastUpdated = DateTime.Parse("1900-01-01")
				},
				new UserProgram
				{
					UserProfileID = 1,
					Name = "Notepad",
					DateLastUpdated = DateTime.Parse("2015-09-01")
				},
				new UserProgram
				{
					UserProfileID = 1,
					Name = "Paint",
					DateLastUpdated = DateTime.Parse("2015-08-01")
				},
				new UserProgram
				{
					UserProfileID = 1,
					Name = "Windows Media Player",
					DateLastUpdated = DateTime.Parse("2016-03-01")
				},
				new UserProgram
				{
					UserProfileID = 2,
					Name = "Google Chrome",
					DateLastUpdated = DateTime.Parse("2016-01-01")
				},
				new UserProgram
				{
					UserProfileID = 2,
					Name = "Paint2",
					DateLastUpdated = DateTime.Parse("2016-02-01")
				}

			);

			context.VoiceCommands.AddOrUpdate
			(vc => vc.Keyword,
				new VoiceCommand
				{
					UserProfileID = 1,
					UserProgramID = 2,
					Keyword = "OPEN_NOTEPAD",
					Phrase = "open notepad",
					Action = "^{ESC}notepad{ENTER}"
				},
				new VoiceCommand
				{
					UserProfileID = 1,
					UserProgramID = 4,
					Keyword = "PLAY_MY_MUSIC_WMP",
					Phrase = "play my music",
					Action = "^{ESC}windows media player{ENTER}music{TAB}{ENTER}"
				},
				new VoiceCommand
				{
					UserProfileID = 1,
					UserProgramID = 4,
					Keyword = "STOP_SONG",
					Phrase = "stop song",
					Action = "^s"
				},
				new VoiceCommand
				{
					UserProfileID = 1,
					UserProgramID = 4,
					Keyword = "PLAY_SONG",
					Phrase = "play song",
					Action = "^p"
				},
				new VoiceCommand
				{
					UserProfileID = 1,
					UserProgramID = 1,
					Keyword = "EXIT_PROGRAM",
					Phrase = "exit program",
					Action = "%{F4}"
				},
				new VoiceCommand
				{
					UserProfileID = 1,
					UserProgramID = 1,
					Keyword = "DOWN_ARROW",
					Phrase = "down arrow",
					Action = "{DOWN}"
				},
				new VoiceCommand
				{
					UserProfileID = 1,
					UserProgramID = 1,
					Keyword = "UP_ARROW",
					Phrase = "up arrow",
					Action = "{UP}"
				},
				new VoiceCommand
				{
					UserProfileID = 1,
					UserProgramID = 1,
					Keyword = "RIGHT_ARROW",
					Phrase = "right arrow",
					Action = "{RIGHT}"
				},
				new VoiceCommand
				{
					UserProfileID = 1,
					UserProgramID = 1,
					Keyword = "LEFT_ARROW",
					Phrase = "left arrow",
					Action = "{LEFT}"
				},
				new VoiceCommand
				{
					UserProfileID = 1,
					UserProgramID = 1,
					Keyword = "VOLUME_DOWN",
					Phrase = "volume down",
					Action = "^{ESC}adjust system volume{ENTER}{{DOWN} 5}{ESC}"
				},
				new VoiceCommand
				{
					UserProfileID = 1,
					UserProgramID = 1,
					Keyword = "VOLUME_UP",
					Phrase = "volume up",
					Action = "^{ESC}adjust system volume{ENTER}{{UP} 5}{ESC}"
				},
				new VoiceCommand
				{
					UserProfileID = 1,
					UserProgramID = 1,
					Keyword = "ENTER_KEY",
					Phrase = "press enter",
					Action = "{ENTER}"
				}
			);

        }
    }
}
