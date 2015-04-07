namespace BlowTrial.Migrations.Membership
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveMembershipDefaultAllocation : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.BackupDatas", "DefaultAllocation");
            DropTable("dbo.RandomisingMessages");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.RandomisingMessages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InterventionInstructions = c.String(maxLength: 200),
                        ControlInstructions = c.String(maxLength: 200),
                        DischargeExplanation = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.BackupDatas", "DefaultAllocation", c => c.Int(nullable: false));
        }
    }
}
