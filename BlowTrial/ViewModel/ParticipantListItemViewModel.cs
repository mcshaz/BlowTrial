using BlowTrial.Models;
using BlowTrial.Domain.Outcomes;
using BlowTrial.Domain.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MvvmExtraLite.Helpers;
using BlowTrial.Infrastructure.Extensions;
using BlowTrial.Helpers;
using BlowTrial.Infrastructure.Interfaces;
using BlowTrial.Properties;
using System.Windows.Media;

namespace BlowTrial.ViewModel
{
    public class ParticipantListItemViewModel : CrudWorkspaceViewModel, IBirthday
    {
        #region Fields
        string _searchableString;
        DataRequiredOption? _dataRequired;
        string _dataRequiredString;
        #endregion

        #region Constructors
        public ParticipantListItemViewModel(ParticipantBaseModel participant, IRepository repository=null) : base(repository)
        {
            ParticipantModel = participant;
        }
        #endregion

        #region properties
        public ParticipantBaseModel ParticipantModel {get; private set;}
        public StudyCentreModel StudyCentre
        {
            get
            {
                return ParticipantModel.StudyCentre;
            }
        }

        public string StudyCentreName
        {
            get
            {
                return StudyCentre.Name;
            }
        }

        public Brush TextColour
        {
            get
            {
                return StudyCentre.TextColour;
            }
        }

        public Brush BackgroundColour
        {
            get
            {
                return StudyCentre.BackgroundColour;
            }
        }

        public ParticipantBaseModel Participant
        {
            get
            {
                return ParticipantModel;
            }
        }

        public int Id
        {
            get
            {
                return this.ParticipantModel.Id;
            }
        }

        public string Name
        {
            get
            {
                return ParticipantModel.Name;
            }

            set
            {
                if (value == ParticipantModel.Name) { return; }
                ParticipantModel.Name = value;
                _searchableString = null;
                NotifyPropertyChanged("Name");
            }
        }

        public int AgeDays
        {
            get
            {
                return ParticipantModel.AgeDays;
            }

            set
            {
                if (value == ParticipantModel.AgeDays) { return; }
                ParticipantModel.AgeDays = value;
                NotifyPropertyChanged("AgeDays");
                if (DataRequired== DataRequiredOption.AwaitingOutcomeOr28 && ParticipantModel.AgeDays>=28)
                {
                    RecalculateDataRequired();
                }
            }
        }

        public DateTime RegisteredAt
        {
            get
            {
                return ParticipantModel.RegisteredAt;
            }
            set
            {
                if (ParticipantModel.RegisteredAt != value)
                {
                    ParticipantModel.RegisteredAt = value;
                    NotifyPropertyChanged("RegisteredAt");
                }
            }
        }

        public virtual ICollection<VaccineAdministered> VaccinesAdministered
        {
            get
            {
                return ParticipantModel.VaccinesAdministered;
            }
            set
            {
                if(ParticipantModel.VaccinesAdministered != value)
                {
                    ParticipantModel.VaccinesAdministered = value;
                    RecalculateDataRequired();
                }
            }
        }

        public virtual ICollection<ProtocolViolation> ProtocolViolations
        {
            get
            {
                return ParticipantModel.ProtocolViolations;
            }
            set
            {
                if (ParticipantModel.ProtocolViolations != value)
                {
                    ParticipantModel.ProtocolViolations = value;
                    RecalculateDataRequired();
                }
            }
        }

        public bool? IsKnownDead
        {
            get
            {
                return ParticipantModel.IsKnownDead;
            }
        }

        public string AgeDaysString
        {
            get
            {
                if (ParticipantModel.IsKnownDead == true)
                {
                    return Strings.NotApplicable;
                }
                if (AgeDays > 28)
                {
                    return ">28"; //≥
                }
                return AgeDays.ToString();
            }
        }

        public string TrialArm
        {
            get
            {
                return this.ParticipantModel.TrialArmDescription;
            }
        }

        public string HospitalIdentifier
        {
            get
            {
                return this.ParticipantModel.HospitalIdentifier;
            }
            internal set
            {
                if (ParticipantModel.HospitalIdentifier != value)
                {
                    ParticipantModel.HospitalIdentifier = value;
                    _searchableString = null;
                    NotifyPropertyChanged("HospitalIdentifier");
                }
            }
        }

        public string Gender
        {
            get
            {
                return this.ParticipantModel.Gender;
            }
        }

        public bool IsMale
        {
            set
            {
                if (ParticipantModel.IsMale != value)
                {
                    ParticipantModel.IsMale = value;
                    NotifyPropertyChanged("Gender");
                }
            }
        }

        public DateTime DateTimeBirth
        {
            get
            {
                return ParticipantModel.DateTimeBirth;
            }
            set
            {
                if (value == ParticipantModel.DateTimeBirth) { return; }
                ParticipantModel.DateTimeBirth = value;
                NotifyPropertyChanged("DateTimeBirth");
            }
        }

        public DataRequiredOption DataRequired
        {
            get 
            {
                return (_dataRequired = ParticipantModel.DataRequired).Value; 
            }
        }
        public string DataRequiredString
        {
            get
            {
                return _dataRequiredString ?? (_dataRequiredString = DataRequiredStrings.GetDetails(_dataRequired ?? DataRequired));
            }
        }

        public int DataRequiredSortOrder
        {
            get
            {
                return (int)(_dataRequired ?? DataRequired);
            }
        }

        public virtual OutcomeAt28DaysOption OutcomeAt28Days
        {
            get { return ParticipantModel.OutcomeAt28Days; }
            set
            {
                if (value == ParticipantModel.OutcomeAt28Days) { return; }
                ParticipantModel.OutcomeAt28Days = value;
                RecalculateDataRequired();
            }
        }
        public virtual DateTime? DischargeDateTime
        {
            get { return ParticipantModel.DischargeDateTime; }
            set
            {
                if (value == ParticipantModel.DischargeDateTime) { return; }
                ParticipantModel.DischargeDateTime = value;
                RecalculateDataRequired();
            }
        }
        public virtual DateTime? DeathOrLastContactDateTime
        {
            get { return ParticipantModel.DeathOrLastContactDateTime; }
            set
            {
                if (value == ParticipantModel.DeathOrLastContactDateTime) { return; }
                ParticipantModel.DeathOrLastContactDateTime = value;
                RecalculateDataRequired();
            }
        }
        public virtual CauseOfDeathOption CauseOfDeath
        {
            get { return ParticipantModel.CauseOfDeath; }
            set
            {
                if (value == ParticipantModel.CauseOfDeath) { return; }
                ParticipantModel.CauseOfDeath = value;
                RecalculateDataRequired();
            }
        }

        public string SearchableString
        {
            get
            {
                return _searchableString ?? (_searchableString = ParticipantModel.Id.ToString() + '|' + ParticipantModel.HospitalIdentifier + '|' + ParticipantModel.Name);
            }
        }

        #endregion

        #region methods
        public override bool IsValid()
        {
            return true;
        }
        #endregion

        internal void RecalculateDataRequired()
        {
            DataRequiredOption newRequired = ParticipantModel.DataRequired;
            if (_dataRequired == newRequired) { return; }
            _dataRequired = newRequired;
            _dataRequiredString = null;
            NotifyPropertyChanged("DataRequired", "DataRequiredString", "DataRequiredSortOrder");
        }
    }
}
