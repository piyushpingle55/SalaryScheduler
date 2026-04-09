using System;
using System.Collections.Generic;

namespace SalaryScheduler.Models;

public partial class Salary
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public decimal NetSalary { get; set; }

    public DateTime ProcessedDate { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
