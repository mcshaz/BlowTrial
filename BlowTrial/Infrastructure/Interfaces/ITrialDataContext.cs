using BlowTrial.Domain.Tables;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace BlowTrial.Domain.Interfaces
{
    public interface ITrialDataContext
    {
        DbSet<Vaccine> Vaccines { get; set; }
        DbSet<VaccineAdministered> VaccinesAdministered { get; set; }
        DbSet<Participant> Participants { get; set; }
        DbSet<ScreenedPatient> ScreenedPatients { get; set; }
        DbSet<ProtocolViolation> ProtocolViolations { get; set; }
        Database Database { get; }
        String DbBackupPath { get; }
        void Dispose();
        int SaveChanges();
    }
}
