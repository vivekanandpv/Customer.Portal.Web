using System;
using Customer.Portal.Web.Models;

namespace Customer.Portal.Web.ViewModels {
    public class BankCustomerUpdateViewModel {
        public int Id { get; set; }
        public int FolioNumber { get; set; }
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public Gender Gender { get; set; }
        public string Email { get; set; }
        public MaritalStatus MaritalStatus { get; set; }
        public SourceOfIncome SourceOfIncome { get; set; }
        public int AnnualIncome { get; set; }

        public string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? AddressLine3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int? Pin { get; set; }

        public string Pan { get; set; }
        public DateTime DateOfBirth { get; set; }

        public string MobileNumber { get; set; }
        public string? Telephone { get; set; }

        public string NomineeFirstName { get; set; }
        public string? NomineeLastName { get; set; }
        public string NomineeRelationship { get; set; }
    }
}