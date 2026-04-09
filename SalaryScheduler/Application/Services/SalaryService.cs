using Microsoft.EntityFrameworkCore;

using SalaryScheduler.Models;

namespace SalaryScheduler.Application.Services
{
    public class SalaryService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SalaryService> _logger;

        public SalaryService(AppDbContext context, ILogger<SalaryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task ProcessMonthlySalary()
        {
            _logger.LogInformation("Starting salary processing...");

            var employees = await _context.Employees
                .Where(e => e.IsActive)
                .ToListAsync();

            foreach (var emp in employees)
            {
                // Idempotency check
                bool alreadyProcessed = await _context.Salaries
                    .AnyAsync(s => s.EmployeeId == emp.Id &&
                                   s.ProcessedDate.Month == DateTime.Now.Month &&
                                   s.ProcessedDate.Year == DateTime.Now.Year);

                if (alreadyProcessed)
                {
                    _logger.LogWarning($"Salary already processed for {emp.Name}");
                    continue;
                }

                var deduction = emp.LeavesTaken * 1000;
                var netSalary = emp.BaseSalary - deduction + emp.Bonus;

                var salary = new Salary
                {
                    EmployeeId = emp.Id,
                    NetSalary = netSalary,
                    ProcessedDate = DateTime.Now
                };

                _context.Salaries.Add(salary);

                _logger.LogInformation($"Processed salary for {emp.Name}: {netSalary}");
            }

            await _context.SaveChangesAsync();
        }
    }
}