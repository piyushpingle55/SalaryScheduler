using SalaryScheduler.Models;

namespace SalaryScheduler.Repository
{
    public interface IEmailSettingsRepository
{
    public  Task<EmailSetting?> GetActiveSettingsAsync();
}

}
