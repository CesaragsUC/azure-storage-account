using Azure.Data.Tables;
using Storage.Account.Data;

namespace Storage.Account.Services
{
    public interface ITableStorageService
    {
        Task<List<Participante>> GetParticipantes();
        Task<Participante> GetParticipanteAsync(string atividade, string id);
        Task<TableEntity> GetEntityAsync(string partitionKey, string rowKey);
        Task AddPaticipante(Participante participante);
        Task DelePaticipante(string atividade, string id);
        Task UpdatePaticipante(Participante participante);
        Task UpdateTableEntity(TableEntity data);
    }
}
