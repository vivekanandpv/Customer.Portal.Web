using System;

namespace Customer.Portal.Web.Models {
    public class Account {
        public int Id { get; set; }
        public long AccountNumber { get; set; }
        public AccountType Type { get; set; }
        public double RateOfInterest { get; set; }
        public DateTime CreatedOn { get; set; }
        public AccountStatus Status { get; set; }
        public DateTime? ClosedOn { get; set; }
        public string Ifsc { get; set; }
        public string? CardNumber { get; set; }
        public bool? CardActive { get; set; }
        public string Currency { get; set; }

        public Customer Customer { get; set; }
        public int CustomerId { get; set; }
    }
}