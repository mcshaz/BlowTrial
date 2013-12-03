using BlowTrial.Domain.Providers;
using BlowTrial.Infrastructure.Interfaces;
using BlowTrial.Models;
using BlowTrial.Properties;
using BlowTrial.Security;
using MvvmExtraLite.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Input;

namespace BlowTrial.ViewModel
{
    public sealed class CreateNewUserViewModel : CrudWorkspaceViewModel, IDataErrorInfo
    {
        #region Fields
        CreateNewUserModel _model;
        
        #endregion
        #region Constructors
        public CreateNewUserViewModel(IMembershipContext membershipContext)
        {
            _model = new CreateNewUserModel();
            MembershipContext = membershipContext;
            SaveChangesCmd = new RelayCommand(SaveChanges, param => WasValidOnLastNotify);
            DisplayName = Strings.CreateNewUserViewModel_DisplayName;
            WasCancelled = true;
        }
        #endregion
        #region Properties
        public IMembershipContext MembershipContext { get; private set; }
        public bool WasCancelled {get; private set;}
        public string UserName 
        {
            get { return _model.UserName; }
            set
            {
                if (value == _model.UserName) { return; }
                _model.UserName = value;
                NotifyPropertyChanged("UserName");
            }
        }
        public string EncryptedPassword 
        {
            get { return _model.EncryptedPassword; }
            set
            {
                if (value == _model.EncryptedPassword) { return; }
                _model.EncryptedPassword = value;
                NotifyPropertyChanged("EncryptedPassword", "ConfirmEncryptedPassword");
            }
        }
        public int UnencryptedPasswordLength
        {
            get { return _model.UnencryptedPasswordLength; }
            set
            {
                if (value == _model.UnencryptedPasswordLength) { return; }
                _model.UnencryptedPasswordLength = value;
                NotifyPropertyChanged("EncryptedPassword");
            }
        }
        public string ConfirmEncryptedPassword 
        {
            get { return _model.ConfirmEncryptedPassword; }
            set
            {
                if (value == _model.ConfirmEncryptedPassword) { return; }
                _model.ConfirmEncryptedPassword = value;
                NotifyPropertyChanged("ConfirmEncryptedPassword");
            }
        }
        public bool ChangeToThisUserOnSave { get; set; }

        #endregion
        #region ICommands
        public RelayCommand SaveChangesCmd { get; private set; }
        void SaveChanges(object param)
        {
            var roles = MembershipContext.Roles.Where(r => r.Name == "Administrator").ToList();
            var newUser = new Domain.Tables.Investigator
                {
                    Id = Guid.NewGuid(),
                    Username = UserName,
                    LastLoginAt = DateTime.UtcNow,
                    Password = EncryptedPassword,
                    Roles = roles
                };
            MembershipContext.Investigators.Add(newUser);
            if (ChangeToThisUserOnSave)
            {
                CustomPrincipal customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;
                if (customPrincipal == null)
                {
                    throw new ArgumentException("The application's default thread principal must be set to a CustomPrincipal object on startup.");
                }

                //Authenticate the user
                customPrincipal.Identity = new CustomIdentity(newUser.Id, newUser.Username, roles.Select(r=>r.Name).ToArray());
            }
            MembershipContext.SaveChanges();
            WasCancelled = false;
            OnRequestClose();
        }
        #endregion

        #region IDataErrorInfo Members

        string IDataErrorInfo.Error { get { return null; } }

        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                string error = this.GetValidationError(propertyName);
                CommandManager.InvalidateRequerySuggested();
                return error;
            }
        }

        #endregion // IDataErrorInfo Members

        #region Validation

        /// <summary>
        /// Returns true if this object has no validation errors.
        /// </summary>
        public override bool IsValid()
        {
            bool returnVal = _model.IsValid() && !ValidatedProperties.Any(v => this.GetValidationError(v) != null);
            CommandManager.InvalidateRequerySuggested();
            return returnVal;
        }

        readonly string[] ValidatedProperties = 
        { 
            "UserName"
        };
        string GetValidationError(string propertyName)
        {
            string error = ((IDataErrorInfo)_model)[propertyName];

            if (error == null && ValidatedProperties.Contains(propertyName))
            {
                switch (propertyName)
                {
                    case "UserName":
                        error = this.ValidateNewUserName();
                        break;
                    default:
                        Debug.Fail("Unexpected property being validated on NewPatient: " + propertyName);
                        break;
                }
            }
            return error;
        }

        string ValidateNewUserName()
        {
            if (MembershipContext.Investigators.Any(i => i.Username == UserName))
            {
                return Strings.CreateNewUserViewModel_Error_UserExists;
            }
            return null;
        }
        #endregion
    }
}
