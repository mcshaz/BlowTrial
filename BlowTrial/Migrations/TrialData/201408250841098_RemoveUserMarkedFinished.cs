namespace BlowTrial.Migrations.TrialData
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveUserMarkedFinished : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Participants", "UserMarkedFinished");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Participants", "UserMarkedFinished", c => c.Boolean(nullable: false));
        }
    }
}
