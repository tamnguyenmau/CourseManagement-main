using System.Threading.Tasks;

namespace linq.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string body);

    }
}