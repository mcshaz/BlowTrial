namespace BlowTrial.Migrations.TrialData
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BcgScarFollowUp : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UnsuccessfulFollowUps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AttemptedContact = c.DateTime(nullable: false),
                        ParticipantId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Participants", t => t.ParticipantId, cascadeDelete: true)
                .Index(t => t.ParticipantId);
            
            AddColumn("dbo.Participants", "FollowUpContactMade", c => c.DateTime());
            AddColumn("dbo.Participants", "FollowUpComment", c => c.String(maxLength: 512));
            AddColumn("dbo.Participants", "PermanentlyUncontactable", c => c.Boolean(nullable: false, defaultValue:false));
            AddColumn("dbo.Participants", "MaternalBCGScar", c => c.Int(nullable: false,defaultValue:0));
            AddColumn("dbo.Participants", "FollowUpBabyBCGReaction", c => c.Int(nullable: false,defaultValue:0));
            AddColumn("dbo.Vaccines", "IsBcg", c => c.Boolean(nullable: false, defaultValue: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UnsuccessfulFollowUps", "ParticipantId", "dbo.Participants");
            DropIndex("dbo.UnsuccessfulFollowUps", new[] { "ParticipantId" });
            DropColumn("dbo.Participants", "FollowUpBabyBCGReaction");
            DropColumn("dbo.Participants", "MaternalBCGScar");
            DropColumn("dbo.Participants", "PermanentlyUncontactable");
            DropColumn("dbo.Participants", "FollowUpComment");
            DropColumn("dbo.Participants", "FollowUpContactMade");
            DropTable("dbo.UnsuccessfulFollowUps");
        }
    }
}
