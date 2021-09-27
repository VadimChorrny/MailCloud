using AE.Net.Mail;
using MailCloud.EF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace MailCloud.Pages
{
    /// <summary>
    /// Логика взаимодействия для SMTPSenderWindow.xaml
    /// </summary>
    public partial class SMTPSenderWindow : Window
    {
        #region property
        UserModel userModel = null;
        string filename;
        string server = "smtp.gmail.com"; // sets the server address
        static ImapClient IC;
        int port = 587; //sets the server port
        #endregion
        public SMTPSenderWindow()
        {
            InitializeComponent();
        }
        private Task WriteMessageInDataBase(string from, string to, string theme, string body)
        {
            userModel = new UserModel();
            userModel.Messages.Add(new Message()
            {
                From = from,
                To = to,
                Theme = theme,
                Body = body,
                Time = DateTime.Now,
                Date = DateTime.Now.Date
            });
            userModel.SaveChanges();
            return Task.CompletedTask;
        }
        public async Task<Task> SendMail()
        {
            // create a message object
            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(tbFrom.Text, tbTo.Text, tbTheme.Text, tbBody.Text);
            //using (StreamReader sr = new StreamReader("mail.html")) // reed our html-file
            //{
            //    message.Body = sr.ReadToEnd();
            //}

            message.Body = $"<h2>{tbBody.Text}</h2>";

            message.IsBodyHtml = true;

            message.Priority = System.Net.Mail.MailPriority.High; // important
            if (filename != null)
            {
                foreach (var item in lbFiles.Items)
                {
                    message.Attachments.Add(new System.Net.Mail.Attachment((string)item));
                }
            }

            // create a send object
            SmtpClient client = new SmtpClient(server, port);
            client.EnableSsl = true;

            // settings for sending mail
            // vlad email and password xD
            client.Credentials = new NetworkCredential("prodoq@gmail.com", "r4e3w2q1");

            client.SendCompleted += Client_SendCompleted;

            // call asynchronous message sending
            client.SendAsync(message, "ChorrnyToken");
            await WriteMessageInDataBase(tbFrom.Text, tbTo.Text, tbTheme.Text, tbBody.Text);
            return Task.CompletedTask;
        }
        private void Client_SendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            MessageBox.Show($"Message was sent! Token:{e.UserState}");
        }
        private async void btnSend_Click(object sender, RoutedEventArgs e)
        {
            await SendMail();
        }
        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "My dick"; // Default file name
            dlg.DefaultExt = "*"; // Default file extension
            dlg.Filter = "Only naked photo (.png)|*.png|Text files (*.txt)|*.txt|All files (*.*)|*.*"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                filename = dlg.FileName;
                lbFiles.Items.Add(filename);
            }
        }
    }
}
