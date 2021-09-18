using MailCloud.EF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
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

namespace MailCloud
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region property
        UserModel userModel = null;
        string filename;
        string server = "smtp.gmail.com"; // sets the server address
        int port = 587; //sets the server port
        #endregion

        public MainWindow()
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
                    gridAuth.Visibility = Visibility.Hidden;
                    gridMessage.Visibility = Visibility.Visible;
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

        public void SendMail()
        {
            // create a message object
            MailMessage message = new MailMessage(tbFrom.Text, tbTo.Text, tbTheme.Text, tbBody.Text);
            //using (StreamReader sr = new StreamReader("mail.html")) // reed our html-file
            //{
            //    message.Body = sr.ReadToEnd();
            //}
            message.IsBodyHtml = false;

            message.Priority = MailPriority.High; // important
            if(filename != null)
            {
                message.Attachments.Add(new Attachment(filename));
            }

            // create a send object
            SmtpClient client = new SmtpClient(server, port);
            client.EnableSsl = true;

            // settings for sending mail
            // vlad email and password xD
            client.Credentials = new NetworkCredential("prodoq@gmail.com", "r4e3w2q1");

            client.SendCompleted += Client_SendCompleted;

            // call asynchronous message sending
            client.SendAsync(message, "blablatoken");
        }
        private void Client_SendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            MessageBox.Show($"Message was sent! Token:{e.UserState}");
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Authorizate();
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Photo"; // Default file name
            dlg.DefaultExt = ".png"; // Default file extension
            dlg.Filter = "Only naked photo (.png)|*.png"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                filename = dlg.FileName;
            }
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            SendMail();
        }
    }
}
