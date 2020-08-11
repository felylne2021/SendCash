using SendCash.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SendCash.ViewModels {
    public class TransactionViewModel {
        // account
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }

        //bank
        public string BankName { get; set; }

        public TransactionHeader TransactionHeader { get; set; }

        public IEnumerable<TransactionDetail> TransactionDetails { get; set; }
    }
}