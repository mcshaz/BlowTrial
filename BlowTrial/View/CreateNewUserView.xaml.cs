using BlowTrial.Security;
using BlowTrial.ViewModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace BlowTrial.View
{

    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class CreateNewUserView : System.Windows.Controls.UserControl
    {
        public CreateNewUserView()
        {
            InitializeComponent();
            DataContextChanged += CreateNewUserView_DataContextChanged;
        }

        void CreateNewUserView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CreateNewUserViewModel vm = (CreateNewUserViewModel)DataContext;
            passwordErrors.Text = ((IDataErrorInfo)vm)["EncryptedPassword"];
            confirmPasswordErrors.Text = ((IDataErrorInfo)vm)["ConfirmEncryptedPassword"];
        }

        private void passwordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox pwdBox = (PasswordBox)sender;
            CreateNewUserViewModel vm = (CreateNewUserViewModel)DataContext;

            string userName = userNameTxt.Text;

            string hashedPassword = (pwdBox.SecurePassword.Length == 0) ? string.Empty : AuthenticationService.CalculateHash(pwdBox.SecurePassword, userName);

            if (sender == passwordBox)
            {
                vm.EncryptedPassword = hashedPassword;
                vm.UnencryptedPasswordLength = pwdBox.SecurePassword.Length;
                passwordErrors.Text = ((IDataErrorInfo)vm)["EncryptedPassword"];
            }
            else
            {
                vm.ConfirmEncryptedPassword = hashedPassword;
                confirmPasswordErrors.Text = ((IDataErrorInfo)vm)["ConfirmEncryptedPassword"];
            }
        }
    }
}
