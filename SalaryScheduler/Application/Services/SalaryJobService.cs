using Prometheus;
using Serilog;

namespace SalaryScheduler.Application.Services
{
    public class SalaryJobService
    {
        // Metric 1: Counter - Tracks cumulative salary processing attempts
        private static readonly Counter ProcessedSalariesCounter = Metrics.CreateCounter(
            "salary_jobs_processed_total",
            "Total number of salary jobs processed.",
            new CounterConfiguration { LabelNames = new[] { "status" } }
        );

        // Metric 2: Histogram - Tracks job execution time distribution
        private static readonly Histogram ExecutionDuration = Metrics.CreateHistogram(
            "salary_job_duration_seconds",
            "Duration of salary processing jobs in seconds."
        );

        public async Task ProcessMonthlySalariesAsync()
        {
            // Start duration timer
            using (ExecutionDuration.NewTimer())
            {
                try
                {
                    // Core processing logic...

                    // Track success counter
                    ProcessedSalariesCounter.WithLabels("success").Inc();
                }
                catch (Exception ex)
                {
                    // Track failure counter
                    ProcessedSalariesCounter.WithLabels("failed").Inc();
                    Log.Error(ex, "Failed to process monthly salaries.");
                    throw;
                }
            }
        }
    }
}
