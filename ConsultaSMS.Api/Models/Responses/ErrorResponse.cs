namespace WebConsultaSMS.Models.Responses
{
    public class ErrorResponse
    {
        private string _message;
        public string Message
        {
            get { return _message; }
            set { _message = !IsValid ? value : string.Empty; }
        }
        public bool IsValid { get; set; } = false;
    }
}
