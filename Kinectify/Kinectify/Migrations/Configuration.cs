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
				},
				new UserProfile
				{
					UserName = "Sephy79",
				}
			);


			context.UserPrograms.AddOrUpdate
			(p => p.UserProfileID,
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
					UserProgramID = 1,
					Keyword = "OPEN_NOTEPAD",
					Phrase = "open notepad",
					Action = "^{ESC}notepad~"
				}
			);

        }
    }
}
