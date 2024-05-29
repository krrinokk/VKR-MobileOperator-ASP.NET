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
    public class CostDetailsController : ControllerBase
    {
        // Интерфейс для работы с базой данных
        private readonly IDbCrud _crud;
        // Логгер для записи информации о работе приложения
        private readonly ILogger _logger;

        // Конструктор с параметрами
        public CostDetailsController(IDbCrud newIDbCrud, ILogger<CostDetailsController> logger)
        {
            _crud = newIDbCrud;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CostDetails>>> GetAllCostDetails()
        {
            try
            {
                _logger.LogInformation("You have moved to CostDetailsController, to the GetAllCostDetails() method");
                DateTime.UtcNow.ToLongTimeString();

                // Получаем все CostDetails из базы данных
                var costDetails = _crud.GetAllCostDetails().ToList();

                // Получаем все транзакции из базы данных
                var transactions = _crud.GetAllTransactions().ToList();

                // Проходим по каждому CostDetails
                foreach (var costDetail in costDetails)
                {
                    // Получаем контракт по ContractId_FK
                    var contract = _crud.GetContract(costDetail.ContractId_FK);
                    if (contract != null)
                    {
                        // Получаем TariffId_FK из контракта
                        var tariffId = contract.TariffId;

                        // Находим соответствующую транзакцию в Transactions
                        var matchingTransaction = transactions.FirstOrDefault(t => t.TariffId_FK == tariffId && t.CategoryTransaction_FK == costDetail.CategoryTransaction_FK);

                        // Если найдена соответствующая транзакция, обновляем CostDetails.Cost
                        if (matchingTransaction != null)
                        {
                            costDetail.Cost = matchingTransaction.Cost;
                        }
                    }
                }

                // Возвращаем список CostDetails с обновленными Cost
                return Ok(costDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in GetAllCostDetails: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet("{contractIdFK}")]
        public async Task<ActionResult<IEnumerable<CostDetailsDTO>>> GetCostDetailsByContractIdFK(int contractIdFK)
        {
            try
            {
                _logger.LogInformation($"You have moved to CostDetailsController, to the GetCostDetailsByContractIdFK({contractIdFK}) method");
                DateTime.UtcNow.ToLongTimeString();

                // Получаем все CostDetails из базы данных для указанного ContractId_FK
                var costDetails = _crud.GetAllCostDetails().Where(cd => cd.ContractId_FK == contractIdFK).ToList();

                // Получаем все транзакции из базы данных
                var transactions = _crud.GetAllTransactions().ToList();

                // Получаем все категории из базы данных
                var categories = _crud.GetAllCategoryTransaction().ToList();

                var costDetailsDTO = new List<CostDetailsDTO>();

                // Проходим по каждому CostDetails
                foreach (var costDetail in costDetails)
                {
                    // Получаем контракт по ContractId_FK
                    var contract = _crud.GetContract(costDetail.ContractId_FK);
                    if (contract != null)
                    {
                        // Получаем TariffId_FK из контракта
                        var tariffId = contract.TariffId;

                        // Находим соответствующую транзакцию в Transactions
                        var matchingTransaction = transactions.FirstOrDefault(t => t.TariffId_FK == tariffId && t.CategoryTransaction_FK == costDetail.CategoryTransaction_FK);

                        // Если найдена соответствующая транзакция, обновляем CostDetails.Cost
                        if (matchingTransaction != null)
                        {
                            costDetail.Cost = matchingTransaction.Cost;
                        }
                    }

                    // Получаем имя категории транзакции
                    var category = categories.FirstOrDefault(c => c.CategoryTransactionId == costDetail.CategoryTransaction_FK);
                    if (category != null)
                    {
                        costDetailsDTO.Add(new CostDetailsDTO
                        {
                            Id = costDetail.CostDetailId,
                            ContractId_FK = costDetail.ContractId_FK,
                            CategoryTransaction_FK = costDetail.CategoryTransaction_FK,
                            Cost = costDetail.Cost,
                            CategoryName = category.Name,
                            Date = costDetail.Date
                        }) ;
                    }
                }

                // Возвращаем список CostDetailsDTO с обновленными Cost и именами категорий
                return Ok(costDetailsDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in GetCostDetailsByContractIdFK: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        public class CostDetailsDTO
        {
            public int Id { get; set; }
            public int ContractId_FK { get; set; }
            public int CategoryTransaction_FK { get; set; }
            public DateTime Date { get; set; }
            public decimal? Cost { get; set; }
            public string CategoryName { get; set; } // Добавлено поле для названия категории
        }


        //// Контроллер для обновления Clientа с определенным id
        //[HttpPut("{id}")]

        //public async Task<IActionResult> UpdateClient(int id, OperatorMO_ASPNET.DAL.Models.Client Client)
        //{
        //    try
        //    {
        //        // Проверяем, что id Clientа совпадает с переданным в параметрах
        //        if (id != Client.ClientId)
        //        {
        //            return BadRequest();
        //        }
        //        // Вызываем метод для обновления Clientа в базе данных
        //        _crud.UpdateClient(Client);
        //        // Логируем информацию об изменении Clientа
        //        _logger.LogInformation("Изменен Client с id " + Client.ClientId);
        //        try
        //        {
        //            // Сохраняем изменения в базе данных
        //            _crud.Save();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            // Если Client уже был изменен другим пользователем, возвращаем ошибку
        //            if (!ClientExists(id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        // Возвращаем успешный результат без содержимого
        //        return NoContent();
        //    }
        //    catch (Exception ex)
        //    {
        //        // Логируем ошибку и возвращаем ошибку сервера
        //        _logger.LogError(ex, $"An error occurred while updating Client with id {id}");
        //        return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating Client with id {id}");
        //    }
        //}

        //// Контроллер для создания нового Clientа
        //[HttpPost]

        //public IActionResult CreateClient(OperatorMO_ASPNET.DAL.Models.Client Client)
        //{
        //    try
        //    {
        //        // Проверяем валидность модели
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }


        //        var client = _crud.GetClient(Client.ClientId);


        //        // Создаем новый Client в базе данных
        //        _crud.CreateClient(Client);
        //        // Сохраняем изменения в базе данных
        //        _crud.Save();

        //        // Возвращаем успешный результат с информацией о созданном Clientе
        //        return CreatedAtAction("GetClient", new { id = Client.ClientId }, Client);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Логируем ошибку и возвращаем ошибку сервера
        //        _logger.LogError(ex, "An error occurred while creating a new Client");
        //        return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating a new Client");
        //    }
        //}




        //// Этот метод проверяет, существует ли Client с заданным id
        //private bool ClientExists(int id)
        //{
        //    //return _context.CategoryTables.Any(e => e.CategoryId == id);
        //    return _crud.GetClient(id) != null;
        //}

        //// Этот метод удаляет Client с заданным id
        //[HttpDelete("{id}")]

        //public async Task<IActionResult> DeleteClient(int id)
        //{
        //    try
        //    {
        //        // Получаем Client с заданным id из Unit of Work
        //        var Client = _crud.GetClient(id);
        //        if (Client == null)
        //        {
        //            // Если Client не найден, возвращаем ошибку 404 Not Found
        //            return NotFound();
        //        }
        //        // Удаляем Client из Unit of Work
        //        _crud.DeleteClient(id);
        //        _crud.Save();
        //        _logger.LogInformation("Удален Client с id " + Client.ClientId);
        //        return NoContent();
        //    }
        //    catch (Exception ex)
        //    {
        //        // Если произошла ошибка, логируем ее и возвращаем ошибку 500 Internal Server Error
        //        _logger.LogError(ex, "Ошибка при удалении Clientа");
        //        return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка при удалении Clientа");
        //    }
        //}
    }
}