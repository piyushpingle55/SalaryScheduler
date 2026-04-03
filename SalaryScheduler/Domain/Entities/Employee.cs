namespace SalaryScheduler.Domain.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal BaseSalary { get; set; }
        public int LeavesTaken { get; set; }
        public decimal Bonus { get; set; }
        public bool IsActive { get; set; } = true;
    }
}