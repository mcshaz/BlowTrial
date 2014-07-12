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
using System.Diagnostics;

namespace BlowTrialUnitTests
{
    [TestClass]
    public class BirthdayHandlers
    {
        public BirthdayHandlers()
        {
            shorter = longer = TimeSpan.FromSeconds(2);
        }
        readonly TimeSpan shorter;
        readonly TimeSpan longer;
        [TestMethod]
        public void TestAgeUpdatingSetInterval()
        {
            var moq = TestAgeUpdatingSetInterval(
                DateTime.Now - TimeSpan.FromDays(1),
                    TimeSpan.FromMinutes(-1),
                    TimeSpan.FromMinutes(1),
                    TimeSpan.FromHours(23),
                    TimeSpan.FromHours(-23));
            moq.AgeService.Cleanup();
            moq.Moq.Verify(m => m.Stop(), Times.Once);
        }

        [TestMethod]
        public void TestAgeUpdatingAsParticipantsExceed28()
        {
            var moq = TestAgeUpdatingSetInterval(
                DateTime.Now - TimeSpan.FromDays(29),20,15,5,15);
            moq.Moq.Verify(m => m.Stop(), Times.AtLeastOnce);
        }

        [TestMethod]
        public void TestAgeUpdatingAllParticipantsEarlierInDay() //assuming not run within 20 secs of midnight!
        {
            var moq = TestAgeUpdatingSetInterval(DateTime.Now.Date ,20,15,5,15 );
        }

        class MyMoq
        {
            public Mock<IDispatcherTimer> Moq { get; set; }
            public AgeUpdatingService AgeService { get; set; }
        }

        static DateTime[] MapAroundDate(DateTime date, params int[] minsAround)
        {
            return MapAroundDate(date, minsAround.Map(a => TimeSpan.FromMinutes(a)));
        }

        static DateTime[] MapAroundDate(DateTime date, params TimeSpan[] around)
        {
            return around.Map(a => date + a);
        }

        MyMoq TestAgeUpdatingSetInterval(DateTime date, params int[] around)
        {
            return TestAgeUpdatingSetInterval(MapAroundDate(date, around));
        }

        MyMoq TestAgeUpdatingSetInterval(DateTime date, params TimeSpan[] around)
        {
            return TestAgeUpdatingSetInterval(MapAroundDate(date, around));
        }

        MyMoq TestAgeUpdatingSetInterval(params DateTime[] birthDateTimes)
        {
            if (birthDateTimes == null || birthDateTimes.Length < 2)
            {
                throw new ArgumentException("must include at least 2 birthDateTimes");
            }
            var participants = new List<ParticipantListItemViewModel>(birthDateTimes.Length);

            var moqArgs = new MoqTimerEventArgs();

            for (int i = 0; i < birthDateTimes.Length; i++)
            {
                var p = new ParticipantListItemViewModel(
                    new ParticipantBaseModel 
                    { 
                        Id = i + 1, 
                        DateTimeBirth = birthDateTimes[i] 
                    } );
                p.PropertyChanged += (s, e) =>
                {
                    var returnedP = (ParticipantListItemViewModel)s;
                    Console.WriteLine("Id:{0}, DOB:{1:dd/MM hh:mm}, daysOld:{2}, called:{3:dd/MM hh:mm}", p.Id, p.DateTimeBirth, p.AgeDays, moqArgs.StartAt);
                };
                participants.Add(p);
            }

            var expectedTimesFromNow = TimesFromNow(birthDateTimes, moqArgs.StartAt);

            var mockTimer = new Mock<IDispatcherTimer>(MockBehavior.Strict);
            mockTimer.SetupProperty(m => m.Interval);
            mockTimer.Setup(m => m.Start());
            mockTimer.Setup(m => m.Stop());
            TimeSpan expectedInterval = expectedTimesFromNow[0];
            TimeSpan tolerance = TimeSpan.FromSeconds(defaultSecsTolerance);
            bool requiresInterval = true;
            mockTimer.SetupSet(m => m.Interval = It.IsAny<TimeSpan>()).Callback<TimeSpan>(i =>
            {
                string usrMsg = string.Format(intervalLogTemplate, i, expectedInterval, tolerance);
                if (requiresInterval && (i < expectedInterval - tolerance || i > expectedInterval + tolerance))
                {
                    throw new ArgumentOutOfRangeException("Interval", usrMsg);
                }
                else
                {
                    Console.WriteLine(usrMsg);
                }
            });
            IDispatcherTimer timer = mockTimer.Object;
            Console.WriteLine("instantiating AgeUpdatingService");
            var ageService = new AgeUpdatingService(participants, timer);
            Console.WriteLine("finished instantiation");
            mockTimer.Verify(m => m.Start(), Times.AtLeastOnce);
            
            Action<TimeSpan, TimeSpan> validateIntervals = new Action<TimeSpan,TimeSpan>((last, curr) => {
                if (curr.Ticks > 0)
                {
                    moqArgs.StartAt += last;
                    expectedInterval = curr;
                    mockTimer.Raise(m => m.Tick += null, moqArgs);
                }
            });
            moqArgs.StartAt += expectedTimesFromNow.First(); //will have been set during instantiation
            expectedTimesFromNow.AggregatePairSelect((prev, cur)=>cur-prev)
                .Where(t=>t.Ticks != 0)
                .AggregatePairForEach(new TimeSpan(), validateIntervals);
            requiresInterval = false;
            mockTimer.Raise(m => m.Tick += null, moqArgs);

            Console.WriteLine("Adding new participants");
            foreach (var addedInterval in (new int[]{10,-10}).Select(i=>TimeSpan.FromSeconds(i)))
            {
                participants.Add(new ParticipantListItemViewModel(
                        new ParticipantBaseModel
                        {
                            Id = participants.Count + 1,
                            DateTimeBirth = moqArgs.StartAt - TimeSpan.FromDays(1)
                        }));
                ageService.AddParticipant(participants[participants.Count-1]);
            }
            requiresInterval = true;

            moqArgs.StartAt = DateTime.Now;
            expectedTimesFromNow = TimesFromNow(from p in participants where p.AgeDays <= 28 select p.DateTimeBirth, moqArgs.StartAt);

            expectedTimesFromNow.AggregatePairSelect(new TimeSpan(),(prev, cur) => cur - prev)
                .Where(t=>t.Ticks!=0)
                .AggregatePairForEach(new TimeSpan(), validateIntervals);

            requiresInterval = false;
            mockTimer.Raise(m => m.Tick += null, moqArgs);

            return new MyMoq { AgeService = ageService, Moq = mockTimer };
        }

        static List<TimeSpan> TimesFromNow(IEnumerable<DateTime> birthDateTimes, DateTime now)
        {
            TimeSpan currentTime = now.TimeOfDay;
            TimeSpan oneDay = TimeSpan.FromDays(1);
            var expectedTimesFromNow = new List<TimeSpan>(birthDateTimes
                .Select(dob =>
                {
                    if (dob > now) { throw new ArgumentException("all dates must be before now"); }
                    var timeDif = dob.TimeOfDay - currentTime;
                    if (timeDif.Ticks < 0) { timeDif += oneDay; }
                    return timeDif;
                }));
            expectedTimesFromNow.Sort();
            return expectedTimesFromNow;
        }
        [TestMethod]
        public void TestAgeUpdatingStartOnAdd()
        {
            var moqArgs = new MoqTimerEventArgs();
            var mockTimer = new Mock<IDispatcherTimer>(MockBehavior.Strict);
            mockTimer.SetupProperty(m => m.Interval);
            mockTimer.Setup(m => m.Start()).Verifiable();
            mockTimer.Setup(m => m.Stop()).Verifiable();
            IDispatcherTimer timer = mockTimer.Object;

            var ageService = new AgeUpdatingService(new ParticipantListItemViewModel[0], timer);
            mockTimer.Verify(m => m.Start(), Times.Never);
            
            TimeSpan toDob = TimeSpan.FromMinutes(-10);
            var p = new ParticipantListItemViewModel(
                new ParticipantBaseModel
                {
                    Id = 1,
                    DateTimeBirth = moqArgs.StartAt + toDob
                });

            TimeSpan expectedInterval = TimeSpan.FromDays(1) +toDob;
            TimeSpan tolerance = TimeSpan.FromSeconds(defaultSecsTolerance);
            mockTimer.SetupSet(m => m.Interval = It.IsAny<TimeSpan>()).Callback<TimeSpan>(i =>
            {
                string usrMsg = string.Format(intervalLogTemplate, i, expectedInterval, tolerance);
                if (i < expectedInterval - tolerance || i > expectedInterval + tolerance)
                {
                    throw new ArgumentOutOfRangeException("Interval", usrMsg);
                }
                else
                {
                    Console.WriteLine(usrMsg);
                }
            });
            ageService.AddParticipant(p);
            mockTimer.Verify(m => m.Start(), Times.Once);

            TimeSpan laterDayversary = TimeSpan.FromMinutes(-5);
            p = new ParticipantListItemViewModel(
                new ParticipantBaseModel
                {
                    Id = 2,
                    DateTimeBirth = moqArgs.StartAt + laterDayversary
                });
            ageService.AddParticipant(p);

            toDob = TimeSpan.FromMinutes(-30);
            p = new ParticipantListItemViewModel(
                new ParticipantBaseModel
                {
                    Id = 2,
                    DateTimeBirth = moqArgs.StartAt + toDob
                });
            expectedInterval = TimeSpan.FromDays(1) + toDob;

            ageService.AddParticipant(p);

            toDob = TimeSpan.FromDays(-1).Add(TimeSpan.FromMinutes(5));
            p = new ParticipantListItemViewModel(
                new ParticipantBaseModel
                {
                    Id = 2,
                    DateTimeBirth = moqArgs.StartAt + toDob
                });
            expectedInterval = TimeSpan.FromDays(1) + toDob;

            ageService.AddParticipant(p);
        }
        const int defaultSecsTolerance = 2;
        const string intervalLogTemplate = @"interval was {0:hh\:mm\:ss}, expected {1:hh\:mm\:ss} (+/-{2}sec)";
    }
}
