using AE.Net.Mail;
using EAGetMail;
using MailKit;
using MailKit.Search;
using MailKit.Security;
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
    /// Логика взаимодействия для ReedMessagePage.xaml
    /// </summary>
    public partial class ReedMessagePage : Page
    {
        private MailServer server = null;
        private MailClient client = null;
        public ReedMessagePage()
        {
            InitializeComponent();
            LoadFolders();
        }
        public void LoadFolders()
        {
            tvInput.Header = "INPUT";
            server = new MailServer(
                "imap.gmail.com",
                "prodoq@gmail.com",
                "r4e3w2q1",
                ServerProtocol.Imap4)
            {
                SSLConnection = true,
                Port = 993
            };
            client = new MailClient("TryIt"); // trial version

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
            client = new MailClient("TryIt");
            client.SelectFolder(client.Imap4Folders[1].SubFolders[1]);

            // get mails in selected folder
            MailInfo[] messages = client.GetMail();

            // demo-view
            foreach (var item in messages)
            {
                Mail message = client.GetMail(item);
                lbPreviewMail.Items.Add(/*message.Headers+"\n"+*/message.Subject);
            }
        }
    }
}
