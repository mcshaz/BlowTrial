using System;
using System.Threading;
using System.Windows.Controls;
using System.Security;
using MvvmExtraLite.Helpers;
using System.Linq;
using BlowTrial.Security;
using BlowTrial.Domain.Interfaces;
//http://blog.magnusmontin.net/2013/03/24/custom-authorization-in-wpf/
namespace BlowTrial.ViewModel
{

    public class AuthenticationViewModel : WorkspaceViewModel
    {
        private readonly IAuthenticationService _authenticationService;
        private string _username;
        private string _status;

        public AuthenticationViewModel(IAuthenticationService authenticationService) : base()
        {
            _authenticationService = authenticationService;
            LoginCommand = new RelayCommand(Login, CanLogin);
        }

        #region Properties
        public string Username
        {
            get { return _username; }
            set {
                if (_username == value) { return; }
                _username = value; 
                NotifyPropertyChanged("Username"); }
        }


        public string Status
        {
            get { return _status; }
            set
            {
                if (_status == value) { return; }
                _status = value; 
                NotifyPropertyChanged("Status"); }
        }
        #endregion

        #region Commands
        public RelayCommand LoginCommand { get; private set; }
        #endregion

        private const int MaxLoginAttempts = 3;
        private int LoginAttempts { get; set; }

        private void Login(object parameter)
        {
            PasswordBox passwordBox = parameter as PasswordBox;
            var clearTextPassword = passwordBox.SecurePassword;
            try
            {
                //Validate credentials through the authentication service
                IUser user = _authenticationService.AuthenticateUser(Username, clearTextPassword);

                //Get the current principal object
                CustomPrincipal customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;
                if (customPrincipal == null)
                {
                    throw new ArgumentException("The application's default thread principal must be set to a CustomPrincipal object on startup.");
                }

                //Authenticate the user
                customPrincipal.Identity = new CustomIdentity(user.Id, user.Username, user.Roles.Select(r=>r.Name).ToArray());

                //Update UI
                //RaisePropertyChanged("AuthenticatedUser");
                //RaisePropertyChanged("IsAuthenticated");
                Username = string.Empty; //reset
                passwordBox.Password = string.Empty; //reset
                Status = string.Empty;
                Mediator.NotifyColleagues("AuthorisationRequest", true);
                log4net.LogManager.GetLogger("Authenticaton").InfoFormat("Logged in as {0}", user.Username);
                CloseCmd.Execute(null);
            }
            catch (UnauthorizedAccessException)
            {
                if (++LoginAttempts > MaxLoginAttempts)
                {
                    CloseCmd.Execute(null);
                }
                Status = "Login failed! Please provide valid credentials.";
                //Mediator.NotifyColleagues("AuthorisationRequest", false);
            }
        }

        private bool CanLogin(object parameter)
        {
            return true;
        }

        private void Logout(object parameter)
        {
            CustomPrincipal customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;
            if (customPrincipal != null)
            {
                customPrincipal.Identity = new AnonymousIdentity();
                NotifyPropertyChanged("AuthenticatedUser");
                NotifyPropertyChanged("IsAuthenticated");
                Status = string.Empty;
            }
        }

        private void ShowView(object parameter)
        {
            try
            {
                Status = string.Empty;
            }
            catch (SecurityException)
            {
                Status = "You are not authorized!";
            }
        }
    }
}