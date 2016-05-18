namespace Kinectify.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class vcActionSize : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.VoiceCommands", "Action", c => c.String(maxLength: 1024));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.VoiceCommands", "Action", c => c.String(maxLength: 128));
        }
    }
}
