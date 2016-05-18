namespace Kinectify.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class testName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VoiceCommands", "DateLastUpdated", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.VoiceCommands", "DateLastUpdated");
        }
    }
}
