using BlowTrial.Infrastructure;
using BlowTrial.Infrastructure.Interfaces;
using BlowTrial.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlowTrial.Helpers
{
    static class AgeUpdatingMediator
    {
        static AgeUpdatingService _updator;
        internal static AgeUpdatingService GetService(IRepository repo)
        {
            var minBirthday = from p in repo.Participants
                              let age = DbFunctions.DiffDays(p.DateTimeBirth, DateTime.Now)
                              where age <= 28 && !ParticipantBaseModel.KnownDeadOutcomes.Contains(p.OutcomeAt28Days)
                              select new MinBirthdayInfo { Id = p.Id, DateTimeBirth=p.DateTimeBirth };
            return GetService(minBirthday, minBirthday.Count()); 
        }
        internal static AgeUpdatingService GetService(IEnumerable<IBirthday> participants, int capacity=4)
        {
            if (_updator==null)
            {
                _updator = new AgeUpdatingService(participants: participants, capacity: capacity);
            }
            return _updator;
        }
        internal static void Cleanup()
        {
            if (_updator !=null)
            {
                _updator.Cleanup();
            }
        }
    }
    class MinBirthdayInfo : IBirthday
    {
        public int Id { get; set; }
        public DateTime DateTimeBirth { get; set; }
        public int AgeDays { get; set; }
    }
}
