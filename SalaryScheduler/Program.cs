using Hangfire;
using Microsoft.EntityFrameworkCore;
using SalaryScheduler.Application.Services;
using SalaryScheduler.Domain.Entities.Infrastructure.Data;
using SalaryScheduler.Hangfire.Jobs;

var builder = WebApplication.CreateBuilder(args);

// DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddScoped<SalaryService>();
builder.Services.AddScoped<SalaryScheduler.BackgroundJobs.Jobs.SalaryJob>();
builder.Services.AddScoped<UtilityJobs>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Hangfire Configuration (requires valid SQL Server)
var hangfireConnection = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrEmpty(hangfireConnection))
{
    builder.Services.AddHangfire(config =>
        config.UseSqlServerStorage(hangfireConnection));
    
    builder.Services.AddHangfireServer();
}

builder.Services.AddControllers();

var app = builder.Build();

// Hangfire Dashboard (only if configured)
if (!string.IsNullOrEmpty(hangfireConnection))
{
    app.UseHangfireDashboard();
    
    // Optional Test Job
    RecurringJob.AddOrUpdate(
        "test-job",
        () => Console.WriteLine("Hangfire is working!"),
        Cron.Minutely
    );

    // Monthly Salary Processing Job
    RecurringJob.AddOrUpdate<SalaryScheduler.BackgroundJobs.Jobs.SalaryJob>(
        "monthly-salary-job",
        job => job.Execute(),
        "0 0 22 L * ?"  // Last day of month at 22:00
    );
}

// Root URL endpoint
app.MapGet("/", () => "Salary Scheduler is running...");

// Controllers
app.MapControllers();

app.Run();