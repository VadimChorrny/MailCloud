using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MailCloud.EF
{
    public class UserModel : DbContext
    {
        public UserModel()
            : base("name=UserModel")
        {
        }
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
    }

    public class Account
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class Message
    {
        public int Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Theme { get; set; }
        public string Body { get; set; }
        public int? AccountId { get; set; }
        public Account Account { get; set; }

    }
}