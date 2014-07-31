namespace BlowTrial.Migrations.TrialData
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AllocationBlockIdNotNull : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Participants", "AllocationBlockId", "dbo.AllocationBlocks");
            DropIndex("dbo.Participants", new[] { "AllocationBlockId" });
            AlterColumn("dbo.Participants", "AllocationBlockId", c => c.Int(nullable: false));
            CreateIndex("dbo.Participants", "AllocationBlockId");
            AddForeignKey("dbo.Participants", "AllocationBlockId", "dbo.AllocationBlocks", "Id", cascadeDelete: false);
            DropColumn("dbo.Participants", "BlockNumber");
            DropColumn("dbo.Participants", "BlockSize");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Participants", "BlockSize", c => c.Int(nullable: false));
            AddColumn("dbo.Participants", "BlockNumber", c => c.Int());
            DropForeignKey("dbo.Participants", "AllocationBlockId", "dbo.AllocationBlocks");
            DropIndex("dbo.Participants", new[] { "AllocationBlockId" });
            AlterColumn("dbo.Participants", "AllocationBlockId", c => c.Int());
            CreateIndex("dbo.Participants", "AllocationBlockId");
            AddForeignKey("dbo.Participants", "AllocationBlockId", "dbo.AllocationBlocks", "Id");
        }
    }
}
