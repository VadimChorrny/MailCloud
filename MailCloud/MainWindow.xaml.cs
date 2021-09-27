using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MailKit.Security;
using MimeKit;
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
using AE.Net.Mail;
using MailCloud.Pages;
using EAGetMail;
using ImapClient = MailKit.Net.Imap.ImapClient;
using System.Threading;

namespace MailCloud
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MailServer server = null;
        private MailClient client = null;
        public MainWindow()
        {
            InitializeComponent();
            client = new MailClient("TryIt");
            LoadFolders();
        }
        public void LoadFolders()
        {
            #region connect
            tvFolders.Header = "INPUT";
            server = new MailServer(
                "imap.gmail.com",
                "chorrnyinc@gmail.com",
                "epvytbgottgvwexh",
                ServerProtocol.Imap4)
            {
                SSLConnection = true,
                Port = 993
            };

            client = new MailClient("TryIt");
            #endregion
            try
            {
                client.Connect(server);
                // show all folders
                foreach (var f in client.GetFolders())
                {
                    tvFolders.Items.Add(f.Name);
                    foreach (var subF in f.SubFolders)
                    {
                        tvInput.Items.Add(subF.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        // maybe for preview
        private void tvInput_Selected(object sender, RoutedEventArgs e)
        {
            _ = Handler();
        }
        private async Task Handler()
        {
            await LoadPreview();
        }
        private Task LoadPreview()
        {
            return Task.Run(() =>
            {
                using (var client = new ImapClient())
                {
                    using (var cancel = new CancellationTokenSource())
                    {
                        client.Connect("imap.gmail.com", 993, true, cancel.Token);

                        // If you want to disable an authentication mechanism,
                        // you can do so by removing the mechanism like this:
                        client.AuthenticationMechanisms.Remove("XOAUTH");

                        client.Authenticate("chorrnyinc@gmail.com", "epvytbgottgvwexh", cancel.Token);

                        // The Inbox folder is always available...
                        var inbox = client.Inbox;
                        inbox.Open(FolderAccess.ReadOnly, cancel.Token);
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            txtCountMessage.Text = inbox.Count.ToString();
                        }));
                        // download each message based on the message index
                        for (int i = 0; i < inbox.Count; i++)
                        {
                            Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                var message = inbox.GetMessage(i, cancel.Token);
                                lbPreviewMail.Items.Add(message.Subject);
                            }));
                        }
                    }
                }
            });
        }

        private void ClearMessage()
        {
            txtFrom.Text = null;
            txtHeader.Text = null;
            txtSubject.Text = null;
            txtSendDate.Text = null;
            txtBody.Text = null;
        }
        private Task LoadFull()
        {
            return Task.Run(() =>
            {
                try
                {
                    using (var client = new ImapClient())
                    {
                        using (var cancel = new CancellationTokenSource())
                        {

                            client.Connect("imap.gmail.com", 993, true, cancel.Token);

                            // If you want to disable an authentication mechanism,
                            // you can do so by removing the mechanism like this:
                            client.AuthenticationMechanisms.Remove("XOAUTH");

                            client.Authenticate("chorrnyinc@gmail.com", "epvytbgottgvwexh", cancel.Token);

                            // The Inbox folder is always available...
                            var inbox = client.Inbox;
                            inbox.Open(FolderAccess.ReadOnly, cancel.Token);
                            // let's try searching for some messages...
                            Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                var query = SearchQuery.DeliveredAfter(DateTime.Parse("2021-09-12"))
                                .And(SearchQuery.SubjectContains((string)lbPreviewMail.SelectedItem))
                                .And(SearchQuery.Seen);

                                foreach (var uid in inbox.Search(query, cancel.Token))
                                {
                                    var message = inbox.GetMessage(uid, cancel.Token);
                                    txtFrom.Text = message.From.ToString();
                                    txtSubject.Text = message.Subject;
                                    txtHeader.Text = message.Headers.ToString();
                                    txtSendDate.Text = message.Date.ToString();
                                    txtBody.Text = message.TextBody;
                                }
                            }));
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            });
        }
        private void btnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            SendWindow send = new SendWindow();
        }
        /// <summary>
        ///  MAYBE NEED CHANGE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// 
        private async Task HandlerTwo()
        {
            await LoadFull();
        }
        private void lbPreviewMail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HandlerTwo();
        }
    }
}
