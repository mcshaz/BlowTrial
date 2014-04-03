namespace BlowTrial.Migrations.TrialData
{
    using BlowTrial.Domain.Tables;
    using BlowTrial.Infrastructure;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TrialDataV2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Participants", "Notes", c => c.String(maxLength: 160));
            AddColumn("dbo.Participants", "WasEnvelopeRandomised", c => c.Boolean(nullable: false));
            Sql(string.Format("UPDATE [Participants] SET [WasEnvelopeRandomised] = 1 WHERE [Id] <= {0} OR [MultipleSiblingId] <= {0};", EnvelopeDetails.MaxEnvelopeNumber));
            AddColumn("dbo.Participants", "Inborn", c => c.Boolean());
            AddColumn("dbo.ProtocolViolations", "ViolationType", c => c.Int(nullable: false, defaultValue: (int?)ViolationTypeOption.Minor));
            Sql(string.Format("UPDATE [ProtocolViolations] SET [ViolationType] = {0} WHERE [Details] LIKE 'Randomised incorrectly%'", (int)ViolationTypeOption.MajorWrongAllocation));
            Sql(string.Format("UPDATE [ProtocolViolations] SET [ViolationType] = {0} WHERE [Details] LIKE '%Receive%'", (int)ViolationTypeOption.MajorWrongTreatment));
            Sql(string.Format("UPDATE [ProtocolViolations] SET [ViolationType] = {0} WHERE  [ViolationType] <> {1} AND [MajorViolation] = 1", (int)ViolationTypeOption.MajorOther, (int)ViolationTypeOption.MajorWrongAllocation));
            AddColumn("dbo.ScreenedPatients", "Inborn", c => c.Boolean());
            AlterColumn("dbo.Participants", "BlockNumber", c => c.Int());
            //Sql("ALTER TABLE [Participants] ALTER COLUMN [BlockNumber] INTEGER NULL;");
            DropColumn("dbo.ProtocolViolations", "MajorViolation");
            Sql("UPDATE [Participants] SET [PhoneNumber] = null WHERE SUBSTRING([PhoneNumber],LEN([PhoneNumber])-8,8) = '99999999';");
            AddColumn("dbo.Participants", "AdmissionDiagnosis", c => c.String(maxLength: 512));
            Sql("UPDATE [Participants] SET [AdmissionDiagnosis] = [Abnormalities];");
            DropColumn("dbo.Participants", "Abnormalities");
            AddColumn("dbo.ScreenedPatients", "AdmissionDiagnosis", c => c.String(maxLength: 512));
            Sql("UPDATE [ScreenedPatients] SET [AdmissionDiagnosis] = [Abnormalities];");
            DropColumn("dbo.ScreenedPatients", "Abnormalities");
            AddColumn("dbo.ScreenedPatients", "AppVersionAtEnrollment", c => c.Int(nullable: false));
            AddColumn("dbo.Participants", "AppVersionAtEnrollment", c => c.Int(nullable: false));
            /*
            CreateIndex("dbo.Participants", "CentreId");
            CreateIndex("dbo.ProtocolViolations", "ParticipantId");
            CreateIndex("dbo.VaccinesAdministered", "VaccineId");
            CreateIndex("dbo.VaccinesAdministered", "ParticipantId");
            CreateIndex("dbo.ScreenedPatients", "CentreId");
             * */
        }

        public override void Down()
        {
            AddColumn("dbo.ScreenedPatients", "Abnormalities", c => c.String(maxLength: 512));
            Sql("UPDATE [ScreenedPatients] SET [Abnormalities] = [AdmissionDiagnosis]");
            AddColumn("dbo.ProtocolViolations", "MajorViolation", c => c.Boolean(nullable: false));
            Sql(string.Format("UPDATE dbo.ProtocolViolations SET MajorViolation = 1 WHERE WasEnvelopeRandomised <> {0};", (int)ViolationTypeOption.Minor));
            AddColumn("dbo.Participants", "Abnormalities", c => c.String(maxLength: 512));
            Sql("UPDATE [Participants] SET [Abnormalities] = [AdmissionDiagnosis];");
            Sql("UPDATE [Participants] SET [BlockNumber] = 0 WHERE [BlockNumber] IS NULL;");
            AlterColumn("dbo.Participants", "BlockNumber", c => c.Int(nullable: false));
            DropColumn("dbo.ScreenedPatients", "Inborn");
            DropColumn("dbo.ScreenedPatients", "AdmissionDiagnosis");
            DropColumn("dbo.ProtocolViolations", "ViolationType");
            DropColumn("dbo.Participants", "Inborn");
            DropColumn("dbo.Participants", "AdmissionDiagnosis");
            DropColumn("dbo.Participants", "WasEnvelopeRandomised");
            DropColumn("dbo.Participants", "Notes");
            DropColumn("dbo.ScreenedPatients", "AppVersionAtEnrollment");
            DropColumn("dbo.Participants", "AppVersionAtEnrollment");
            /*
            DropIndex("dbo.ScreenedPatients", new[] { "CentreId" });
            DropIndex("dbo.VaccinesAdministered", new[] { "ParticipantId" });
            DropIndex("dbo.VaccinesAdministered", new[] { "VaccineId" });
            DropIndex("dbo.ProtocolViolations", new[] { "ParticipantId" });
            DropIndex("dbo.Participants", new[] { "CentreId" });
             * */
        }
    }
}
