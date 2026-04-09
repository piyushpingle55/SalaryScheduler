using Microsoft.EntityFrameworkCore;
using SalaryScheduler.Models;
using SalaryScheduler.Repository;

namespace SalaryScheduler.Repository
{
public class EmailSettingsRepository : IEmailSettingsRepository
{
    private readonly AppDbContext _context;

    public EmailSettingsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<EmailSetting?> GetActiveSettingsAsync()
    {
        return await _context.EmailSettings
            .Where(x => x.IsActive == true)
            .FirstOrDefaultAsync();
    }
}
}