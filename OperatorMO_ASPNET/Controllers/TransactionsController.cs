using Microsoft.AspNetCore.Mvc;
using OperatorMO_ASPNET.DAL.Repository;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OperatorMO_ASPNET.DAL.Models;
using BLL.Interfaces;
using System.Linq;
using static OperatorMO_ASPNET.Controllers.ContractController;

namespace OperatorMO_ASPNET.Controllers
{
    // Указываем маршрут для данного контроллера
    [Route("api/[controller]")]
    // Указываем, что данный класс является контроллером API
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        // Интерфейс для работы с базой данных
        private readonly IDbCrud _crud;
        // Логгер для записи информации о работе приложения
        private readonly ILogger _logger;

        // Конструктор с параметрами
        public TransactionsController(IDbCrud newIDbCrud, ILogger<UserController> logger)
        {
            _crud = newIDbCrud;
            _logger = logger;
        }

        // Метод для получения всех Userов
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transactions>>> GetAllTransactions()
        {


            // Записываем информацию о том, что был вызван данный метод
            _logger.LogInformation("You have moved to TransactionsController, to the GetAllTransactions() method");
            // Получаем текущее время
            DateTime.UtcNow.ToLongTimeString();
            // Получаем все Userы из базы данных
            var Transactions = from s in _crud.GetAllTransactions() select s;
            // Возвращаем список Userов
            return Transactions.ToList();



        }
        [HttpGet("{id}")]
        public IActionResult GetTransactions(int id)
        {
            try
            {
                var transaction = _crud.GetTransaction(id);
                if (transaction == null)
                {
                    return NotFound();
                }

                return Ok(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the Transactions");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the Transactions");
            }
        }


        [HttpGet("ByTariffId/{tariffId}")]
        public async Task<ActionResult<IEnumerable<TransactionsDTO>>> GetTransaction(int tariffId)
        {
            try
            {
                var transactions = from sc in _crud.GetAllTransactions()
                                        where sc.TariffId_FK == tariffId
                                   join s in _crud.GetAllCategory() on sc.CategoryTransaction_FK equals s.CategoryTransactionId
                                        select new TransactionsDTO
                                        {
                                            Name = s.Name,
                                            TariffId_FK = sc.TariffId_FK,
                                            CategoryTransaction_FK = sc.CategoryTransaction_FK,
                                            Cost = sc.Cost,
                                        };

                // Return the list of connected services with additional information
                return transactions.ToList();
            }
            catch (Exception ex)
            {
                // Записываем информацию об ошибке в лог
                _logger.LogError(ex, $"An error occurred while getting transactions for tariff with id {tariffId}");
                // Возвращаем ошибку сервера
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while getting transactions for tariff with id {tariffId}");
            }
        }
        [HttpPost]
        public IActionResult CreateTransactions(Transactions Transactions)
        {
            try
            {
             
                // Проверяем валидность модели
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

              
                _crud.CreateTransactions(Transactions);
                // Сохраняем изменения в базе данных
                _crud.Save();

                // Возвращаем успешный результат с информацией о созданном тарифе
                return CreatedAtAction("GetTransactions", new { id = Transactions.TransactionId }, Transactions);
            }
            catch (Exception ex)
            {
                // Логируем ошибку и возвращаем ошибку сервера
                _logger.LogError(ex, "An error occurred while creating a new Transactions");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating a new Transactions");
            }
        }


        public class TransactionsDTO
        {
            public string Name { get; set; }
          
            public int TariffId_FK { get; set; }
            public int CategoryTransaction_FK { get; set; }
            public decimal Cost { get; set; }
        }


    }
}