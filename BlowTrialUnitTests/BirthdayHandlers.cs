using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlowTrial.Infrastructure;
using BlowTrial.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using BlowTrial.Models;
using System.Threading.Tasks;
using Moq;
using BlowTrial.Infrastructure.Interfaces;
using System.Threading;
using BlowTrial.Infrastructure.Extensions;

namespace BlowTrialUnitTests
{
    [TestClass]
    public class BirthdayHandlers
    {
        [TestMethod]
        public void TestBirthdayHandlers()
        {
            const int participantCount = 4;

            TimeSpan[] birthTimes = new TimeSpan[] {
                TimeSpan.FromSeconds(-30),
                TimeSpan.FromSeconds(30),
                TimeSpan.FromHours(23),
                TimeSpan.FromHours(-23)
            };

            var expectedTimesFromNow = new List<TimeSpan>(birthTimes
                .Map(i => (i.Ticks > 0) ? i : i + TimeSpan.FromHours(24)));
            expectedTimesFromNow.Sort();

            var participants = new ParticipantListItemViewModel[participantCount];

            var moqArgs = new MoqTimerEventArgs();

            DateTime birthDayStart = DateTime.Now - TimeSpan.FromHours(24);

            for (int i = 0; i < participantCount; i++)
            {
                var p = participants[i] = new ParticipantListItemViewModel(
                    new ParticipantBaseModel 
                    { 
                        Id = i, 
                        DateTimeBirth = birthDayStart + birthTimes[i] 
                    } );
                p.PropertyChanged += (s, e) =>
                {
                    var returnedP = (ParticipantListItemViewModel)s;
                    Console.WriteLine("Id:{0}, propertyName:{1},DOB:{2:dd/MM hh:mm}, daysOld:{3}, called:{4:dd/MM hh:mm}", p.Id, e.PropertyName, p.DateTimeBirth, p.AgeDays, moqArgs.StartAt);
                };
            }
            var mockTimer = new Mock<IDispatcherTimer>();
            mockTimer.SetupProperty(m => m.Interval);
            IDispatcherTimer timer = mockTimer.Object;
            moqArgs.StartAt = DateTime.Now;
            var x = new AgeUpdatingService(participants, timer);
            TimeSpan lastTime = new TimeSpan(0);
            
            for (int i = 0; i < participantCount; i++ )
            {
                TimeSpan nextInterval = expectedTimesFromNow[i] - lastTime;
                IsInInterval(timer.Interval, nextInterval);
                moqArgs.StartAt += nextInterval;
                mockTimer.Raise(m => m.Tick += null, moqArgs);
                lastTime = expectedTimesFromNow[i];
            }

        }

        static void IsInInterval(TimeSpan actualInterval, TimeSpan expectedInterval)
        {
            const int allowedSecs = 5;
            TimeSpan minInterval = expectedInterval;
            TimeSpan maxInterval = expectedInterval + TimeSpan.FromSeconds(allowedSecs);
            Console.WriteLine(@"interval was {0:hh\:mm\:ss} (expected {1:hh\:mm\:ss})", actualInterval, expectedInterval);
            Assert.IsTrue(minInterval < actualInterval && actualInterval < maxInterval, @"interval was {0:hh\:mm\:ss}, should be {1:hh\:mm\:ss} (+{2}sec)", actualInterval, expectedInterval, allowedSecs);
        }
    }
}
