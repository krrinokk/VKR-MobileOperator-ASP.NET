using Microsoft.AspNetCore.Mvc;
using OperatorMO_ASPNET.DAL.Repository;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OperatorMO_ASPNET.DAL.Models;
using BLL.Interfaces;
using Microsoft.AspNetCore.Cors;

namespace OperatorMO_ASPNET.Controllers
{
    // Указываем маршрут для данного контроллера
    [Route("api/[controller]")]
    [EnableCors]
  
    // Указываем, что данный класс является контроллером API
    [ApiController]
    public class TariffController : ControllerBase
    {
        // Интерфейс для работы с базой данных
        private readonly IDbCrud _crud;
        // Логгер для записи информации о работе приложения
        private readonly ILogger _logger;

        // Конструктор с параметрами
        public TariffController(IDbCrud newIDbCrud, ILogger<TariffController> logger)
        {
            _crud = newIDbCrud;
            _logger = logger;
        }
        private IActionResult NoPermissionsResponse()
        {
            return BadRequest(new { message = "У вас нет прав" });
        }

        // Метод для получения всех Tariffов
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tariff>>> GetAllTariff()
        {
            
           
                // Записываем информацию о том, что был вызван данный метод
                _logger.LogInformation("You have moved to TariffController, to the GetAllTariff() method");
                // Получаем текущее время
                DateTime.UtcNow.ToLongTimeString();
                // Получаем все Tariffы из базы данных
                var Tariff = from s in _crud.GetAllTariff() select s;
                // Возвращаем список Tariffов
                return Tariff.ToList();
            
         

        }

        // Метод для получения Tariffа по идентификатору
        [HttpGet("{id}")]
        public async Task<ActionResult<OperatorMO_ASPNET.DAL.Models.Tariff>> GetTariff(int id)
        {
            try
            {
                // Получаем Tariff из базы данных по идентификатору
                var Tariff = _crud.GetTariff(id);
                // Если Tariff не найден, возвращаем ошибку
                if (Tariff == null)
                {
                    _logger.LogInformation($"Tariff with id {id} not found");
                    return NotFound();
                }
                // Если Tariff найден, возвращаем его
                _logger.LogInformation($"Tariff with id {id} found: {Tariff}");
                return Tariff;
            }
            catch (Exception ex)
            {
                // Записываем информацию об ошибке в лог
                _logger.LogError(ex, $"An error occurred while getting Tariff with id {id}");
                // Возвращаем ошибку сервера
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while getting Tariff with id {id}");
            }
        }
        // Контроллер для обновления Tariffа с определенным id
        [HttpPut("{tariffId}")]
        //[Authorize(Roles = "user")]
        //[Authorize(Roles = "Администратор")]
        public async Task<IActionResult> UpdateTariff(int tariffId, OperatorMO_ASPNET.DAL.Models.Tariff Tariff)
        {
            try
            {
             
                // Проверяем, что id Tariffа совпадает с переданным в параметрах
                if (tariffId != Tariff.TariffId)
                {
                    return BadRequest();
                }
                // Вызываем метод для обновления Tariffа в базе данных
                _crud.UpdateTariff(Tariff);
                // Логируем информацию об изменении Tariffа
                _logger.LogInformation("Изменен Tariff с id " + Tariff.TariffId);
                try
                {
                    // Сохраняем изменения в базе данных
                    _crud.Save();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Если Tariff уже был изменен другим пользователем, возвращаем ошибку
                    if (!TariffExists(tariffId))
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
                _logger.LogError(ex, $"An error occurred while updating Tariff with id {tariffId}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating Tariff with id {tariffId}");
            }

        }

        // Контроллер для создания нового Tariffа


[HttpPost]
        // [Authorize(Roles = "user")]
        //[Authorize(Roles = "Администратор")]
        public IActionResult CreateTariff(Tariff tariff)
    {
        try
        {
                //if (!User.IsInRole("Администратор"))
                //{
                //    return NoPermissionsResponse();
                //}
                // Проверяем валидность модели
                if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Установка значения DateOpening на сегодняшнюю дату
            tariff.DateOpening = DateTime.Today;

            // Создаем новый Tariff в базе данных
            _crud.CreateTariff(tariff);
            // Сохраняем изменения в базе данных
            _crud.Save();

            // Возвращаем успешный результат с информацией о созданном тарифе
            return CreatedAtAction("GetTariff", new { id = tariff.TariffId }, tariff);
        }
        catch (Exception ex)
        {
            // Логируем ошибку и возвращаем ошибку сервера
            _logger.LogError(ex, "An error occurred while creating a new tariff");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating a new tariff");
        }
    }





    // Этот метод проверяет, существует ли Tariff с заданным id
    private bool TariffExists(int id)
        {
            //return _context.CategoryTables.Any(e => e.CategoryId == id);
            return _crud.GetTariff(id) != null;
        }

        // Этот метод удаляет Tariff с заданным id
        [HttpDelete("{id}")]
        //[Authorize(Roles = "user")]
      
        public async Task<IActionResult> DeleteTariff(int id)
        {
            try
            {
                if (!User.IsInRole("Администратор"))
                {
                    return NoPermissionsResponse();
                }
                // Получаем Tariff с заданным id из Unit of Work
                var Tariff = _crud.GetTariff(id);
                if (Tariff == null)
                {
                    // Если Tariff не найден, возвращаем ошибку 404 Not Found
                    return NotFound();
                }
                // Удаляем Tariff из Unit of Work
                _crud.DeleteTariff(id);
                _crud.Save();
                _logger.LogInformation("Удален Tariff с id " + Tariff.TariffId);
                return NoContent();
            }
            catch (Exception ex)
            {
                // Если произошла ошибка, логируем ее и возвращаем ошибку 500 Internal Server Error
                _logger.LogError(ex, "Ошибка при удалении Tariffа");
                return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка при удалении Tariffа");
            }
        }



        public class TariffPopularity
        {
            public string TariffName { get; set; }
            public int ConnectionCount { get; set; }
            public int PopularityPercentage { get; set; }
        }
        public class TariffPopularityDay
        {
            public string TariffName { get; set; }
            public int ConnectionCount { get; set; }
            public int PopularityPercentage { get; set; }
        }
        public class TariffPopularityResult
        {
            public List<TariffPopularity> TariffPopularity { get; set; }
            public int TotalConnections { get; set; }
            public double AverageConnectionsPerDay { get; set; }
            public decimal AverageCost { get; set; }
        }
        public class TariffPopularityResultDay
        {
            public List<TariffPopularity> TariffPopularityDay { get; set; }
            public int TotalConnectionsDay { get; set; }
            public double AverageConnectionsPerDayDay { get; set; }

            public decimal AverageCostDay { get; set; }
            public decimal AverageCost { get; set; }
        }
        [HttpGet("tariff/popularity")]
        public ActionResult<TariffPopularityResult> GetTariffPopularity()
        {
            try
            {
                // Получаем текущую дату
                DateTime dateTimeToday = DateTime.Today;

                // Получаем общее количество контрактов
                var totalContractsCount = _crud.GetAllContract().Count();

                // Получаем самую раннюю дату завершения контракта
                DateTime earliestConclusionDate = _crud.GetAllContract().Min(c => c.DateConclusion);

                // Вычисляем общее количество дней работы
                int totalDaysOfWork = (dateTimeToday - earliestConclusionDate).Days;

                // Получаем общую стоимость всех контрактов
                var totalCost = _crud.GetAllContract()
                    .Sum(c => _crud.GetTariff(c.TariffId)?.Cost ?? 0);

                // Рассчитываем среднюю стоимость контракта
                var averageCost = totalContractsCount > 0 ? totalCost / totalContractsCount : 0;

                // Получаем популярность тарифов, подсчитывая количество контрактов для каждого тарифа
                var tariffPopularity = _crud.GetAllContract()
                    .GroupBy(c => c.TariffId)
                    .Select(g => new TariffPopularity
                    {
                        TariffName = _crud.GetTariff(g.Key)?.Name, // Получаем имя тарифа по его идентификатору
                        ConnectionCount = g.Count(),
                        PopularityPercentage = (int)((double)g.Count() / totalContractsCount * 100), // Преобразуем одно из чисел в тип double
                      
                    })
                    .OrderByDescending(tp => tp.ConnectionCount)
                    .ToList();

                // Вычисляем общее количество подключений
                var totalConnections = tariffPopularity.Sum(tp => tp.ConnectionCount);

                // Вычисляем среднее количество подключений в день
                double averageConnectionsPerDay = totalConnections / (double)totalDaysOfWork;

                // Создаем объект TariffPopularityResult, который содержит список популярности тарифов, общее количество подключений и среднее количество подключений в день
                var result = new TariffPopularityResult
                {
                    TariffPopularity = tariffPopularity,
                    TotalConnections = totalConnections,
                    AverageConnectionsPerDay = averageConnectionsPerDay,
                    AverageCost= averageCost
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Логируем ошибку и возвращаем ошибку сервера
                _logger.LogError(ex, "An error occurred while getting tariff popularity");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while getting tariff popularity");
            }
        }




        [HttpGet("tariff/popularity/day")]
        public ActionResult<IEnumerable<TariffPopularity>> GetTariffPopularityDay(DateTime date)
        {
            try
            {


                // Получаем общее количество контрактов
                var totalContractsCount = _crud.GetAllContract().Count();

                // Получаем самую раннюю дату завершения контракта
           
            

                // Получаем общую стоимость всех контрактов
                var totalCost = _crud.GetAllContract()
                    .Sum(c => _crud.GetTariff(c.TariffId)?.Cost ?? 0);

                // Рассчитываем среднюю стоимость контракта
                var averageCost = totalContractsCount > 0 ? totalCost / totalContractsCount : 0;






                // Получаем текущую дату
                DateTime dateTimeToday = DateTime.Today;
                // Получаем самую раннюю дату завершения контракта
                DateTime earliestConclusionDate = _crud.GetAllContract().Min(c => c.DateConclusion);

                // Вычисляем общее количество дней работы
                int totalDaysOfWork = (dateTimeToday - earliestConclusionDate).Days;

                // Получаем популярность тарифов, подсчитывая количество контрактов для каждого тарифа
                var tariffPopularity = _crud.GetAllContract()
                    .GroupBy(c => c.TariffId)
                    .Select(g => new TariffPopularity
                    {
                        TariffName = _crud.GetTariff(g.Key)?.Name, // Получаем имя тарифа по его идентификатору
                        ConnectionCount = g.Count(),
                      
                    })
                    .OrderByDescending(tp => tp.ConnectionCount)
                    .ToList();

                // Вычисляем общее количество подключений
                var totalConnections = tariffPopularity.Sum(tp => tp.ConnectionCount);

                // Вычисляем среднее количество подключений в день
                double averageConnectionsPerDay = totalConnections / (double)totalDaysOfWork;

                // Получаем общее количество контрактов на заданную дату
                var totalContractsCountDay = _crud.GetAllContract()
                    .Where(c => c.DateConclusion == date) // Фильтруем контракты по дате
                    .Count();
                // Получаем общую стоимость всех услуг за указанный период
                var totalCostDay = _crud.GetAllContract()
                    .Where(c => c.DateConclusion == date)
                 .Sum(c => _crud.GetTariff(c.TariffId)?.Cost ?? 0);

                // Рассчитываем среднюю стоимость услуги
                var averageCostDay = totalContractsCountDay > 0 ? totalCostDay / totalContractsCountDay : 0;

                // Получаем популярность тарифов, подсчитывая количество контрактов для каждого тарифа на заданную дату
                var tariffPopularityDay = _crud.GetAllContract()
                    .Where(c => c.DateConclusion == date) // Фильтруем контракты по дате
                    .GroupBy(c => c.TariffId)
                    .Select(g => new TariffPopularity
                    {
                        TariffName = _crud.GetTariff(g.Key)?.Name, // Получаем имя тарифа по его идентификатору
                        ConnectionCount = g.Count(),
                        PopularityPercentage = (int)((double)g.Count() / totalContractsCount * 100) // Преобразуем одно из чисел в тип double
                    })
                    .OrderByDescending(tp => tp.ConnectionCount)
                    .ToList();
                // Вычисляем общее количество подключений
                var totalConnectionsDay = tariffPopularityDay.Sum(tp => tp.ConnectionCount);

                // Создаем объект TariffPopularityResult, который содержит список популярности тарифов, общее количество подключений и среднее количество подключений в день
                var result = new TariffPopularityResultDay
                {
                    TariffPopularityDay = tariffPopularityDay,
                    TotalConnectionsDay = totalConnectionsDay,
                    AverageConnectionsPerDayDay = averageConnectionsPerDay,
                   AverageCostDay = averageCostDay,
                   AverageCost= averageCost
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Логируем ошибку и возвращаем ошибку сервера
                _logger.LogError(ex, "An error occurred while getting tariff popularity");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while getting tariff popularity");
            }
        }



        // Метод для получения тарифов, с DateOpening которых не прошел год
        [HttpGet("tariffs/active")]
        public async Task<ActionResult<IEnumerable<Tariff>>> GetExpiringTariffs()
        {
            try
            {
                // Получаем текущую дату
                var currentDate = DateTime.Today;

                // Вычисляем дату, которая была год назад от текущей даты
                var oneYearAgo = currentDate.AddYears(-1);

                // Получаем тарифы, у которых DateOpening больше чем дата, которая была год назад
                var expiringTariffs = _crud.GetAllTariff()
                    .Where(t => t.DateOpening > oneYearAgo)
                    .ToList();

                return expiringTariffs;
            }
            catch (Exception ex)
            {
                // Логируем ошибку и возвращаем ошибку сервера
                _logger.LogError(ex, "An error occurred while getting expiring tariffs");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while getting expiring tariffs");
            }
        }











    }
}