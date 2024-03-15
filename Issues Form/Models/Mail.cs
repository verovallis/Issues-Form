namespace Issues_Form.Models
{
    public class Mail
    {
        public string To {  get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string AttachmentPath { get; set; }
    }
}
