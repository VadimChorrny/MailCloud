using MailCloud.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MailCloud.Pages
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : Page
    {
        UserModel userModel = null;

        public Authorization()
        {
            InitializeComponent();
            userModel = new UserModel();
            tbUsername.Text = userModel.Accounts.Select(a => a.Username).First();
            tbPassword.Text = userModel.Accounts.Select(a => a.Password).First();
        }
        public void Authorizate()
        {
            try
            {
                userModel = new UserModel();

                if (userModel.Accounts.Select(a => a.Username == tbUsername.Text && a.Password == tbPassword.Text).First())
                {
                    //gridAuth.Visibility = Visibility.Hidden;
                    //gridMessage.Visibility = Visibility.Visible;
                }
                else
                {
                    MessageBox.Show($"Your login: {tbUsername.Text}\n" +
                        $"Your password: {tbPassword.Text}\n" +
                        $"Is Worng!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Authorizate();
        }



    }
}
