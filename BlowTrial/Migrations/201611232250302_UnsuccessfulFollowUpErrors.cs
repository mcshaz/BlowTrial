namespace BlowTrial.Migrations.TrialData
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UnsuccessfulFollowUpErrors : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UnsuccessfulFollowUps", "Id_old", c => c.Int(nullable: false));
            Sql("UPDATE UnsuccessfulFollowUps SET Id_old = Id");

            DropPrimaryKey("dbo.UnsuccessfulFollowUps");
            DropColumn("dbo.UnsuccessfulFollowUps", "Id");

            AddColumn("dbo.UnsuccessfulFollowUps", "Id", c => c.Int(nullable: false));
            Sql("UPDATE UnsuccessfulFollowUps SET Id = Id_old");

            DropColumn("dbo.UnsuccessfulFollowUps", "Id_old");
            AddPrimaryKey("dbo.UnsuccessfulFollowUps", "Id");
            
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.UnsuccessfulFollowUps");
            AlterColumn("dbo.UnsuccessfulFollowUps", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.UnsuccessfulFollowUps", "Id");
        }
    }
}
