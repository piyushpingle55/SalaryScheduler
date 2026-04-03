using SalaryScheduler.Application.Services;

namespace SalaryScheduler.BackgroundJobs.Jobs
{
    public class SalaryJob
    {
        private readonly SalaryService _salaryService;

        public SalaryJob(SalaryService salaryService)
        {
            _salaryService = salaryService;
        }

        public async Task Execute()
        {
            await _salaryService.ProcessMonthlySalary();
        }
    }
}