
namespace Storage.Account.Demo.Services
{
    public interface IQueueService
    {
        Task SendMessage(EmailDto email);
    }
}