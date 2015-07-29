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

        public virtual int AgeDays
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
                RecalculateDataRequired();
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

        public MaternalBCGScarStatus MaternalBCGScar
        {
            get
            {
                return ParticipantModel.MaternalBCGScar;
            }
            set
            {
                if (ParticipantModel.MaternalBCGScar != value)
                {
                    ParticipantModel.MaternalBCGScar = value;
                    NotifyPropertyChanged("MaternalBCGScar");
                    if (_dataRequired == DataRequiredOption.MaternalBCGScarDetails || _dataRequired == DataRequiredOption.FailedInitialContact)
                    {
                        RecalculateDataRequired();
                    }
                }
            }
        }

        public FollowUpBabyBCGReactionStatus FollowUpBabyBCGReaction
        {
            get
            {
                return ParticipantModel.FollowUpBabyBCGReaction;
            }
            set
            {
                if (ParticipantModel.FollowUpBabyBCGReaction != value)
                {
                    ParticipantModel.FollowUpBabyBCGReaction = value;
                    NotifyPropertyChanged("FollowUpBabyBCGReaction");
                    RecalculateDataRequired();
                }
            }
        }

        public bool PermanentlyUncontactable
        {
            get
            {
                return ParticipantModel.PermanentlyUncontactable;
            }
            set
            {
                if (ParticipantModel.PermanentlyUncontactable != value)
                {
                    ParticipantModel.PermanentlyUncontactable = value;
                    NotifyPropertyChanged("PermanentlyUncontactable");
                    RecalculateDataRequired();
                }
            }
        }

        public int ContactAttempts
        {
            get
            {
                return ParticipantModel.UnsuccessfulFollowUps.Count;
            }
        }

        public DateTime? LastAttemptedContact
        {
            get
            {
                return GetLastDate(ParticipantModel.UnsuccessfulFollowUps);
            }
        }
        static DateTime? GetLastDate(IEnumerable<UnsuccessfulFollowUp> ufs)
        {
            return ufs.Select(u => (DateTime?)u.AttemptedContact).Max();
        }

        public virtual ICollection<UnsuccessfulFollowUp> UnsuccessfulFollowUps
        {
            get
            {
                return ParticipantModel.UnsuccessfulFollowUps;
            }
            set
            {
                if (ParticipantModel.UnsuccessfulFollowUps != value)
                {
                    bool contactChanged = (LastAttemptedContact != GetLastDate(value));
                    bool countChanged = (ContactAttempts != value.Count);
                    ParticipantModel.UnsuccessfulFollowUps = value;
                    if (_dataRequired == DataRequiredOption.AwaitingInfantScarDetails )
                    {
                        RecalculateDataRequired();
                    }
                    if (countChanged) { NotifyPropertyChanged("ContactAttempts"); }
                    if (contactChanged) { NotifyPropertyChanged("LastAttemptedContact"); }
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
                if (AgeDays > ParticipantBaseModel.FollowToAge)
                {
                    return '>' + ParticipantBaseModel.FollowToAge.ToString(); //≥
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
                if (!_dataRequired.HasValue)
                {
                    _dataRequired = ParticipantModel.GetDataRequired();
                }
                return _dataRequired.Value; 
            }
            set
            {
                if (value==_dataRequired) { return; }
                _dataRequired = value;
                NotifyPropertyChanged("DataRequired", "DataRequiredString", "DataRequiredSortOrder");
            }
        }
        public string DataRequiredString
        {
            get
            {
                return DataRequiredStrings.GetDetails(DataRequired);
            }
        }

        public int DataRequiredSortOrder
        {
            get
            {
                return (int)DataRequired;
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
            DataRequired = ParticipantModel.GetDataRequired();
        }
    }
}
