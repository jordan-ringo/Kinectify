namespace Kinectify.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserProfiles",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserName = c.String(nullable: false, maxLength: 32),
                        KinectName = c.String(maxLength: 32),
                        ImageURL = c.String(maxLength: 2083),
                        VoiceURL = c.String(maxLength: 2083),
                        VoiceActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.UserPrograms",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserProfileID = c.Int(nullable: false),
                        Name = c.String(maxLength: 128),
                        ImageURL = c.String(maxLength: 2083),
                        DateLastUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.UserProfiles", t => t.UserProfileID, cascadeDelete: true)
                .Index(t => t.UserProfileID);
            
            CreateTable(
                "dbo.VoiceCommands",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserProfileID = c.Int(nullable: false),
                        UserProgramID = c.Int(nullable: false),
                        Keyword = c.String(maxLength: 128),
                        Phrase = c.String(maxLength: 128),
                        Action = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.UserProfiles", t => t.UserProfileID, cascadeDelete: true)
                .ForeignKey("dbo.UserPrograms", t => t.UserProgramID, cascadeDelete: false)
                .Index(t => t.UserProfileID)
                .Index(t => t.UserProgramID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VoiceCommands", "UserProgramID", "dbo.UserPrograms");
            DropForeignKey("dbo.VoiceCommands", "UserProfileID", "dbo.UserProfiles");
            DropForeignKey("dbo.UserPrograms", "UserProfileID", "dbo.UserProfiles");
            DropIndex("dbo.VoiceCommands", new[] { "UserProgramID" });
            DropIndex("dbo.VoiceCommands", new[] { "UserProfileID" });
            DropIndex("dbo.UserPrograms", new[] { "UserProfileID" });
            DropTable("dbo.VoiceCommands");
            DropTable("dbo.UserPrograms");
            DropTable("dbo.UserProfiles");
        }
    }
}
