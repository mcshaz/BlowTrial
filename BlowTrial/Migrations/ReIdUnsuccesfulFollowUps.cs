using BlowTrial.Domain.Providers;
using BlowTrial.Domain.Tables;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlowTrial.Migrations
{
    internal static class ReIdUnsuccesfulFollowUps
    {
        public static void Process(TrialDataContext repo)
        {
            var needsRedo = NeedsRedo(repo);
            if (needsRedo.Any())
            {
                repo.Database.ExecuteSqlCommand("ALTER TABLE [UnsuccessfulFollowUps] DROP CONSTRAINT [PK_dbo.UnsuccessfulFollowUps]");
                foreach (var g in needsRedo.GroupBy(u => u.TrialParticipant.Centre))
                {
                    var ids = g.Select(u => u.Id);
                    repo.Database.ExecuteSqlCommand($"UPDATE [UnsuccessfulFollowUps] SET [Id] = [Id] + {g.Key.Id} WHERE [Id] <= {ids.Max()} AND [Id]>= {ids.Min()}");
                }
                repo.Database.ExecuteSqlCommand("ALTER TABLE [UnsuccessfulFollowUps] ADD CONSTRAINT [PK_dbo.UnsuccessfulFollowUps] PRIMARY KEY(Id)");
            }
        }
        private static IQueryable<UnsuccessfulFollowUp> NeedsRedo(TrialDataContext repo)
        {
            var sites = repo.StudyCentres.ToList();
            var predicate = PredicateBuilder.New<UnsuccessfulFollowUp>();
            foreach (var s in sites)
            {
                predicate = predicate.Or(p => p.ParticipantId >= s.Id && p.ParticipantId <= s.MaxIdForSite && !(p.Id >= s.Id && p.Id <= s.MaxIdForSite));
            }
            return repo.UnsuccessfulFollowUps.AsExpandable().Where(predicate);
        }
    }
}
