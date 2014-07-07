namespace BlowTrial.Migrations.TrialData
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialTrialData : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Participants",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(maxLength: 256),
                        MothersName = c.String(maxLength: 256),
                        PhoneNumber = c.String(maxLength: 16),
                        IsInterventionArm = c.Boolean(nullable: false),
                        BcgAdverse = c.Boolean(),
                        BcgAdverseDetail = c.String(maxLength: 2056),
                        BcgPapuleAtDischarge = c.Boolean(),
                        LastContactWeight = c.Int(),
                        LastWeightDate = c.DateTime(),
                        DischargeDateTime = c.DateTime(),
                        DeathOrLastContactDateTime = c.DateTime(),
                        OtherCauseOfDeathDetail = c.String(maxLength: 2056),
                        BlockNumber = c.Int(nullable: false),
                        BlockSize = c.Int(nullable: false),
                        MultipleSiblingId = c.Int(),
                        CauseOfDeath = c.Int(nullable: false),
                        OutcomeAt28Days = c.Int(nullable: false),
                        HospitalIdentifier = c.String(maxLength: 128),
                        Abnormalities = c.String(maxLength: 512),
                        AdmissionWeight = c.Int(nullable: false),
                        GestAgeBirth = c.Double(nullable: false),
                        IsMale = c.Boolean(nullable: false),
                        DateTimeBirth = c.DateTime(nullable: false),
                        RegisteredAt = c.DateTime(nullable: false),
                        CentreId = c.Int(nullable: false),
                        RegisteringInvestigator = c.String(maxLength: 64),
                        RecordLastModified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StudyCentres", t => t.CentreId, cascadeDelete: true)
                .Index(t => t.CentreId);
            
            CreateTable(
                "dbo.StudyCentres",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        DuplicateIdCheck = c.Guid(nullable: false),
                        Name = c.String(maxLength: 128),
                        ArgbTextColour = c.Int(nullable: false),
                        ArgbBackgroundColour = c.Int(nullable: false),
                        PhoneMask = c.String(maxLength: 16),
                        HospitalIdentifierMask = c.String(maxLength: 16),
                        MaxIdForSite = c.Int(nullable: false),
                        RecordLastModified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProtocolViolations",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        ParticipantId = c.Int(nullable: false),
                        Details = c.String(maxLength: 4000),
                        MajorViolation = c.Boolean(nullable: false),
                        ReportingInvestigator = c.String(maxLength: 4000),
                        ReportingTimeLocal = c.DateTime(nullable: false),
                        RecordLastModified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Participants", t => t.ParticipantId, cascadeDelete: true)
                .Index(t => t.ParticipantId);
            
            CreateTable(
                "dbo.VaccinesAdministered",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        VaccineId = c.Int(nullable: false),
                        ParticipantId = c.Int(nullable: false),
                        RecordLastModified = c.DateTime(nullable: false),
                        AdministeredAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Participants", t => t.ParticipantId, cascadeDelete: true)
                .ForeignKey("dbo.Vaccines", t => t.VaccineId, cascadeDelete: true)
                .Index(t => t.ParticipantId)
                .Index(t => t.VaccineId);
            
            CreateTable(
                "dbo.Vaccines",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(maxLength: 4000),
                        RecordLastModified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ScreenedPatients",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        LikelyDie24Hr = c.Boolean(nullable: false),
                        BadMalform = c.Boolean(nullable: false),
                        BadInfectnImmune = c.Boolean(nullable: false),
                        WasGivenBcgPrior = c.Boolean(nullable: false),
                        RefusedConsent = c.Boolean(),
                        Missed = c.Boolean(),
                        HospitalIdentifier = c.String(maxLength: 128),
                        Abnormalities = c.String(maxLength: 512),
                        AdmissionWeight = c.Int(nullable: false),
                        GestAgeBirth = c.Double(nullable: false),
                        IsMale = c.Boolean(nullable: false),
                        DateTimeBirth = c.DateTime(nullable: false),
                        RegisteredAt = c.DateTime(nullable: false),
                        CentreId = c.Int(nullable: false),
                        RegisteringInvestigator = c.String(maxLength: 64),
                        RecordLastModified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StudyCentres", t => t.CentreId, cascadeDelete: true)
                .Index(t => t.CentreId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ScreenedPatients", "CentreId", "dbo.StudyCentres");
            DropForeignKey("dbo.VaccinesAdministered", "VaccineId", "dbo.Vaccines");
            DropForeignKey("dbo.VaccinesAdministered", "ParticipantId", "dbo.Participants");
            DropForeignKey("dbo.ProtocolViolations", "ParticipantId", "dbo.Participants");
            DropForeignKey("dbo.Participants", "CentreId", "dbo.StudyCentres");
            DropIndex("dbo.ScreenedPatients", new[] { "CentreId" });
            DropIndex("dbo.VaccinesAdministered", new[] { "VaccineId" });
            DropIndex("dbo.VaccinesAdministered", new[] { "ParticipantId" });
            DropIndex("dbo.ProtocolViolations", new[] { "ParticipantId" });
            DropIndex("dbo.Participants", new[] { "CentreId" });
            DropTable("dbo.ScreenedPatients");
            DropTable("dbo.Vaccines");
            DropTable("dbo.VaccinesAdministered");
            DropTable("dbo.ProtocolViolations");
            DropTable("dbo.StudyCentres");
            DropTable("dbo.Participants");
        }
    }
}
