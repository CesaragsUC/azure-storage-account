using Azure;
using Azure.Data.Tables;
using System.Runtime.Serialization;

namespace Storage.Account.Data
{
    public class Participante : ITableEntity
    {
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public string Email { get; set; }
        public string Atividade { get; set; }
        public string ImageName { get; set; }

        [IgnoreDataMember]
        public string PropriedadeIgnorada { get; set; }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set ; }
    }
}
