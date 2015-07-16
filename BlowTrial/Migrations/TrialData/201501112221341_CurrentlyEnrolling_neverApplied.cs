namespace BlowTrial.Migrations.TrialData
{
    using Domain.Tables;
    using System.Data.Entity.Migrations;
    
    public partial class CurrentlyEnrolling : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StudyCentres", "IsCurrentlyEnrolling", c => c.Boolean(nullable: false, defaultValue:true));
            AddColumn("dbo.StudyCentres", "DefaultAllocation", c => c.Int(nullable: false,defaultValue: (int)AllocationGroups.India2Arm));
            AddColumn("dbo.StudyCentres", "IsOpvInIntervention", c => c.Boolean(nullable: false,defaultValue:true));
            AddColumn("dbo.StudyCentres", "IsToHospitalDischarge", c => c.Boolean(nullable: false,defaultValue:true));
            Sql("update StudyCentres set IsOpvInIntervention=0, IsToHospitalDischarge=0 where Id=1");
            Sql("update StudyCentres set Name=[Name] + ' Indian BCG'");

            Sql("INSERT INTO [StudyCentres] ([Id],[DuplicateIdCheck],[Name],[ArgbTextColour],[ArgbBackgroundColour],[PhoneMask],[HospitalIdentifierMask],[MaxIdForSite],[RecordLastModified],[IsCurrentlyEnrolling],[DefaultAllocation],[IsOpvInIntervention],[IsToHospitalDischarge]) (SELECT 30000,'23621e38-7efb-4422-a4bc-9c42911d2050',N'GMH (Chennai inborn) Danish BCG',255,-589505281,N'999-9000-0000',N'>9000000',39999,{ts '2015-01-13 02:55:15.850'},0,5,1,1 WHERE EXISTS (Select Id From [StudyCentres] WHERE Id=1))");
            Sql("INSERT INTO [StudyCentres] ([Id],[DuplicateIdCheck],[Name],[ArgbTextColour],[ArgbBackgroundColour],[PhoneMask],[HospitalIdentifierMask],[MaxIdForSite],[RecordLastModified],[IsCurrentlyEnrolling],[DefaultAllocation],[IsOpvInIntervention],[IsToHospitalDischarge]) (SELECT 40000,'91546be7-92a4-4022-bac3-268857572dea',N'ICH (Chennai outborn) Danish BCG',255,-840171521,N'999-9000-0000',N'>9000000',49999,{ts '2015-01-13 02:55:15.850'},0,5,1,1 WHERE EXISTS (Select Id From [StudyCentres] WHERE Id=1))");
            Sql("INSERT INTO [StudyCentres] ([Id],[DuplicateIdCheck],[Name],[ArgbTextColour],[ArgbBackgroundColour],[PhoneMask],[HospitalIdentifierMask],[MaxIdForSite],[RecordLastModified],[IsCurrentlyEnrolling],[DefaultAllocation],[IsOpvInIntervention],[IsToHospitalDischarge]) (SELECT 50000,'ca3144d2-a725-45f1-bd5a-3fe1b820412b',N'JIPMER (Pondicherry) Danish BCG',255,-876544001,N'999-9000-000',N'>L000000',59999,{ts '2015-01-13 02:55:15.850'},0,5,1,1 WHERE EXISTS (Select Id From [StudyCentres] WHERE Id=20000))");
        }
        
        public override void Down()
        {
            DropColumn("dbo.StudyCentres", "IsToHospitalDischarge");
            DropColumn("dbo.StudyCentres", "IsOpvInIntervention");
            DropColumn("dbo.StudyCentres", "DefaultAllocation");
            DropColumn("dbo.StudyCentres", "IsCurrentlyEnrolling");
        }
    }
}
