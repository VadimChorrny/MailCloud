using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailCloud.Service
{
    public class MailInfoService
    {
        private string from;
        private string header;
        private string subject;
        private DateTime sendDate;
        private string body;

        public MailInfoService(string from,string header,string subject,DateTime sendDate,string body)
        {
            From = from;
            Header = header;
            Subject = subject;
            SendDate = sendDate;
            Body = body;
        }

        public string Body
        {
            get { return body; }
            set { body = value; }
        }

        public DateTime SendDate
        {
            get { return sendDate; }
            set { sendDate = value; }
        }

        public string Subject
        {
            get { return subject; }
            set { subject = value; }
        }


        public string Header
        {
            get { return header; }
            set { header = value; }
        }


        public string From
        {
            get { return from; }
            set { from = value; }
        }

    }
}
