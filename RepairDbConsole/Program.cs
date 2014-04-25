using BlowTrial.Domain.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Windows;
using BlowTrial.Domain.Tables;

namespace RepairDbConsole
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var argList = Instruction.GetInstructions(args).ToDictionary(i=>i.OptionName,i=>i.OptionValues);
            //UpdateVA(argList["directory"] + '\\' + argList["source"], argList["directory"] + '\\' + argList["dest"]);
            MakeComparisonList(argList["directory"]);
        }
        
        static void MakeComparisonList(string directory)
        {
            string validationMessage = ValidateDirectory(directory);
            if (validationMessage != null)
            {
                Console.WriteLine(validationMessage);
                Console.ReadLine();
                return;
            }
            Console.Write("Beginning");
            var sb = new StringBuilder();
            sb.AppendLine(string.Join("\t", (new string[] { "FileName" }).Concat(typeof(DbSummary).GetAllReadablePropertyNames())));
            string[] relevantFiles = Directory.GetFiles(directory, "ParticipantData*.sdf");
            foreach (string f in relevantFiles)
            {
                sb.AppendLine(Path.GetFileName(f));
                foreach (var c in DbInfo(f))
                {
                    sb.AppendLine(string.Join("\t", (new string[] { "" }).Concat(c.GetAllPropertyToStrings())));
                    Console.Write('.');
                }
            }
            Clipboard.SetText(sb.ToString());
            Console.WriteLine();
            Console.WriteLine("tab seperated values written to clipboard. press enter to continue.");
            Console.ReadLine();
        }
        static string ValidateDirectory(string directory)
        {
            if (string.IsNullOrWhiteSpace(directory)) { return "Please enter a directory name"; }
            var invalidChars = Path.GetInvalidPathChars().Intersect(directory.ToCharArray());
            if (invalidChars.Any()) { return "The following invalid characters were found in the directory name: " + string.Join(",",invalidChars); }
            if (!Directory.Exists(directory)){return("Directory doesnt exist");}
            return null;
        }

        static void CompareFiles(string dbFilename1, string dbFilename2)
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Join("\t", new string[] { "Copies", "Id", "ParticipantId", "VaccineId", "AdministeredAt", "RecordLastModified" }));
            foreach (var grp in Diff(dbFilename1, dbFilename2))
            {
                sb.AppendLine(Path.GetFileName(grp.Key.ToString()));
                foreach (var match in grp)
                {
                    foreach (var va in match) 
                    {
                        sb.AppendLine(string.Join("\t", new string[] { "", va.Id.ToString(), va.ParticipantId.ToString(), va.VaccineId.ToString(), va.AdministeredAt.ToString(), va.RecordLastModified.ToString() }));
                        Console.Write('.');
                    }
                    sb.AppendLine();
                }
            }
            Clipboard.SetText(sb.ToString());
            Console.WriteLine();
            Console.WriteLine("tab seperated values written to clipboard. press enter to continue.");
            Console.ReadLine();
        }
        static ILookup<int,List<VaccineAdministered>> Diff(string db1Filename, string db2Filename)
        {
            IEnumerable<VaccineAdministered> va1;
            using (var db = new TrialDataContext(db1Filename))
            {
                va1 = db.VaccinesAdministered.AsNoTracking().ToList();
            }
            IEnumerable<VaccineAdministered> va2;
            using (var db = new TrialDataContext(db2Filename))
            {
                va2 = db.VaccinesAdministered.AsNoTracking().ToList();
            }
            return (from va in va1.Concat(va2).ToList()
                    group va by va.AdministeredAt.ToString() + va.VaccineId.ToString() into s
                    select s).Select(g=>g.ToList()).ToList().ToLookup(g=>g.Count);
                    
        }

        static void UpdateVA(string source, string dest)
        {
            List<VaccineAdministered> sourceVas;
            using (var db = new TrialDataContext(source))
            {
                sourceVas = db.VaccinesAdministered.AsNoTracking().ToList();
            }
            using (var db = new TrialDataContext(dest))
            {
                var destVas = db.VaccinesAdministered.ToList();
                int i=0;
                while (sourceVas.Count > i && destVas.Count > i && sourceVas[i].Id == destVas[i].Id)
                {
                    destVas[i].ParticipantId = sourceVas[i].ParticipantId;
                    i++;
                }
                db.SaveChanges();
            }

        }
        static IEnumerable<DbSummary> DbInfo(string filename)
        {
            List<DbSummary> returnVar = new List<DbSummary>();
            using(var db = new TrialDataContext(filename))
            {
                foreach (var studyCentre in db.StudyCentres.ToList())
                {
                    returnVar.Add(new DbSummary
                    {
                        CentreName = studyCentre.Name,
                        MostRecentDemographicUpdate = db.Participants.Where(p=>p.CentreId == studyCentre.Id).Max(p => p.RecordLastModified),
                        MostRecentEnrollment = db.Participants.Where(p => p.CentreId == studyCentre.Id).Max(p => p.RegisteredAt),
                        MostRecentVaccineAdmin = db.VaccinesAdministered.Where(va => va.Id >= studyCentre.Id && va.Id <= studyCentre.MaxIdForSite).Max(va => va.AdministeredAt),
                        MostRecentVaccineUpdate = db.VaccinesAdministered.Where(va => va.Id >= studyCentre.Id && va.Id <= studyCentre.MaxIdForSite).Max(va => va.RecordLastModified),
                        Participants = db.Participants.Where(p => p.CentreId == studyCentre.Id).Count(),
                        ParticipantsWithVaccinesAdmin = db.VaccinesAdministered.Where(va => va.Id >= studyCentre.Id && va.Id <= studyCentre.MaxIdForSite).GroupBy(va => va.ParticipantId).Count(),
                        TotalVaccinesAdmin = db.VaccinesAdministered.Where(va => va.Id >= studyCentre.Id && va.Id <= studyCentre.MaxIdForSite).Count()
                    });
                }
            }
            return returnVar;
        }

    }
}
