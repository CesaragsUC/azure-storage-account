using Azure.Storage.Queues;
using System.Text.Json;

namespace Storage.Account.Demo.Services
{
    public class QueueService : IQueueService
    {
        private readonly QueueClient _queueClient;

        public QueueService( QueueClient queueClient)
        {
            _queueClient = queueClient;
        }

        public async Task SendMessage(EmailDto email)
        {

            await _queueClient.CreateIfNotExistsAsync();

            var message = JsonSerializer.Serialize(email);

            await _queueClient.SendMessageAsync(message);
        }
    }
}
