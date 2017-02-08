using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeFixers.Models
{
    public class Transaction
    {
        public int ID { get; set; }
        public double Amount { get; set; }
        

        public virtual int CustomerBankAccountID { get; set; }
        public virtual CustomerBankAccount customerbankaccount { get; set; }
    }
}