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

namespace BlowTrial.ViewModel
{
    public class ParticipantListItemViewModel : ViewModelBase
    {
        #region Fields
        #endregion

        #region Constructors
        internal ParticipantListItemViewModel(ParticipantModel participant)
        {
            this.ParticipantModel = participant;
        }
        #endregion

        #region properties
        internal ParticipantModel ParticipantModel
        {
            get;
            set;
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
                return this.ParticipantModel.Name;
            }
        }
        public string PhoneNumber
        {
            get
            {
                return this.ParticipantModel.PhoneNumber;
            }
        }
        public string TrialArm
        {
            get
            {
                return this.ParticipantModel.TrialArm;
            }
        }
        public string HospitalIdentifier
        {
            get
            {
                return this.ParticipantModel.HospitalIdentifier;
            }
        }
        public int AgeDays
        {
            get
            {
                return this.ParticipantModel.Age.Days;
            }
        }


        public string Gender
        {
            get
            {
                return this.ParticipantModel.Gender;
            }
        }
        public int AdmissionWeight
        {
            get
            {
                return this.ParticipantModel.AdmissionWeight;
            }
        }
        public DateTime DateTimeEnrollment
        {
            get { return this.ParticipantModel.RegisteredAt; }
        }



        #endregion

        #region methods
        internal void SuggestRequery()
        {
            base.NotifyPropertyChanged("DataRequired", "PhoneNumber");
        }
        void SuggestRequeryDay(object args)
        {
            NotifyPropertyChanged("AgeDays", "DataRequired"); //"CGA", 
        }
        #endregion

        #region Destructor
        ~ParticipantListItemViewModel()
        {
        }
        #endregion
    }
}
