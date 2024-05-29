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
    public class CategoryTransactionController : ControllerBase
    {
        // Интерфейс для работы с базой данных
        private readonly IDbCrud _crud;
        // Логгер для записи информации о работе приложения
        private readonly ILogger _logger;

        // Конструктор с параметрами
        public CategoryTransactionController(IDbCrud newIDbCrud, ILogger<ChatController> logger)
        {
            _crud = newIDbCrud;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryTransaction>>> GetAllCategory()
        {
            // Записываем информацию о том, что был вызван данный метод
            _logger.LogInformation("You have moved to TransactionsController, to the GetAllTransactions() method");
            // Получаем текущее время
            DateTime.UtcNow.ToLongTimeString();
            // Получаем все Userы из базы данных
            var CategoryTransaction = from s in _crud.GetAllCategory() select s;
            // Возвращаем список Userов
            return CategoryTransaction.ToList();

        }




    }
}