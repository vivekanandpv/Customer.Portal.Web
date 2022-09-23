using System;

namespace Customer.Portal.Web.Exceptions {
    public class DomainInvariantException : Exception {
        public DomainInvariantException(string message): base(message) {
            
        }
    }
}