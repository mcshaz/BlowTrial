namespace BlowTrial.Migrations.Membership
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MembershipV2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RandomisingMessages", "DischargeExplanation", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RandomisingMessages", "DischargeExplanation");
        }
    }
}
