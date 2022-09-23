using System;

namespace Customer.Portal.Web.Exceptions {
    public class RecordNotFoundException : Exception {
        public RecordNotFoundException(string message): base(message) {
            
        }
    }
}