using Hangfire;
using Microsoft.EntityFrameworkCore;
using SalaryScheduler.Application.Services;
using SalaryScheduler.Hangfire.Jobs;
using SalaryScheduler.Models;
using SalaryScheduler.Repository;

var builder = WebApplication.CreateBuilder(args);

// DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddScoped<SalaryService>();
builder.Services.AddScoped<SalaryScheduler.BackgroundJobs.Jobs.SalaryJob>();
builder.Services.AddScoped<UtilityJobs>();
builder.Services.AddScoped<IEmailSettingsRepository,EmailSettingsRepository>();
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
    
   //Remove Old Jobs.
    RecurringJob.RemoveIfExists("email-notification-batch-job"); 

    // Email Notification Batch Job - Runs every 2 minutes
    RecurringJob.AddOrUpdate<UtilityJobs>(
        "email-notification-batch-job",
        job => job.SendSalaryNotification(1),
        "*/2 * * * *"  // Every 2 minutes
    );
}

// Root URL endpoint
app.MapGet("/", () => "Salary Scheduler is running...");


// Controllers
app.MapControllers();

app.Run();