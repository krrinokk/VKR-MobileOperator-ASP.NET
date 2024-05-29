using Microsoft.AspNetCore.Mvc;
using OperatorMO_ASPNET.DAL.Repository;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OperatorMO_ASPNET.DAL.Models;
using BLL.Interfaces;
using static OperatorMO_ASPNET.Controllers.TariffController;
using static OperatorMO_ASPNET.Controllers.ServicesController;

namespace OperatorMO_ASPNET.Controllers
{
    // Указываем маршрут для данного контроллера
    [Route("api/[controller]")]
    // Указываем, что данный класс является контроллером API
    [ApiController]
    public class ServicesController : ControllerBase
    {
        // Интерфейс для работы с базой данных
        private readonly IDbCrud _crud;
        // Логгер для записи информации о работе приложения
        private readonly ILogger _logger;

        // Конструктор с параметрами
        public ServicesController(IDbCrud newIDbCrud, ILogger<ServicesController> logger)
        {
            _crud = newIDbCrud;
            _logger = logger;
        }
        private IActionResult NoPermissionsResponse()
        {
            return BadRequest(new { message = "У вас нет прав" });
        }
        // Метод для получения всех Services
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Services>>> GetAllServices()
        {


            // Записываем информацию о том, что был вызван данный метод
            _logger.LogInformation("You have moved to ServicesController, to the GetAllServices() method");
            // Получаем текущее время
            DateTime.UtcNow.ToLongTimeString();
            // Получаем все из базы данных
            var Services = from s in _crud.GetAllServices() select s;
            // Возвращаем список Tariffов
            return Services.ToList();



        }

        public class ServicesPopularity
        {
            public string Name { get; set; }
            public int ConnectionCount { get; set; }
            public int PopularityPercentage { get; set; }
        }

        public class ServicesPopularityResult
        {
            public List<ServicesPopularity> ServicesPopularity { get; set; }
            public decimal AverageCost { get; set; }
        }


        [HttpGet("services/popularity")]
        public ActionResult<ServicesPopularity> GetServicesPopularity()
        {
            try
            {
           

                // Получаем общее количество контрактов
                var totalCount = _crud.GetAllServicesConnected().Count();

                // Получаем общую стоимость всех услуг за указанный период
                var totalCost = _crud.GetAllServicesConnected()
                    .Sum(c => _crud.GetServices(c.ServiceId_FK)?.Cost ?? 0);

                // Рассчитываем среднюю стоимость услуги
                var averageCost = totalCount > 0 ? totalCost / totalCount : 0;

                // Получаем популярность тарифов, подсчитывая количество контрактов для каждого тарифа
                var servicesPopularity = _crud.GetAllServicesConnected()
                    .GroupBy(c => c.ServiceId_FK)
                    .Select(g => new ServicesPopularity
                    {
                       Name = _crud.GetServices(g.Key)?.Name, // Получаем имя тарифа по его идентификатору
                        ConnectionCount = g.Count(),
                        PopularityPercentage = (int)((double)g.Count() / totalCount * 100) // Преобразуем одно из чисел в тип double
                    })
                    .OrderByDescending(tp => tp.ConnectionCount)
                    .ToList();

                // Создаем объект TariffPopularityResult, который содержит список популярности тарифов, общее количество подключений и среднее количество подключений в день
                var result = new ServicesPopularityResult
                {
                    ServicesPopularity = servicesPopularity,
                    AverageCost = averageCost
                };

                return Ok(result); ;
            }
            catch (Exception ex)
            {
                // Логируем ошибку и возвращаем ошибку сервера
                _logger.LogError(ex, "An error occurred while getting services popularity");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while getting services popularity");
            }
        }



        public class ServicesPopularityDay
        {
            public string Name { get; set; }
            public int ConnectionCount { get; set; }
            public int PopularityPercentage { get; set; }
            
            public decimal AverageCost { get; set; }
        }
        public class ServicesPopularityDayResult
        {
            public List<ServicesPopularityDay> ServicesPopularityDay { get; set; }
            public decimal AverageCost { get; set; }
          
        }

        [HttpGet("services/popularity-day")]
        public ActionResult<ServicesPopularityDay> GetServicesPopularityDay(DateTime startDate, DateTime endDate)
        {
            try
            {
                // Получаем общее количество контрактов за указанный период
                var totalCount = _crud.GetAllServicesConnected()
                    .Count(c => c.DateConnection >= startDate && c.DateConnection <= endDate);

                // Получаем общую стоимость всех услуг за указанный период
                var totalCost = _crud.GetAllServicesConnected()
                    .Where(c => c.DateConnection >= startDate && c.DateConnection <= endDate)
                    .Sum(c => _crud.GetServices(c.ServiceId_FK)?.Cost ?? 0);

                // Рассчитываем среднюю стоимость услуги
                var averageCost = totalCount > 0 ? totalCost / totalCount : 0;

                // Получаем популярность услуг, подсчитывая количество контрактов для каждой услуги
                var servicesPopularityDay = _crud.GetAllServicesConnected()
                    .Where(c => c.DateConnection >= startDate && c.DateConnection <= endDate)
                    .GroupBy(c => c.ServiceId_FK)
                    .Select(g => new ServicesPopularityDay
                    {
                        Name = _crud.GetServices(g.Key)?.Name,
                        ConnectionCount = g.Count(),
                        PopularityPercentage = (int)((double)g.Count() / totalCount * 100),
                        AverageCost = averageCost // Добавляем среднюю стоимость в объект ServicesPopularityDay
                    })
                    .OrderByDescending(tp => tp.ConnectionCount)
                    .ToList();

                // Создаем объект TariffPopularityResult, который содержит список популярности тарифов, общее количество подключений и среднее количество подключений в день
                var result = new ServicesPopularityDayResult
                {
                    ServicesPopularityDay = servicesPopularityDay,
                    AverageCost = averageCost
                };

                return Ok(result); ;
            }
            catch (Exception ex)
            {
                // Логируем ошибку и возвращаем ошибку сервера
                _logger.LogError(ex, "An error occurred while getting services popularity");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while getting services popularity");
            }
        }












        // Метод для получения по идентификатору
        [HttpGet("{id}")]
        public async Task<ActionResult<OperatorMO_ASPNET.DAL.Models.Services>> GetServices(int id)
        {
            try
            {
                // Получаем Chat из базы данных по идентификатору
                var Services = _crud.GetServices(id);
                // Если Chat не найден, возвращаем ошибку
                if (Services == null)
                {
                    _logger.LogInformation($"Services with id {id} not found");
                    return NotFound();
                }
                // Если Chat найден, возвращаем его
                _logger.LogInformation($"Services with id {id} found: {Services}");
                return Services;
            }
            catch (Exception ex)
            {
                // Записываем информацию об ошибке в лог
                _logger.LogError(ex, $"An error occurred while getting Services with id {id}");
                // Возвращаем ошибку сервера
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while getting Services with id {id}");
            }
        }


        // Этот метод удаляет  с заданным id
        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteServices(int id)
        {
            try
            {
                if (!User.IsInRole("Администратор"))
                {
                    return NoPermissionsResponse();
                }
                // Получаем  с заданным id из Unit of Work
                var Services = _crud.GetServices(id);
                if (Services == null)
                {
                    // Если Client не найден, возвращаем ошибку 404 Not Found
                    return NotFound();
                }
                // Удаляем Client из Unit of Work
                _crud.DeleteServices(id);
                _crud.Save();
                _logger.LogInformation("Удален message с id " + Services.ServiceId);
                return NoContent();
            }
            catch (Exception ex)
            {
                // Если произошла ошибка, логируем ее и возвращаем ошибку 500 Internal Server Error
                _logger.LogError(ex, "Ошибка при удалении Services");
                return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка при удалении Services");
            }
        }


        // Контроллер для обновления Clientа с определенным id
        [HttpPut("{serviceId}")]

        public async Task<IActionResult> UpdateServices(int serviceId, OperatorMO_ASPNET.DAL.Models.Services Services)
        {
            try
            {
                // Проверяем, что id Clientа совпадает с переданным в параметрах
                if (serviceId != Services.ServiceId)
                {
                    return BadRequest();
                }
                // Вызываем метод для обновления Clientа в базе данных
                _crud.UpdateServices(Services);
                // Логируем информацию об изменении Clientа
                _logger.LogInformation("Изменен Services с id " + Services.ServiceId);
                try
                {
                    // Сохраняем изменения в базе данных
                    _crud.Save();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Если Client уже был изменен другим пользователем, возвращаем ошибку
                    if (!ServicesExists(serviceId))
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
                _logger.LogError(ex, $"An error occurred while updating Services with id {serviceId}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating Services with id {serviceId}");
            }
        }

        // Контроллер для создания нового Message
        [HttpPost]

        public IActionResult CreateServices(OperatorMO_ASPNET.DAL.Models.Services Services)
        {
            try
            {
               
                // Проверяем валидность модели
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var services = _crud.GetClient(Services.ServiceId);


                // Создаем новый Client в базе данных
                _crud.UpdateServices(Services);
                // Сохраняем изменения в базе данных
                _crud.Save();

                // Возвращаем успешный результат с информацией о созданном Clientе
                return CreatedAtAction("GetMessage", new { id = Services.ServiceId }, services);
            }
            catch (Exception ex)
            {
                // Логируем ошибку и возвращаем ошибку сервера
                _logger.LogError(ex, "An error occurred while creating a new services");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating a new services");
            }
        }

        // Этот метод проверяет, существует ли Chat с заданным id
        private bool ServicesExists(int id)
        {
            //return _context.CategoryTables.Any(e => e.CategoryId == id);
            return _crud.GetServices(id) != null;
        }

     
    }
}