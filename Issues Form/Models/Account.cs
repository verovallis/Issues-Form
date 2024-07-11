using System.ComponentModel.DataAnnotations;

namespace Issues_Form.Models
{
    public class Account
    {
        [Key]
        public int DepartmentID { get; set; }

        public string DepartmentCode { get; set; }

        public string CostCenter { get; set; }

        public string ManagerID { get; set; }

        public string ManagerName { get; set; }

        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
 
