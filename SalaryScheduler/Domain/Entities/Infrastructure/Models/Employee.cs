using System;
using System.Collections.Generic;

namespace SalaryScheduler.Models;

public partial class Employee
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal BaseSalary { get; set; }

    public int LeavesTaken { get; set; }

    public decimal Bonus { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Salary> Salaries { get; set; } = new List<Salary>();
}
