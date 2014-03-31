using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using BlowTrial.Infrastructure.Extensions;
using System.Data.Entity;
using System.Data.Common;
using System.Data.SqlServerCe;
using BlowTrial.Domain.Providers;

namespace BlowTrial.Migrations
{
    public static class CodeBasedMigration
    {
        static int ApplySqlCeCommand(string ceConnectionString, string sqlCommand)
        {
            int returnVar;
            using (SqlCeConnection conn = new SqlCeConnection(ceConnectionString))
            {
                SqlCeCommand cmd = new SqlCeCommand(sqlCommand, conn);
                conn.Open();
                returnVar = cmd.ExecuteNonQuery();
                conn.Close();
            }
            return returnVar;
        }
        static int MoveTrialDataToExplicitMigrations(string connectionString)
        {
            return ApplySqlCeCommand(connectionString,
                "UPDATE [__MigrationHistory] SET [MigrationId] = '201403062253529_InitialTrialData',[ContextKey] = 'BlowTrial.Migrations.TrialData.TrialDataConfiguration' WHERE ContextKey = 'BlowTrial.Migrations.TrialDataConfiguration';");
        }
        static int MoveMembershipDataToExplicitMigrations(string connectionString)
        {
            return ApplySqlCeCommand(connectionString,
                "UPDATE [__MigrationHistory] SET [MigrationId] = '201403062252244_InitialMembership',[ContextKey] = 'BlowTrial.Migrations.Membership.MembershipConfiguration' WHERE ContextKey = 'BlowTrial.Migrations.MembershipConfiguration';");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="providerInvariantName"></param>
        /// <returns>whether migrations were applied</returns>
        public static bool ApplyPendingMigrations<T>(string connectionString, string providerInvariantName, bool createIfNotExists = false)
            where T : DbMigrationsConfiguration, new()
        {
            bool returnVar = false;
            if (!createIfNotExists || !CreateDatabaseIfRequired(connectionString))
            {
                returnVar = MoveFromAutoToExplicitMigrations(typeof(T), connectionString);
            }
            
            var configuration = new T();
            configuration.TargetDatabase = new DbConnectionInfo(connectionString, providerInvariantName);
            var migrator = new DbMigrator(configuration);
            if (migrator.GetPendingMigrations().Any())
            {
                migrator.Update();
                returnVar = true;
            }
            return returnVar;
        }

        private static readonly object _lock = new object();
       static bool CreateDatabaseIfRequired(string connection)
        {
            lock (_lock)
            {
                SqlCeConnectionStringBuilder builder = new SqlCeConnectionStringBuilder(connection);

                if (!System.IO.File.Exists(builder.DataSource))
                {
                    //OK, try to create the database file
                    using (var engine = new SqlCeEngine(connection))
                    {
                        engine.CreateDatabase();
                    }
                    return true;
                }
                return false;
            }
        }

        static bool MoveFromAutoToExplicitMigrations(Type T, string connectionString)
        {
            if (T == typeof(BlowTrial.Migrations.Membership.MembershipConfiguration))
            {
                return MoveMembershipDataToExplicitMigrations(connectionString) != 0;
            }
            if (T == typeof(BlowTrial.Migrations.TrialData.TrialDataConfiguration))
            {
                return MoveTrialDataToExplicitMigrations(connectionString) != 0;
            }
            return false;
        }

        //namespacing changed to facilitate migrations - this returns to prior namespacing
        static string GetConfigurationKey(Type T)
        {
            if (T==typeof(BlowTrial.Migrations.Membership.MembershipConfiguration))
            {
                return "BlowTrial.Migrations.MembershipConfiguration";
            }
            if (T == typeof(BlowTrial.Migrations.TrialData.TrialDataConfiguration))
            {
                return "BlowTrial.Migrations.TrialDataConfiguration";
            }
            return T.ToString();
        }
    }
}
