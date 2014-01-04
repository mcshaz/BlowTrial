using System.Data.Entity;
using System.Linq;

namespace BlowTrial.Infrastructure.FilesysServices
{
    public static class DbBackupService
    {
        private const string FullName = " FULL";
        private const string DifferentialName = " (dif)";
        public static int BackupDataBase(Database db, string dbName, string outputPath, bool differential=false)
        {
            if (!outputPath.EndsWith(".bak")) { outputPath += ".bak"; }
            int sqlReturn = db.ExecuteSqlCommand(
                    "BACKUP DATABASE [" + dbName + "] TO  DISK = N'" + outputPath + "' WITH " + (differential ? "DIFFERENTIAL," : "") + " CHECKSUM");
            if (sqlReturn != -1) { return sqlReturn; }
            return db.ExecuteSqlCommand("RESTORE VERIFYONLY FROM  DISK = N'" + outputPath + "'");
        }
        public static bool IsCompactEdition(Database db)
        {
            return SqlVersion(db) == SqlCeVersionString;
        }
        public const string SqlCeVersionString = "Sql Server Compact Edition (version unspecified)";
        public static string SqlVersion(Database db)
        {
            try
            {
                return db.SqlQuery(typeof(string), "SELECT @@version").Cast<string>().FirstOrDefault();
            }
            catch (System.Data.SqlServerCe.SqlCeException)
            {
                return SqlCeVersionString; // could return the data provider, ie 'Data Provider for Microsoft SQL Server Compact' with e.Source
            }
        }
    }
}