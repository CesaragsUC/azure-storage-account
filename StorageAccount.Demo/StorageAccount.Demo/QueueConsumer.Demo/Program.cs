
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System.Text;

/// <summary>
/// Esse projeto console poderia ser um background service,para ficar rodando em segundo plano e consumir as mensagens da fila
/// Para nivel de Demonstração, foi criado um projeto console
/// </summary>

class QueueConsumer
{

    static QueueClient queue;

    ///
    /// 1.No cmd: setx AZURE_STORAGE_CONNECTION_STRING "your connection string azure storage"
    /// Isso ira criar uma variavel de ambiente com o nome AZURE_STORAGE_CONNECTION_STRING onde sera armazenado a connection string do azure storage
    /// pode ser que precise reinicar o PC para que a variavel de ambiente seja reconhecida
    /// 
    static readonly string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");

    static async Task Main(string[] args)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("[+] Iniciando o Consumidor de Mensagens");
        

        queue = new QueueClient(connectionString, "agsqueue");

        if (await queue.ExistsAsync())
        {
            QueueProperties properties = await queue.GetPropertiesAsync();
            for (int i=0;i < properties.ApproximateMessagesCount;i++)
            {
                string message = await ObterMensagens();
                Console.WriteLine($"Mensagem: {message}");

                //Usar a imagniação com o que fazer com a mensagens
                //Enviar Email
                //Enviar SMS
                //Enviar Notificação
                //Salvar no banco de dados
            }
        }

        
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("[+] Fim do processamento.");
        Console.ReadKey();
    }

    /// <summary>
    /// As mensagens foram salvas no modelo base64, por isso é necessario converter para string antes de exibir
    /// </summary>
    /// <returns></returns>
    static async Task<string> ObterMensagens()
    {
        QueueMessage[] messages = await queue.ReceiveMessagesAsync(1);
        var data = Convert.FromBase64String(messages[0].Body.ToString());
        string messageResult = Encoding.UTF8.GetString(data);

        // Deletando a mensagem da fila após a leitura
        await queue.DeleteMessageAsync(messages[0].MessageId, messages[0].PopReceipt);

        return messageResult;
    }
}