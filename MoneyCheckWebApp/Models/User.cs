using System;
using System.Collections.Generic;

#nullable disable

namespace MoneyCheckWebApp.Models
{
    public partial class User
    {
        public User()
        {
            Categories = new HashSet<Category>();
            Debtors = new HashSet<Debtor>();
            Debts = new HashSet<Debt>();
            Purchases = new HashSet<Purchase>();
            UserAuthTokens = new HashSet<UserAuthToken>();
        }

        public long Id { get; set; }
        public string Username { get; set; }
        public string PasswordMd5Hash { get; set; }
        public long Balance { get; set; }

        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Debtor> Debtors { get; set; }
        public virtual ICollection<Debt> Debts { get; set; }
        public virtual ICollection<Purchase> Purchases { get; set; }
        public virtual ICollection<UserAuthToken> UserAuthTokens { get; set; }
    }
}
