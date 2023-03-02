using System;
using System.ComponentModel;

namespace WebConsultaSMS.Models.Responses
{
    [DisplayName("AuthenticateResponse")]
    public class AuthenticateResponse : ErrorResponse
    {
        public string Token { get; set; }
        private DateTime? _expirationDate;
        public DateTime? ExpirationDate
        {
            get { return _expirationDate; }
            set { _expirationDate = IsValid ? value : null; }
        }
    }
}
