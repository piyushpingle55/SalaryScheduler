namespace SalaryScheduler.Domain.Entities
{
    public class Salary
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public decimal NetSalary { get; set; }
        public DateTime ProcessedDate { get; set; }

        public Employee Employee { get; set; }
    }
}