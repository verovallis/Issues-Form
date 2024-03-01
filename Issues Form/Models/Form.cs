using System.ComponentModel.DataAnnotations;

namespace Issues_Form.Models
{
    public class Form
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; } = "";
        [MaxLength(100)]
        public string Email { get; set; } = "";
        [MaxLength(100)]
        public string PhoneNumber { get; set; } = "";
        [MaxLength(100)]
        public string Subject { get; set; } = "";
        [MaxLength(100)]
        public string Category { get; set; } = "";
        [MaxLength(100)]
        public string Building { get; set; } = "";
        [MaxLength(100)]
        public string Company { get; set; } = "";

        public string Description { get; set; } = "";
        [MaxLength(100)]
        public string Attachment { get; set; } = "";
        [MaxLength(100)]
        public DateTime CreatedAt { get; set; }
    }
}
