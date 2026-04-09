using System;
using System.Collections.Generic;

namespace SalaryScheduler.Models;

public partial class EmailSetting
{
    public int Id { get; set; }

    public string? SmtpServer { get; set; }

    public int? SmtpPort { get; set; }

    public string? SenderEmail { get; set; }

    public string? SenderPassword { get; set; }

    public bool? EnableSsl { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }
}
