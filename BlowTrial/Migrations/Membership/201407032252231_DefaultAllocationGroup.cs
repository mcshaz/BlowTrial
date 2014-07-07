namespace BlowTrial.Migrations.Membership
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DefaultAllocationGroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BackupDatas", "DefaultAllocation", c => c.Int(nullable: false, defaultValue: 1));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BackupDatas", "DefaultAllocation");
        }
    }
}
