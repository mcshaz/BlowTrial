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
            moq.Moq.VerifySet(m => m.Stop(), Times.Once);
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
            if (birthDateTimes == null || birthDateTimes.Length == 0)
            {
                throw new ArgumentException("must include birthDateTimes");
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

            var expectedTimesFromNow = TimesFromNow(birthDateTimes);

            var mockTimer = new Mock<IDispatcherTimer>();
            mockTimer.SetupProperty(m => m.Interval);
            mockTimer.Setup(m => m.Start());
            mockTimer.Setup(m => m.Stop());
            mockTimer.SetupSet(m => m.Interval = It.IsInRange(
                expectedTimesFromNow[0] - shorter, expectedTimesFromNow[0] + longer, Range.Inclusive));
            IDispatcherTimer timer = mockTimer.Object;
            moqArgs.StartAt = DateTime.Now;
            Console.WriteLine("instantiating AgeUpdatingService");
            var ageService = new AgeUpdatingService(participants, timer);
            LogInterval(timer.Interval, expectedTimesFromNow[0]);
            Console.WriteLine("finished instantiation");
            mockTimer.Verify(m => m.Start(), Times.AtLeastOnce);

            EnumerateAndValidateIntervals(expectedTimesFromNow.Skip(1), timer, moqArgs, mockTimer);

            // test add
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

            moqArgs.StartAt = DateTime.Now;
            expectedTimesFromNow = TimesFromNow(from p in participants where p.AgeDays <= 28 select p.DateTimeBirth);

            EnumerateAndValidateIntervals(expectedTimesFromNow, timer, moqArgs, mockTimer);

            return new MyMoq { AgeService = ageService, Moq = mockTimer };
        }

        void EnumerateAndValidateIntervals(IEnumerable<TimeSpan> expectedTimesFromNow, IDispatcherTimer timer, MoqTimerEventArgs moqArgs, Mock<IDispatcherTimer> mockTimer)
        {
            TimeSpan interval = new TimeSpan(0);
            foreach (TimeSpan et in expectedTimesFromNow)
            {
                moqArgs.StartAt += interval;
                interval = et - interval;
                if (interval.Ticks != 0)
                {
                    mockTimer.SetupSet(m => m.Interval = It.IsInRange(
                        interval - shorter, interval + longer, Range.Inclusive));
                    
                    mockTimer.Raise(m => m.Tick += null, moqArgs);

                    LogInterval(timer.Interval, interval);
                }
            }
        }

        static List<TimeSpan> TimesFromNow(IEnumerable<DateTime> birthDateTimes)
        {
            DateTime currentDateTime = DateTime.Now;
            TimeSpan currentTime = currentDateTime.TimeOfDay;
            TimeSpan oneDay = TimeSpan.FromDays(1);
            var expectedTimesFromNow = new List<TimeSpan>(birthDateTimes
                .Select(dob =>
                {
                    if (dob > currentDateTime) { throw new ArgumentException("all dates must be before now"); }
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
            var mockTimer = new Mock<IDispatcherTimer>();
            mockTimer.SetupProperty(m => m.Interval);
            mockTimer.Setup(m => m.Start());
            mockTimer.Setup(m => m.Stop());
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
            TimeSpan interval = TimeSpan.FromDays(1) +toDob;
            Console.WriteLine(interval);
            mockTimer.SetupSet(m => m.Interval = It.IsInRange(
                interval - shorter, interval + longer, Range.Inclusive));
            ageService.AddParticipant(p);
            mockTimer.Verify(m => m.Start(), Times.Once);
            LogInterval(timer.Interval, interval);

            TimeSpan laterDayversary = TimeSpan.FromMinutes(-5);
            p = new ParticipantListItemViewModel(
                new ParticipantBaseModel
                {
                    Id = 2,
                    DateTimeBirth = moqArgs.StartAt + laterDayversary
                });
            mockTimer.ResetCalls();
            ageService.AddParticipant(p);
            mockTimer.VerifySet(x => x.Interval = It.IsAny<TimeSpan>(), Times.Never());
            LogInterval(timer.Interval, interval);

            toDob = TimeSpan.FromMinutes(-30);
            p = new ParticipantListItemViewModel(
                new ParticipantBaseModel
                {
                    Id = 2,
                    DateTimeBirth = moqArgs.StartAt + toDob
                });
            interval = TimeSpan.FromDays(1) + toDob;
            mockTimer.SetupSet(m => m.Interval = It.IsInRange(
                interval - shorter, interval + longer, Range.Inclusive));
            ageService.AddParticipant(p);
            LogInterval(timer.Interval, interval);

            toDob = TimeSpan.FromDays(-1).Add(TimeSpan.FromMinutes(5));
            p = new ParticipantListItemViewModel(
                new ParticipantBaseModel
                {
                    Id = 2,
                    DateTimeBirth = moqArgs.StartAt + toDob
                });
            interval = TimeSpan.FromDays(1) + toDob;
            mockTimer.SetupSet(m => m.Interval = It.IsInRange(
                interval - shorter, interval + longer, Range.Inclusive));
            ageService.AddParticipant(p);
            LogInterval(timer.Interval, interval);
        }

        //[DebuggerStepThrough]
        static void LogInterval(TimeSpan actualInterval, TimeSpan expectedInterval)
        {
            const int allowedSecs = 2;
            TimeSpan minInterval = expectedInterval - TimeSpan.FromSeconds(allowedSecs);
            TimeSpan maxInterval = expectedInterval + TimeSpan.FromSeconds(allowedSecs);
            Console.WriteLine(@"interval was {0:hh\:mm\:ss} (expected {1:hh\:mm\:ss})", actualInterval, expectedInterval);
            //Assert.IsTrue(minInterval < actualInterval && actualInterval < maxInterval, @"interval was {0:hh\:mm\:ss}, should be {1:hh\:mm\:ss} (+/-{2}sec)", actualInterval, expectedInterval, allowedSecs);
        }

        
    }
}
