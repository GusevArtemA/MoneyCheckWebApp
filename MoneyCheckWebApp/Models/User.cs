using System;
using System.Collections.Generic;

#nullable disable

namespace MoneyCheckWebApp.Models
{
    public partial class User
    {
        public User()
        {
            Debtors = new HashSet<Debtor>();
            Purchases = new HashSet<Purchase>();
        }

        public long Id { get; set; }
        public string Username { get; set; }
        public string PasswordMd5Hash { get; set; }
        public long? Balance { get; set; }

        public virtual ICollection<Debtor> Debtors { get; set; }
        public virtual ICollection<Purchase> Purchases { get; set; }
    }
}
