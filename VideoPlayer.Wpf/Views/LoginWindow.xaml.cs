// Views/LoginWindow.xaml.cs
using System.Windows;
using VideoPlayer.Wpf.ViewModels;

namespace VideoPlayer.Wpf.Views
{
    public partial class LoginWindow : Window
    {
        private LoginViewModel Vm => (LoginViewModel)DataContext;

        public LoginWindow()
        {
            InitializeComponent();
            DataContext = new LoginViewModel();
            Vm.LoginSucceeded += OnLoginSucceeded;
        }

        private void OnLoginSucceeded()
        {
            // Open main window after successful login
            var mw = new MainWindow();
            mw.Show();
            Close();
        }

        private void Pwd_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm && sender is System.Windows.Controls.PasswordBox pb)
                vm.Pass = pb.Password;
        }
    }
}
