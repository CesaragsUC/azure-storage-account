using Azure;
using Microsoft.AspNetCore.Mvc;
using Storage.Account.Data;
using Storage.Account.Demo.Services;
using Storage.Account.Models;
using Storage.Account.Services;
using System.Diagnostics;

namespace Storage.Account.Controllers
{
    public class ParticipantesController : Controller
    {
        private readonly ILogger<ParticipantesController> _logger;
        private readonly ITableStorageService _tableStorageService;
        private readonly IBobStorageService _bobStorageService;
        private readonly IQueueService _queueService;

        public ParticipantesController(ILogger<ParticipantesController> logger,
            ITableStorageService tableStorageService,
            IBobStorageService bobStorageService,
            IQueueService queueService)
        {
            _logger = logger;
            _tableStorageService = tableStorageService;
            _bobStorageService = bobStorageService;
            _queueService = queueService;
        }

        public async Task<ActionResult> Index()
        {
            var dataList = new List<Participante>();

            var data = await _tableStorageService.GetParticipantes();
            foreach (var item in data)
            {
                item.ImageName = await _bobStorageService.GetBlobUrl(item.ImageName);
                dataList.Add(item);
            }

            return View(dataList);
        }

        public async Task<ActionResult> Details(string atividade, string id)
        {
            var data = await _tableStorageService.GetParticipanteAsync(atividade, id);
            data.ImageName = await _bobStorageService.GetBlobUrl(data.ImageName);
            return View(data);
        }
        public async Task<ActionResult> Create()
        {

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(Participante participante, IFormFile fileName)
        {

            participante.PartitionKey = participante.Atividade;
            participante.RowKey = Guid.NewGuid().ToString();

            if (fileName?.Length > 0)
                participante.ImageName = await _bobStorageService.UploadBlob(fileName, Guid.NewGuid().ToString());
            else
                participante.ImageName = "default.jpg";

            await _tableStorageService.AddPaticipante(participante);

            //envia uma mensagen para a fila
            await _queueService.SendMessage(new EmailDto
            {
                EmailAdress = participante.Email,
                TimeStamp = DateTime.UtcNow,
                Message = $"Ola {participante.Nome} {participante.Sobrenome}, obrigado por se cadastrar no nosso site." +
                $"\n\r Sua atividade é {participante.Atividade}" +
                $"\n\r Sesus dados foram salvos com sucesso."
            });

            return RedirectToAction(nameof(Index));
        }


        public async Task<ActionResult> Update(string atividade, string id)
        {
            var data = await _tableStorageService.GetParticipanteAsync(atividade, id);
            ViewBag.ImageUrl = await _bobStorageService.GetBlobUrl(data.ImageName);
            ViewBag.eTag = data.ETag;
            return View(data);
        }

        [HttpPost]
        public async Task<ActionResult> Update(Participante participante, IFormFile fileName,string partitionKey,
            string rowKey,string eTag)
        {
            if (fileName?.Length > 0)
                participante.ImageName = await _bobStorageService.UploadBlob(fileName, participante.RowKey, participante.ImageName);// retorna o nome da imagem

            await _tableStorageService.UpdatePaticipante(participante);

            //envia uma mensagen para a fila
            await _queueService.SendMessage(new EmailDto
            {
                EmailAdress = participante.Email,
                TimeStamp = DateTime.UtcNow,
                Message = $"Ola {participante.Nome} {participante.Sobrenome}, seus dados foram atualizados." +
                $"\n\r Sua nova atividade agora é {participante.Atividade}"
            });

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<ActionResult> Delete(string atividade, string id)
        {
            var data = await _tableStorageService.GetParticipanteAsync(atividade, id);

            if (data is null)
                return RedirectToAction(nameof(Index));

            await _tableStorageService.DelePaticipante(atividade, id);
            await _bobStorageService.Removelob(data.ImageName);

             //envia uma mensagen para a fila
            await _queueService.SendMessage(new EmailDto
            {
                EmailAdress = data.Email,
                TimeStamp = DateTime.UtcNow,
                Message = $"Ola {data.Nome} {data.Sobrenome}," +
                $"\n\r Sua conta foi excluida com sucesso"
            });

            return RedirectToAction(nameof(Index));
        }

    }
}
