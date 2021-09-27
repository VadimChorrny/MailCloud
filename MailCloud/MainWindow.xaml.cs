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
using MailCloud.Service;
using System.Collections.ObjectModel;

namespace MailCloud
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MailClient client = null;
        public bool IsWhite { get; set; } = true;
        public MainWindow()
        {
            InitializeComponent();
            client = new MailClient("TryIt");
        }
        private void btnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            SMTPSenderWindow send = new SMTPSenderWindow();
            send.Show();
        }
        private void btnChangeTheme_Click(object sender, RoutedEventArgs e)
        {
            if (IsWhite)
            {
                lbPreviewMail.Background = Brushes.Gray;
                lbAllMessage.Background = Brushes.LightGray;
                IsWhite = false;
            }
            else
            {
                lbPreviewMail.Background = Brushes.Wheat;
                lbAllMessage.Background = Brushes.White;
                IsWhite = true;
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            lbPreviewMail.Items.Clear();
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
                        var query = SearchQuery.DeliveredAfter(DateTime.Parse("2021-01-01"))
                        .And(SearchQuery.SubjectContains(tbSearching.Text)
                        .And(SearchQuery.Seen));

                        foreach (var uid in inbox.Search(query, cancel.Token))
                        {
                            var message = inbox.GetMessage(uid, cancel.Token);
                            lbPreviewMail.Items.Add(message.Subject);
                        }
                    }));
                }
            }
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            HandlerLoadPreview();
        }
        private async void HandlerLoadPreview() => await LoadPreview();
        private Task LoadPreview() // or add async
        {
            return Task.Run(() =>
            {
                if(!lbPreviewMail.Items.IsEmpty)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        lbPreviewMail.Items.Clear();
                    }));
                }
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
        private void lbPreviewMail_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            HandlerLoadAllMessage();
        }
        private async void HandlerLoadAllMessage() => await LoadAllMessage();
        private Task LoadAllMessage()
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
                                var query = SearchQuery.DeliveredAfter(DateTime.Parse("2021-01-01"))
                                .And(SearchQuery.SubjectContains((string)lbPreviewMail.SelectedItem))
                                .And(SearchQuery.Seen);

                                foreach (var uid in inbox.Search(query, cancel.Token))
                                {
                                    var message = inbox.GetMessage(uid, cancel.Token);
                                    txtFrom.Text = message.From.ToString();
                                    txtSubject.Text = message.Subject;
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

    }
}
