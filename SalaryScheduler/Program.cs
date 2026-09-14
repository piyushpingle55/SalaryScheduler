using Hangfire;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using SalaryScheduler.Application.Services;
using SalaryScheduler.Hangfire.Jobs;
using SalaryScheduler.Models;
using SalaryScheduler.Repository;
using Serilog;
using Serilog.Sinks.Grafana.Loki;

var builder = WebApplication.CreateBuilder(args);

// 1. Configure Serilog with Loki Sink
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("app", "SalaryScheduler")
    .WriteTo.Console()
    .WriteTo.GrafanaLoki("http://localhost:3100") // Loki service endpoint
    .CreateLogger();

builder.Host.UseSerilog();



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
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();
app.UseRouting();

// 2. Enable Prometheus HTTP Metrics tracking middleware
app.UseHttpMetrics();
app.UseAuthorization();

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

app.MapMetrics();
// Controllers
app.MapControllers();

app.Run();