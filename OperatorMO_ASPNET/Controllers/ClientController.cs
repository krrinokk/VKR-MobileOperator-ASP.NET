using Microsoft.AspNetCore.Mvc;
using OperatorMO_ASPNET.DAL.Repository;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OperatorMO_ASPNET.DAL.Models;
using BLL.Interfaces;

namespace OperatorMO_ASPNET.Controllers
{
    // Указываем маршрут для данного контроллера
    [Route("api/[controller]")]
    // Указываем, что данный класс является контроллером API
    [ApiController]
    public class ClientController : ControllerBase
    {
        // Интерфейс для работы с базой данных
        private readonly IDbCrud _crud;
        // Логгер для записи информации о работе приложения
        private readonly ILogger _logger;

        // Конструктор с параметрами
        public ClientController(IDbCrud newIDbCrud, ILogger<ClientController> logger)
        {
            _crud = newIDbCrud;
            _logger = logger;
        }

        private IActionResult NoPermissionsResponse()
        {
            return BadRequest(new { message = "У вас нет прав" });
        }


        // Метод для получения всех Clientов
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetAllClient()
        {
            
           
                // Записываем информацию о том, что был вызван данный метод
                _logger.LogInformation("You have moved to ClientController, to the GetAllClient() method");
                // Получаем текущее время
                DateTime.UtcNow.ToLongTimeString();
                // Получаем все Clientы из базы данных
                var Client = from s in _crud.GetAllClient() select s;
                // Возвращаем список Clientов
                return Client.ToList();
            
         

        }

        // Метод для получения Clientа по идентификатору
        [HttpGet("{id}")]
        public async Task<ActionResult<OperatorMO_ASPNET.DAL.Models.Client>> GetClient(int id)
        {
            try
            {
              
                // Получаем Client из базы данных по идентификатору
                var Client = _crud.GetClient(id);
                // Если Client не найден, возвращаем ошибку
                if (Client == null)
                {
                    _logger.LogInformation($"Client with id {id} not found");
                    return NotFound();
                }
                // Если Client найден, возвращаем его
                _logger.LogInformation($"Client with id {id} found: {Client}");
                return Client;
            }
            catch (Exception ex)
            {
                // Записываем информацию об ошибке в лог
                _logger.LogError(ex, $"An error occurred while getting Client with id {id}");
                // Возвращаем ошибку сервера
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while getting Client with id {id}");
            }
        }
        // Контроллер для обновления Clientа с определенным id
        [HttpPut("{id}")]
  
        public async Task<IActionResult> UpdateClient(int id, OperatorMO_ASPNET.DAL.Models.Client Client)
        {
            try
            {
               
                // Проверяем, что id Clientа совпадает с переданным в параметрах
                if (id != Client.ClientId)
                {
                    return BadRequest();
                }
                // Вызываем метод для обновления Clientа в базе данных
                _crud.UpdateClient(Client);
                // Логируем информацию об изменении Clientа
                _logger.LogInformation("Изменен Client с id " + Client.ClientId);
                try
                {
                    // Сохраняем изменения в базе данных
                    _crud.Save();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Если Client уже был изменен другим пользователем, возвращаем ошибку
                    if (!ClientExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                // Возвращаем успешный результат без содержимого
                return NoContent();
            }
            catch (Exception ex)
            {
                // Логируем ошибку и возвращаем ошибку сервера
                _logger.LogError(ex, $"An error occurred while updating Client with id {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating Client with id {id}");
            }
        }

        // Контроллер для создания нового Clientа
        [HttpPost]
   
        public IActionResult CreateClient(OperatorMO_ASPNET.DAL.Models.Client Client)
        {
            try
            {
             
                // Проверяем валидность модели
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

             
                var client = _crud.GetClient(Client.ClientId);
               

                // Создаем новый Client в базе данных
                _crud.CreateClient(Client);
                // Сохраняем изменения в базе данных
                _crud.Save();

                // Возвращаем успешный результат с информацией о созданном Clientе
                return CreatedAtAction("GetClient", new { id = Client.ClientId }, Client);
            }
            catch (Exception ex)
            {
                // Логируем ошибку и возвращаем ошибку сервера
                _logger.LogError(ex, "An error occurred while creating a new Client");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating a new Client");
            }
        }

 


        // Этот метод проверяет, существует ли Client с заданным id
        private bool ClientExists(int id)
        {
            //return _context.CategoryTables.Any(e => e.CategoryId == id);
            return _crud.GetClient(id) != null;
        }

        // Этот метод удаляет Client с заданным id
        [HttpDelete("{id}")]
    
        public async Task<IActionResult> DeleteClient(int id)
        {
            try
            {
               
                // Получаем Client с заданным id из Unit of Work
                var Client = _crud.GetClient(id);
                if (Client == null)
                {
                    // Если Client не найден, возвращаем ошибку 404 Not Found
                    return NotFound();
                }
                // Удаляем Client из Unit of Work
                _crud.DeleteClient(id);
                _crud.Save();
                _logger.LogInformation("Удален Client с id " + Client.ClientId);
                return NoContent();
            }
            catch (Exception ex)
            {
                // Если произошла ошибка, логируем ее и возвращаем ошибку 500 Internal Server Error
                _logger.LogError(ex, "Ошибка при удалении Clientа");
                return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка при удалении Clientа");
            }
        }
        // Метод для получения клиентов с балансом больше 0
        [HttpGet("clients/positive-balance")]
        public async Task<ActionResult<IEnumerable<Client>>> GetClientsWithPositiveBalance()
        {
            try
            {
                // Получаем клиентов из базы данных, у которых баланс больше 0
                var clientsWithPositiveBalance = _crud.GetAllClient()
                    .Where(c => c.Balance > 0)
                    .ToList();

                return clientsWithPositiveBalance;
            }
            catch (Exception ex)
            {
                // Логируем ошибку и возвращаем ошибку сервера
                _logger.LogError(ex, "An error occurred while getting clients with positive balance");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while getting clients with positive balance");
            }
        }
        public class ContractViewModel
        {
            public int ContractId { get; set; }
            public string NumberPhone { get; set; }
            public string Status { get; set; }
            public string TariffName { get; set; }
            public int TariffId { get; set; }
            public int? SMSRemaining { get; set; }
            public int? MinutesRemaining { get; set; }
            public int? GBRemaining { get; set; }
        }



        [HttpGet("contracts/{clientId}/related-contracts")]
        public async Task<ActionResult<IEnumerable<ContractViewModel>>> GetRelatedContracts(int clientId)
        {
            try
            {
                // Получаем все контракты из базы данных, у которых ClientId_FK совпадает с переданным clientId
                var relatedContracts = _crud.GetAllContract()
                    .Where(c => c.ClientId_FK == clientId)
                    .ToList();

                // Создаем список для хранения расширенной информации о контрактах
                var contractViewModels = new List<ContractViewModel>();

                // Проходимся по каждому контракту и формируем информацию для отображения
                foreach (var contract in relatedContracts)
                {
                    // Получаем информацию о связанном тарифе
                    var tariff = _crud.GetTariff(contract.TariffId);

                    // Формируем объект модели представления для контракта с информацией о тарифе
                    var contractViewModel = new ContractViewModel
                    {
                        ContractId = contract.ContractId,
                        Status = contract.Status,
                        NumberPhone = contract.NumberPhone,
                        // Другие поля контракта, если необходимо
                        TariffName = tariff?.Name, // Если тариф найден, то берем его название, иначе null
                        SMSRemaining = contract.SMSRemaining,
                        GBRemaining = contract.GBRemaining,
                        MinutesRemaining = contract.MinutesRemaining,
                        TariffId = contract.TariffId
                    };

                    // Добавляем объект модели представления в список
                    contractViewModels.Add(contractViewModel);
                }

                // Возвращаем список объектов модели представления
                return contractViewModels;
            }
            catch (Exception ex)
            {
                // Логируем ошибку и возвращаем ошибку сервера
                _logger.LogError(ex, $"An error occurred while getting related contracts for clientId {clientId}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while getting related contracts for clientId {clientId}");
            }
        }



    }
}