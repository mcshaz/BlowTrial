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
            var cmd = Command.GetCommand(args);
            Dictionary<string, string[]> argList = cmd.Instructions.ToDictionary(i => i.OptionName ?? "a", i => i.OptionValues);
            string directory = (argList.ContainsKey("directory"))
                ?string.Join(" ", argList["directory"])
                :null;
            switch (cmd.Name.ToLowerInvariant())
            {
                case "comparevaccinelist":
                    MakeComparisonList(directory);
                    break;
                case "updatevaccines":
                    UpdateVA(directory + '\\' + argList["source"], directory + '\\' + argList["dest"]);
                    break;
                case "compareparticipants":
                    var files = argList["files"];
                    Compare2DBParticipantId(directory + '\\' + files[0], directory + '\\' + files[1]);
                    break;
                case "comparenames":
                    CompareParticipantId.CompareDb(args[1], args[2]);
                    break;
                default:
                    Console.WriteLine("Command not found");
                    break;
            }
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

        static void Compare2DBVaccines(string dbFilename1, string dbFilename2)
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Join("\t", new string[] { "Copies", "Id", "ParticipantId", "VaccineId", "AdministeredAt", "RecordLastModified" }));
            foreach (var grp in VaccineDiff(dbFilename1, dbFilename2))
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
        static void Compare2DBParticipantId(string dbFilename1, string dbFilename2)
        {
            var dif = ParticipantDiff(dbFilename1, dbFilename2);
            Console.WriteLine("found in '{0}' only:", dbFilename1);
            Console.WriteLine(string.Join(",", dif[true]));
            Console.WriteLine();
            Console.WriteLine("found in '{0}' only:", dbFilename2);
            Console.WriteLine(string.Join(",", dif[false]));
            Console.ReadLine();
        }

        static ILookup<int,List<VaccineAdministered>> VaccineDiff(string db1Filename, string db2Filename)
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

        static Dictionary<bool, int[]> ParticipantDiff(string db1Filename, string db2Filename)
        {
            int[] participants1;
            int[] centres1;
            using (var db = new TrialDataContext(db1Filename))
            {
                participants1 = db.Participants.Select(p=>p.Id).ToArray();
                centres1 = db.StudyCentres.Select(c => c.Id).ToArray();
            }
            int[] participants2;
            using (var db = new TrialDataContext(db2Filename))
            {
                participants2 =(from p in db.Participants
                                where centres1.Contains(p.CentreId)
                                select p.Id).ToArray();
            }
            var returnVar = new Dictionary<bool, int[]>();
            returnVar.Add(true, participants1.Where(p => !participants2.Contains(p)).ToArray());
            returnVar.Add(false, participants2.Where(p => !participants1.Contains(p)).ToArray());
            return returnVar;

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
