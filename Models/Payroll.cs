using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IT15_LabExam_Tero.Models
{
    public class Payroll
    {
        [Key]
        public int PayrollId { get; set; }

        [Required]
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Payroll Date")]
        public DateTime Date { get; set; }

        [Required]
        [Range(0, 31, ErrorMessage = "Days Worked must be between 0 and 31.")]
        [Display(Name = "Days Worked")]
        public decimal DaysWorked { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Deduction cannot be negative.")]
        [Display(Name = "Deduction")]
        public decimal Deduction { get; set; }

        // Automatically computed, not required as user input
        [Display(Name = "Gross Pay")]
        public decimal GrossPay { get; set; }

        [Display(Name = "Net Pay")]
        public decimal NetPay { get; set; }
    }
}