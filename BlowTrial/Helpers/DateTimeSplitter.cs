using BlowTrial.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlowTrial.Helpers
{
    class DateTimeSplitter
    {
        #region Constructor
        public DateTimeSplitter() { }
        public DateTimeSplitter(DateTime? dateAndTime)
        {
            this.DateAndTime = dateAndTime;
        }
        #endregion

        #region Fields
        DateTime? _date;
        TimeSpan? _timeSpan;
        #endregion

        #region Properties
        public DateTime? DateAndTime
        {
            get 
            {
                if (_date.HasValue && _timeSpan.HasValue)
                {
                    return _date.Value + _timeSpan.Value;
                }
                return null;
            }
            set 
            {
                if (value.HasValue)
                {
                    _date = value.Value.Date;
                    _timeSpan = value.Value - _date;
                }
                else
                { 
                    _date = null;
                    _timeSpan = null;
                }
            }
        }
        public DateTime? Date
        {
            get
            {
                return _date;
            }
            set
            {
                if (value.HasValue)
                {
                    _date = value.Value.Date;

                }
                else
                {
                    _date = null;
                }
            }
        }
        public TimeSpan? Time
        {
            get
            {
                return _timeSpan;
            }
            set
            {
                if (value.HasValue)
                {
                    double totHrs = value.Value.TotalHours;
                    if (totHrs < 0 || totHrs >= 24)
                    {
                        throw new ArgumentOutOfRangeException("Time", totHrs, "Time must be between 0 and < 24 hours");
                    }
                }
                _timeSpan = value;
            }
        }
        #endregion
        
        #region Methods
        public void ValidateIsAfter(string earlierDescription, DateTime earlierDate, ref DateTimeErrorString returnErrors)
        {
            if (returnErrors.DateError == null)
            {
                if (Date.HasValue)
                {
                    if (earlierDate.Date > Date)
                    {
                        returnErrors.DateError = string.Format(Strings.DateTime_Error_Date_MustComeAfter, earlierDescription);
                    }
                    else if (earlierDate > DateAndTime)
                    {
                        returnErrors.TimeError = string.Format(Strings.DateTime_Error_Time_MustComeAfter, earlierDescription);
                    }
                    if (Time == null)
                    {
                        returnErrors.SetTimeRequired();
                    }
                }
                else if (Time.HasValue)
                {
                    returnErrors.DateError = Strings.DateTime_Error_DateEmpty;
                }
            }
        }
        public void ValidateIsBefore(string laterDescription, DateTime laterDate, ref DateTimeErrorString returnErrors)
        {
            if (returnErrors.DateError == null)
            {
                if (Date.HasValue)
                {
                    if (laterDate < Date)
                    {
                        returnErrors.DateError = string.Format(Strings.DateTime_Error_Date_MustComeBefore, laterDescription);
                    }
                    else if (laterDate < DateAndTime)
                    {
                        returnErrors.TimeError = string.Format(Strings.DateTime_Error_Time_MustComeBefore, laterDescription);
                    }
                    if (Time == null)
                    {
                        returnErrors.SetTimeRequired();
                    }
                }
                else if (Time.HasValue)
                {
                    returnErrors.DateError = Strings.DateTime_Error_DateEmpty;
                }
            }
        }

        public DateTimeErrorString ValidateNotEmpty()
        {
            var returnErrors = new DateTimeErrorString();
            if (Date == null)
            {
                returnErrors.DateError = Strings.DateTime_Error_Date_DateTimeEmpty;
            }
            if (Time==null)
            {
                returnErrors.SetTimeRequired();
            }
            return returnErrors;
        }
        #endregion
    }

    public class DateTimeErrorString
    {
        public string DateError { get; set; }
        public string TimeError { get; set; }
        public void SetTimeRequired()
        {
            if (TimeError == null || (DateError != null && TimeError == Strings.DateTime_Error_TimeEmpty))
            {
                TimeError = (DateError==null)
                    ?Strings.DateTime_Error_TimeEmpty
                    :Strings.DateTime_Error_Time_DateTimeEmpty;
            }
        }
    }
}
