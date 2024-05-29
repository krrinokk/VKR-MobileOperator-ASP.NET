using Microsoft.AspNetCore.Mvc;
using OperatorMO_ASPNET.DAL.Repository;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OperatorMO_ASPNET.DAL.Models;
using BLL.Interfaces;
using System.Text;
using System.Security.Cryptography;

namespace OperatorMO_ASPNET.Controllers
{
    // Указываем маршрут для данного контроллера
    [Route("api/[controller]")]
    // Указываем, что данный класс является контроллером API
    [ApiController]
    public class UserController : ControllerBase
    {
        // Интерфейс для работы с базой данных
        private readonly IDbCrud _crud;
        // Логгер для записи информации о работе приложения
        private readonly ILogger _logger;

        // Конструктор с параметрами
        public UserController(IDbCrud newIDbCrud, ILogger<UserController> logger)
        {
            _crud = newIDbCrud;
            _logger = logger;
        }

        // Метод для получения всех Userов
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUser()
        {


            // Записываем информацию о том, что был вызван данный метод
            _logger.LogInformation("You have moved to UserController, to the GetAllUser() method");
            // Получаем текущее время
            DateTime.UtcNow.ToLongTimeString();
            // Получаем все Userы из базы данных
            var User = from s in _crud.GetAllUser() select s;
            // Возвращаем список Userов
            return User.ToList();



        }

        // Метод для получения Userа по идентификатору
        [HttpGet("{id}")]
        public async Task<ActionResult<OperatorMO_ASPNET.DAL.Models.User>> GetUser(int id)
        {
            try
            {
                // Получаем User из базы данных по идентификатору
                var User = _crud.GetUser(id);
                // Если User не найден, возвращаем ошибку
                if (User == null)
                {
                    _logger.LogInformation($"User with id {id} not found");
                    return NotFound();
                }
                // Если User найден, возвращаем его
                _logger.LogInformation($"User with id {id} found: {User}");
                return User;
            }
            catch (Exception ex)
            {
                // Записываем информацию об ошибке в лог
                _logger.LogError(ex, $"An error occurred while getting User with id {id}");
                // Возвращаем ошибку сервера
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while getting User with id {id}");
            }
        }
        // Контроллер для обновления Userа с определенным id
        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateUser(int id, OperatorMO_ASPNET.DAL.Models.User User)
        {
            try
            {
                // Проверяем, что id Userа совпадает с переданным в параметрах
                if (id != User.UserId)
                {
                    return BadRequest();
                }
                // Вызываем метод для обновления Userа в базе данных
                _crud.UpdateUser(User);
                // Логируем информацию об изменении Userа
                _logger.LogInformation("Изменен User с id " + User.UserId);
                try
                {
                    // Сохраняем изменения в базе данных
                    _crud.Save();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Если User уже был изменен другим пользователем, возвращаем ошибку
                    if (!UserExists(id))
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
                _logger.LogError(ex, $"An error occurred while updating User with id {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating User with id {id}");
            }
        }


        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Преобразуем строку пароля в байтовый массив и вычисляем его хэш
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Преобразуем массив байтов в строку шестнадцатеричных чисел
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }



        // Контроллер для создания нового Userа
        [HttpPost]
        public IActionResult CreateUser(OperatorMO_ASPNET.DAL.Models.User user)
        {
            try
            {
                // Проверяем валидность модели
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Хэшируем пароль перед сохранением
                user.Password = HashPassword(user.Password);

                // Создаем нового пользователя в базе данных
                _crud.CreateUser(user);

                // Сохраняем изменения в базе данных
                _crud.Save();

                // Возвращаем успешный результат с информацией о созданном пользователе
                return CreatedAtAction("GetUser", new { id = user.UserId }, user);
            }
            catch (Exception ex)
            {
                // Логируем ошибку и возвращаем ошибку сервера
                _logger.LogError(ex, "An error occurred while creating a new User");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating a new User");
            }
        }




        // Этот метод проверяет, существует ли User с заданным id
        private bool UserExists(int id)
        {
            //return _context.CategoryTables.Any(e => e.CategoryId == id);
            return _crud.GetUser(id) != null;
        }

        // Этот метод удаляет User с заданным id
        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                // Получаем User с заданным id из Unit of Work
                var User = _crud.GetUser(id);
                if (User == null)
                {
                    // Если User не найден, возвращаем ошибку 404 Not Found
                    return NotFound();
                }
                // Удаляем User из Unit of Work
                _crud.DeleteUser(id);
                _crud.Save();
                _logger.LogInformation("Удален User с id " + User.UserId);
                return NoContent();
            }
            catch (Exception ex)
            {
                // Если произошла ошибка, логируем ее и возвращаем ошибку 500 Internal Server Error
                _logger.LogError(ex, "Ошибка при удалении Userа");
                return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка при удалении Userа");
            }
        }
        public class UserContractsCount
        {
            public int UserId { get; set; }
            public int ContractsCount { get; set; }
            public string? Name { get; set; }
            public List<string> PhoneNumbers { get; set; } // Изменяем тип на список строк
        }

        [HttpGet("contracts-count-day")]
        public ActionResult<IEnumerable<UserContractsCount>> GetUserContractsCount(DateTime startDate, DateTime endDate)
        {

            var contractsByUser = _crud.GetAllContract()
                .Where(c => c.DateConclusion >= startDate && c.DateConclusion <= endDate)
                .GroupBy(c => c.UserId_FK)
                .Select(g => new UserContractsCount
                {
                    UserId = g.Key,
                    Name = _crud.GetUser(g.Key)?.Name,
                    ContractsCount = g.Count(),
                    PhoneNumbers = g.Select(c => c.NumberPhone).ToList() // Извлекаем только номера телефонов
                })
                .OrderByDescending(u => u.ContractsCount)
                .ToList();

            return Ok(contractsByUser);
        }







        [HttpGet("/{userId}/contracts-count")] // Маршрут с параметром userId
        public ActionResult<UserContractsCount> GetUserContractsCountId(int userId)
        {
            var contractsCountForUser = _crud.GetAllContract()
                .Where(c => c.UserId_FK == userId) // Проверка на userId и дату завершения контракта
                .Count();

            var user = _crud.GetUser(userId);
            if (user == null)
            {
                return NotFound(); // Возвращаем 404, если пользователь не найден
            }

            var userContractsCount = new UserContractsCount
            {
                UserId = userId,
                Name = user.Name,
                ContractsCount = contractsCountForUser
              
            };

            return Ok(userContractsCount);
        }









  
    }
}
    
