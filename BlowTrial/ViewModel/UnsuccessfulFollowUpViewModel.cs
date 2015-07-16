using BlowTrial.Domain.Tables;
using BlowTrial.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace BlowTrial.ViewModel
{
    public sealed class UnsuccessfulFollowUpViewModel : NotifyChangeBase, IDataErrorInfo
    {
        #region Constructors
        public UnsuccessfulFollowUpViewModel(UnsuccessfulFollowUpModel unsuccessfulFollowUpModel)
        {
            UnsuccessfulFollowUpMod = unsuccessfulFollowUpModel;
        }
        #endregion
        #region Fields   
        #endregion

        #region Properties
        public UnsuccessfulFollowUpModel UnsuccessfulFollowUpMod { get; private set; }

        public int Id { get { return UnsuccessfulFollowUpMod.Id; } }

        public DateTime? AttemptedContact
        {
            get { return UnsuccessfulFollowUpMod.AttemptedContact; }
            set
            {
                if (value == UnsuccessfulFollowUpMod.AttemptedContact) { return; }
                UnsuccessfulFollowUpMod.AttemptedContact = value;
                NotifyPropertyChanged("AttemptedContact");
            }
        }

        public bool AllowEmptyRecord { get; set; }
        #endregion

        #region Methods
        #endregion

        #region IDataErrorInfo Members

        string IDataErrorInfo.Error { get { return null; } }

        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                if (AllowEmptyRecord && IsEmpty)
                {
                    return null;
                }
                string error = ((IDataErrorInfo)UnsuccessfulFollowUpMod)[propertyName];
                CommandManager.InvalidateRequerySuggested();
                return error;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return !AttemptedContact.HasValue;
            }
        }


        public bool IsValid()
        {
            if (AllowEmptyRecord && IsEmpty) { return true; }
            return this.UnsuccessfulFollowUpMod.IsValid();
        }

        #endregion // IDataErrorInfo Members

        #region Destructor

        #endregion
    }
}
