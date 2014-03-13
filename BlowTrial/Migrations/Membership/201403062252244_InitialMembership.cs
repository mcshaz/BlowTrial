namespace BlowTrial.Migrations.Membership
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMembership : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BackupDatas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BackupIntervalMinutes = c.Int(nullable: false),
                        IsBackingUpToCloud = c.Boolean(nullable: false),
                        IsEnvelopeRandomising = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CloudDirectories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Path = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Investigators",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Username = c.String(maxLength: 64),
                        Password = c.String(maxLength: 128),
                        LastLoginAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RandomisingMessages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InterventionInstructions = c.String(maxLength: 200),
                        ControlInstructions = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RoleInvestigators",
                c => new
                    {
                        Role_Id = c.Int(nullable: false),
                        Investigator_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Role_Id, t.Investigator_Id })
                .ForeignKey("dbo.Roles", t => t.Role_Id, cascadeDelete: true)
                .ForeignKey("dbo.Investigators", t => t.Investigator_Id, cascadeDelete: true)
                .Index(t => t.Role_Id)
                .Index(t => t.Investigator_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RoleInvestigators", "Investigator_Id", "dbo.Investigators");
            DropForeignKey("dbo.RoleInvestigators", "Role_Id", "dbo.Roles");
            DropIndex("dbo.RoleInvestigators", new[] { "Investigator_Id" });
            DropIndex("dbo.RoleInvestigators", new[] { "Role_Id" });
            DropTable("dbo.RoleInvestigators");
            DropTable("dbo.RandomisingMessages");
            DropTable("dbo.Roles");
            DropTable("dbo.Investigators");
            DropTable("dbo.CloudDirectories");
            DropTable("dbo.BackupDatas");
        }
    }
}
