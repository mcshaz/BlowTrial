namespace BlowTrial.Migrations.TrialData
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VaccineNameLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Vaccines", "Name", c => c.String(maxLength: 16));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Vaccines", "Name", c => c.String(maxLength: 4000));
        }
    }
}
