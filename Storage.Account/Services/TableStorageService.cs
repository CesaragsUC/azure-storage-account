using Azure;
using Azure.Data.Tables;
using Storage.Account.Data;

namespace Storage.Account.Services
{
    public class TableStorageService : ITableStorageService
    {

        private readonly TableClient  _tableClient;

        public TableStorageService(TableClient tableClient)
        {
            _tableClient = tableClient;
        }

        public async Task<List<Participante>> GetParticipantes()
        {
            try
            {
                Pageable<Participante> query = _tableClient.Query<Participante>();
                return query.ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public async Task<Participante> GetParticipanteAsync(string atividade, string id)
        {
            try
            {
                var query = await _tableClient.GetEntityAsync<Participante>(atividade, id);
                return query;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        //https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/tables/Azure.Data.Tables/samples/Sample5UpdateUpsertEntities.md
        public async Task<TableEntity> GetEntityAsync(string partitionKey, string rowKey)
        {
            try
            {
                var query = await _tableClient.GetEntityAsync<TableEntity>(partitionKey, rowKey);
                return query.Value;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public async Task AddPaticipante(Participante participante)
        {
            try
            {
                var result = await _tableClient.AddEntityAsync(participante);

                var initialETag = result.Headers.ETag.Value;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public async Task UpdatePaticipante(Participante participante)
        {
            try
            {
                await _tableClient.UpdateEntityAsync(participante, ETag.All);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        //https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/tables/Azure.Data.Tables/samples/Sample5UpdateUpsertEntities.md
        public async Task UpdateTableEntity(TableEntity data)
        {
            try
            {
                await _tableClient.UpdateEntityAsync(data, data.ETag);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public async Task DelePaticipante(string atividade, string id)
        {
            try
            {
                await _tableClient.DeleteEntityAsync(atividade, id);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

    }
}
