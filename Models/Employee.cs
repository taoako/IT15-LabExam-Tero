using System.ComponentModel.DataAnnotations;

namespace IT15_LabExam_Tero.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        public string? LastName { get; set; }

        [Required]
        public string? Position { get; set; }

        [Required]
        public string? Department { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Daily Rate cannot be negative.")]
        public decimal DailyRate { get; set; }

        // Initialized to an empty list to remove the CS8618 warning
        public ICollection<Payroll> Payrolls { get; set; } = new List<Payroll>();
    }
}