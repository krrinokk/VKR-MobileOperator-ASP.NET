using Microsoft.AspNetCore.Mvc;
using OperatorMO_ASPNET.DAL.Repository;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OperatorMO_ASPNET.DAL.Models;
using BLL.Interfaces;
using System.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics.Contracts;
using Microsoft.Extensions.Hosting;

namespace OperatorMO_ASPNET.Controllers
{
    // Указываем маршрут для данного контроллера
    [Route("api/[controller]")]
    // Указываем, что данный класс является контроллером API
    [ApiController]
    public class ContractController : ControllerBase
    {
        // Интерфейс для работы с базой данных
        private readonly IDbCrud _crud;
        // Логгер для записи информации о работе приложения
        private readonly ILogger _logger;

        // Конструктор с параметрами
        public ContractController(IDbCrud newIDbCrud, ILogger<ContractController> logger)
        {
            _crud = newIDbCrud;
            _logger = logger;
        }
        private IActionResult NoPermissionsResponse()
        {
            return BadRequest(new { message = "У вас нет прав" });
        }
        // Метод для получения всех Contractов
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DAL.Models.Contract>>> GetAllContract()
        {


            // Записываем информацию о том, что был вызван данный метод
            _logger.LogInformation("You have moved to ContractController, to the GetAllContract() method");
            // Получаем текущее время
            DateTime.UtcNow.ToLongTimeString();
            // Получаем все Contractы из базы данных
            var Contract = from s in _crud.GetAllContract() select s;
            // Возвращаем список Contractов
            return Contract.ToList();



        }

        public class ContractDTO
        {
            public string NumberPhone { get; set; }
            public int ContractId { get; set; }
            public string NameClient { get; set; }
            public string NameTariff { get; set; }
            public DateTime DateConclusion { get; set; }
            public string Status { get; set; }
        }



        [HttpGet("phone/{phoneNumber}")]
        public async Task<ActionResult<ContractDTO>> GetContractByPhoneNumber(string phoneNumber)
        {
            try
            {
                // Ищем контракт по номеру телефона в базе данных
                var contract = _crud.GetAllContract().FirstOrDefault(c => c.NumberPhone == phoneNumber);

                // Если контракт не найден, возвращаем ошибку 404
                if (contract == null)
                {
                    _logger.LogInformation($"Contract with phone number {phoneNumber} not found");
                    return NotFound();
                }

                // Получаем данные клиента
                var client = _crud.GetClient(contract.ClientId_FK);

                // Получаем данные тарифа
                var tariff = _crud.GetTariff(contract.TariffId);

                // Если клиент не найден, возвращаем ошибку 404
                if (client == null)
                {
                    _logger.LogInformation($"Client with id {contract.ClientId_FK} not found");
                    return NotFound();
                }

                // Если тариф не найден, возвращаем ошибку 404
                if (tariff == null)
                {
                    _logger.LogInformation($"Tariff with id {contract.TariffId} not found");
                    return NotFound();
                }

                // Формируем NameClient из полей имени клиента
                var nameClient = $"{client.FirstName} {client.Patronymic} {client.LastName}";

                // Преобразуем контракт в DTO
                var contractDTO = new ContractDTO
                {
                    NumberPhone = contract.NumberPhone,
                    ContractId = contract.ContractId,
                    NameClient = nameClient,
                    NameTariff = tariff.Name,
                    DateConclusion = contract.DateConclusion,
                    Status = contract.Status
                };

                // Если контракт найден, возвращаем его DTO
                _logger.LogInformation($"Contract with phone number {phoneNumber} found: {contract}");
                return contractDTO;
            }
            catch (Exception ex)
            {
                // Записываем информацию об ошибке в лог
                _logger.LogError(ex, $"An error occurred while getting Contract with phone number {phoneNumber}");
                // Возвращаем ошибку сервера
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while getting Contract with phone number {phoneNumber}");
            }
        }


        // Метод для получения Contractа по идентификатору
        [HttpGet("{id}")]
        public async Task<ActionResult<OperatorMO_ASPNET.DAL.Models.Contract>> GetContract(int id)
        {
            try
            {
                // Получаем Contract из базы данных по идентификатору
                var Contract = _crud.GetContract(id);
                // Если Contract не найден, возвращаем ошибку
                if (Contract == null)
                {
                    _logger.LogInformation($"Contract with id {id} not found");
                    return NotFound();
                }
                // Если Contract найден, возвращаем его
                _logger.LogInformation($"Contract with id {id} found: {Contract}");
                return Contract;
            }
            catch (Exception ex)
            {
                // Записываем информацию об ошибке в лог
                _logger.LogError(ex, $"An error occurred while getting Contract with id {id}");
                // Возвращаем ошибку сервера
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while getting Contract with id {id}");
            }
        }





        // Контроллер для обновления Contractа с определенным id
        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateContract(int id, OperatorMO_ASPNET.DAL.Models.Contract Contract)
        {
            try
            {

                // Проверяем, что контракт существует
                if (Contract == null)
                {
                    return NotFound($"Контракт  не найден");
                }

                // Проверяем, что статус контракта "Активный"
                if (Contract.Status != "Активный")
                {
                    return BadRequest($"Невозможно обновить контракт. Статус контракта должен быть 'Активный'");
                }

                // Проверяем, что id Contractа совпадает с переданным в параметрах
                if (id != Contract.ContractId)
                {
                    return BadRequest();
                }
                _crud.UpdateContract(Contract);

                // Вызываем метод для обновления Contractа в базе данных

                // Логируем информацию об изменении Contractа
                _logger.LogInformation("Изменен Contract с id " + Contract.ContractId);
                try
                {
                    // Сохраняем изменения в базе данных
                    _crud.Save();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Если Contract уже был изменен другим пользователем, возвращаем ошибку
                    if (!ContractExists(id))
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
                _logger.LogError(ex, $"An error occurred while updating Contract with id {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating Contract with id {id}");
            }
        }



        // Контроллер для обновления Contractа с определенным id
        [HttpPut("{id}/smena-tarifa")]

        public async Task<IActionResult> UpdateContractTariff(int id, OperatorMO_ASPNET.DAL.Models.Contract Contract)
        {
            try
            {
                
                // Проверяем, что контракт существует
                if (Contract == null)
                {
                    return NotFound($"Контракт  не найден");
                }

                // Проверяем, что статус контракта "Активный"
                if (Contract.Status != "Активный")
                {
                    return BadRequest($"Невозможно обновить контракт. Статус контракта должен быть 'Активный'");
                }

                // Проверяем, что id Contractа совпадает с переданным в параметрах
                if (id != Contract.ContractId)
                {
                    return BadRequest();
                }
                _crud.UpdateContract(Contract);

                // Вызываем метод для обновления Contractа в базе данных

                var tariff = _crud.GetTariff(Contract.TariffId);
                var client = _crud.GetClient(Contract.ClientId_FK);
                client.Balance = client.Balance - tariff.Cost;
                var newWriteOff = new WriteOffs
                {
                    ContractId_FK = Contract.ContractId,
                    DateWriteOff = DateTime.Today,
                    Sum = tariff.Cost
                };
                Contract.DateConnectionTariff = DateTime.Today;
                _crud.UpdateContract(Contract);
                _crud.UpdateClient(client);
                _crud.UpdateTariff(tariff);

                var events = new EventsContract
                {
                    ContractId_FK = Contract.ContractId,
                    Date = DateTime.Today,
                    CategoryEventId_FK = 2
                };

                // Добавляем новую запись в контекст базы данных
                _crud.CreateEvents(events);
                // Логируем информацию об изменении Contractа
                _logger.LogInformation("Изменен Contract с id " + Contract.ContractId);
                try
                {
                    // Сохраняем изменения в базе данных
                    _crud.Save();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Если Contract уже был изменен другим пользователем, возвращаем ошибку
                    if (!ContractExists(id))
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
                _logger.LogError(ex, $"An error occurred while updating Contract with id {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating Contract with id {id}");
            }
        }

        [HttpPost]
        public IActionResult CreateContract(OperatorMO_ASPNET.DAL.Models.Contract Contract)
        {
            try
            {
                // Проверяем валидность модели
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                // Проверяем, что номер телефона уникален
                var existingContract = _crud.GetAllContract().FirstOrDefault(c => c.NumberPhone == Contract.NumberPhone);

                if (existingContract != null)
                {
                    return BadRequest("Номер телефона уже используется другим контрактом");
                }

                var tariff = _crud.GetTariff(Contract.TariffId);
                if (tariff == null)
                {
                    _logger.LogError("Тариф с указанным идентификатором не найден");
                    return BadRequest("Тариф с указанным идентификатором не найден");
                }

                // Получаем Clientа по его номеру и проверяем, что у него достаточно средств на балансе
                var client = _crud.GetClient(Contract.ClientId_FK);
                if (client == null)
                {
                    _logger.LogError("Клиент с указанным идентификатором не найден");
                    return BadRequest("Клиент с указанным идентификатором не найден");
                }

                // Проверяем, прошло ли уже год с момента открытия тарифа
                var dateDifference = DateTime.Today.Subtract(tariff.DateOpening);
                if (dateDifference.TotalDays > 365)
                {
                    // Пишем "Тариф Архивный", так как не прошло год с момента открытия тарифа
                    return BadRequest("Тариф Архивный");
                }

                // Проверяем, что у клиента достаточно средств на балансе
                if (client.Balance <= 0)
                {
                    // Пишем "Абонент заблокирован!", так как баланс клиента меньше или равен нулю
                    return BadRequest("Абонент заблокирован!");
                }

                // Записываем значения SMS, Minutes и GB из тарифа в контракт
                Contract.SMSRemaining = tariff.SMS;
                Contract.MinutesRemaining = tariff.Minutes;
                Contract.GBRemaining = tariff.GB;
                Contract.Status = "Активный";
                // Устанавливаем текущую дату для поля DateConclusion
                Contract.DateConclusion = DateTime.Today;
                Contract.DateConnectionTariff = DateTime.Today;

                client.Balance -= tariff.Cost;
                _crud.UpdateClient(client);


                // Создаем новый Contract в базе данных
                _crud.CreateContract(Contract);
                // Сохраняем изменения в базе данных
                var newWriteOff = new WriteOffs
                {
                    ContractId_FK = Contract.ContractId,
                    DateWriteOff = DateTime.Today,
                    Sum = tariff.Cost
                };

                // Добавляем новую запись в контекст базы данных
                _crud.CreateWriteOffs(newWriteOff);


                var events = new EventsContract
                {
                    ContractId_FK = Contract.ContractId,
                    Date = DateTime.Today,
                    CategoryEventId_FK = 1
                };

                // Добавляем новую запись в контекст базы данных
                _crud.CreateEvents(events);


                _crud.Save();
                _crud.Save();

                // Возвращаем успешный результат с информацией о созданном Contractе
                return CreatedAtAction("GetContract", new { id = Contract.ContractId }, Contract);
            }
            catch (Exception ex)
            {
                // Логируем ошибку и возвращаем ошибку сервера
                _logger.LogError(ex, "Произошла ошибка при создании нового контракта");
                return StatusCode(StatusCodes.Status500InternalServerError, "Произошла ошибка при создании нового контракта");
            }
        }




        [HttpPost("{id}/charge-monthly-payment")]
        public async Task<IActionResult> ChargeMonthlyPayment(int id)
        {
            try
            {

                // Проверяем, существуют ли записи списаний для данного контракта за текущий месяц
                var writeOffsForCurrentMonth = _crud.GetWriteOffsForContract(id)
                    .Where(w => w.DateWriteOff.Month == DateTime.Today.Month && w.DateWriteOff.Year == DateTime.Today.Year)
                    .ToList();

                var contract = _crud.GetContract(id);
             
                // Проверяем, что контракт существует
                if (contract == null)
                {
                    return NotFound($"Контракт  не найден");
                }

                // Проверяем, что статус контракта "Активный"
                if (contract.Status != "Активный")
                {
                    return BadRequest($"Невозможно отключить услугу. Статус контракта должен быть 'Активный'");
                }
                // Если записи списаний за текущий месяц отсутствуют
                if (!writeOffsForCurrentMonth.Any())
                {

                    if (DateTime.Today.Day == contract.DateConnectionTariff.Day)
                    {
                        // Получаем общую сумму ежемесячной выплаты для контракта с заданным id
                        IActionResult totalPaymentResult = await GetTotalPayment(id);

                        if (!(totalPaymentResult is OkObjectResult totalPaymentObjectResult))
                        {
                            // Если не удалось получить общую сумму ежемесячной выплаты, возвращаем соответствующий результат
                            return totalPaymentResult;
                        }

                        // Получаем значение общей суммы ежемесячной выплаты
                        var totalPayment = (decimal)totalPaymentObjectResult.Value.GetType().GetProperty("TotalPayment").GetValue(totalPaymentObjectResult.Value);

                        // Получаем Contract с заданным id из Unit of Work

                        if (contract == null)
                        {
                            // Если Contract не найден, возвращаем ошибку 404 Not Found
                            return NotFound();
                        }


                        // Получаем список подключенных услуг для контракта
                        var connectedServices = await GetConnectedServices(id);

                        foreach (var service in connectedServices)
                        {
                            var currentMonthConnectedService = connectedServices.FirstOrDefault(s => s.DateConnection.Month == DateTime.Today.Month);

                            if (currentMonthConnectedService != null)
                            {
                                // Если услуга была подключена в текущем месяце, вычитаем ее стоимость из общей суммы ежемесячной выплаты
                                totalPayment -= currentMonthConnectedService.Cost;
                                _crud.Save();
                            }

                            if (currentMonthConnectedService == null)
                            {
                                // Парсим число из названия сервиса
                                var numberInName = int.Parse(Regex.Match(service.Name, @"\d+").Value);

                                // Обновляем соответствующее значение в контракте
                                if (service.Name.Contains("SMS"))
                                {
                                    contract.SMSRemaining = contract.SMSRemaining + numberInName;
                                    _crud.Save();

                                }
                                else if (service.Name.Contains("MIN"))
                                {
                                    contract.MinutesRemaining = contract.MinutesRemaining + numberInName;
                                    _crud.Save();

                                }
                                else if (service.Name.Contains("GB"))
                                {
                                    contract.GBRemaining = contract.GBRemaining + numberInName;
                                    _crud.Save();

                                }
                                _crud.Save();
                            }
                        }
                        // Получаем информацию о клиенте, связанном с этим контрактом
                        var client = _crud.GetClient(contract.ClientId_FK);
                        if (client == null)
                        {
                            // Если клиент не найден, возвращаем ошибку 404 Not Found
                            return NotFound();
                        }
                        var tariff = _crud.GetTariff(contract.TariffId);
                        if (tariff == null)
                        {
                            _logger.LogError("Тариф с указанным идентификатором не найден");
                            return BadRequest("Тариф с указанным идентификатором не найден");
                        }
                        // Выполняем списание общей ежемесячной выплаты с баланса клиента
                        if (client.Balance >= totalPayment)
                        {
                            // Если на балансе достаточно средств, списываем общую ежемесячную выплату
                            client.Balance -= totalPayment;
                            _crud.Save();
                            //Восполнение пакетов услуг, минут, гб и смс
                            //Перенос остатков
                            contract.SMSRemaining = contract.SMSRemaining + tariff.SMS;
                            _crud.Save();
                            contract.MinutesRemaining = contract.MinutesRemaining + tariff.Minutes;
                            _crud.Save();
                            contract.GBRemaining = contract.GBRemaining + tariff.GB;
                            _crud.Save();
                            var newWriteOff = new WriteOffs
                            {
                                ContractId_FK = id,
                                DateWriteOff = DateTime.Today,
                                Sum = totalPayment
                            };

                            // Добавляем новую запись в контекст базы данных
                            _crud.CreateWriteOffs(newWriteOff);
                            _crud.Save();


                            // Возвращаем успешный результат
                            return Ok("Ежемесячная плата успешно списана");
                        }
                        else
                        {
                            // Если на балансе недостаточно средств, возвращаем ошибку
                            return BadRequest("Недостаточно средств на балансе клиента");
                        }

                    }
                    else
                    {
                        // Если записи списаний за текущий месяц уже существуют, верните сообщение об этом
                        return BadRequest("Дата списания в этом месяце еще не наступила.");
                    }
                }
                else
                {
                    // Если записи списаний за текущий месяц уже существуют, верните сообщение об этом
                    return BadRequest("Списание уже было выполнено в этом месяце");
                }
            }




            catch (Exception ex)
            {
                // Если произошла ошибка, логируем ее и возвращаем ошибку 500 Internal Server Error
                _logger.LogError(ex, "Ошибка при списании ежемесячной платы за контракт");
                return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка при списании ежемесячной платы за контракт");
            }
        }
    








        // Этот метод проверяет, существует ли Contract с заданным id
        private bool ContractExists(int id)
        {
            //return _context.CategoryTables.Any(e => e.CategoryId == id);
            return _crud.GetContract(id) != null;
        }

        // Асинхронный метод для получения списка подключенных услуг для контракта
        [HttpGet("{id}/services")]
        public async Task<IEnumerable<ServiceInfo>> GetConnectedServices(int id)
        {
            try
            {
                // Retrieve the list of connected services with additional information from the Services table
                var connectedServices = from sc in _crud.GetAllServicesConnected()
                                        where sc.ContractId_FK == id
                                        join s in _crud.GetAllServices() on sc.ServiceId_FK equals s.ServiceId
                                        select new ServiceInfo
                                        {
                                            ServiceId = s.ServiceId,
                                            Name = s.Name,
                                            Description = s.Description,
                                            Cost = s.Cost
                                        };

                // Return the list of connected services with additional information
                return connectedServices.ToList();
            }
            catch (Exception ex)
            {
                // Log and return 500 Internal Server Error in case of an exception
                _logger.LogError(ex, $"An error occurred while fetching connected services for Contract with id {id}");
                throw; // Let the exception propagate
            }
        }
        // Асинхронный метод для получения списка подключенных услуг для контракта
        [HttpGet("{id}/services-to-connect")]
        public async Task<IEnumerable<ServiceInfo>> GetServicesToConnect(int id)
        {
            try
            {
                // Получаем все услуги
                var allServices = _crud.GetAllServices();

                // Получаем подключенные услуги для данного контракта
                var connectedServices = from sc in _crud.GetAllServicesConnected()
                                        where sc.ContractId_FK == id
                                        join s in allServices on sc.ServiceId_FK equals s.ServiceId
                                        select s;

                // Исключаем из списка всех услуг те, которые уже подключены
                var servicesToConnect = allServices.Except(connectedServices);

                // Преобразуем в информацию о сервисах, которую вы хотите вернуть
                var servicesInfoToConnect = servicesToConnect.Select(s => new ServiceInfo
                {
                    ServiceId = s.ServiceId,
                    Name = s.Name,
                    Description = s.Description,
                    Cost = s.Cost
                });

                // Возвращаем список неподключенных услуг
                return servicesInfoToConnect.ToList();
            }
            catch (Exception ex)
            {
                // Записываем ошибку в лог и возвращаем 500 Internal Server Error в случае исключения
                _logger.LogError(ex, $"An error occurred while fetching connected services for Contract with id {id}");
                throw; // Позволяем исключению распространиться дальше
            }
        }

        // Define a DTO (Data Transfer Object) to represent the service information
        public class ServiceInfo
        {
            public int ServiceId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Cost { get; set; }
            public DateTime DateConnection { get; set; }
        }


        // Метод для подключения услуги к контракту
        [HttpPost("{contractId}/connect-service/{serviceId}")]
        public async Task<IActionResult> ConnectServiceToContract(int contractId, int serviceId)
        {
            try
            {
               
                // Получаем список подключенных услуг для контракта
                var connectedServices = await GetConnectedServices(contractId);

                // Проверяем, есть ли уже запись о подключенной услуге для данного контракта
                if (connectedServices.Any(cs => cs.ServiceId == serviceId))
                {
                    // Если такая услуга уже подключена к контракту, возвращаем соответствующее сообщение
                    return BadRequest($"Услуга с номером {serviceId} уже подключена к контракту под номером {contractId}");
                }

                // Получаем информацию о услуге по ее идентификатору
                var service = _crud.GetServices(serviceId);
                if (service == null)
                {
                    // Если услуга не найдена, возвращаем ошибку 404 Not Found
                    return NotFound($"Услуга под номером {serviceId} не найдена");
                }
                // Получаем контракт
                var contract = _crud.GetContract(contractId);

                // Проверяем, что контракт существует
                if (contract == null)
                {
                    return NotFound($"Контракт с номером {contractId} не найден");
                }

                // Проверяем, что статус контракта "Активный"
                if (contract.Status != "Активный")
                {
                    return BadRequest($"Невозможно отключить услугу. Статус контракта должен быть 'Активный'");
                }

                // Создаем запись о подключении услуги к контракту
                var connectedService = new ServicesConnected
                {
                    ContractId_FK = contractId,
                    ServiceId_FK = serviceId,
                    DateConnection = DateTime.Today
                    // Дополнительные поля, если необходимо
                };

              

                var client = _crud.GetClient(contract.ClientId_FK);
                if (client == null)
                {
                    // Если клиент не найден, возвращаем ошибку 404 Not Found
                    return NotFound();
                }
                // Проверяем, что у клиента достаточно средств на балансе
                if (client.Balance < service.Cost)
                {
                    // Пишем "Абонент заблокирован!", так как баланс клиента меньше или равен нулю
                    return BadRequest("Баланс клиента не позволяет совершить данную операцию!");
                }
                client.Balance = -service.Cost;
                _crud.Save();
                // Парсим число из названия сервиса
                var numberInName = int.Parse(Regex.Match(service.Name, @"\d+").Value);

                // Обновляем соответствующее значение в контракте
                if (service.Name.Contains("SMS"))
                {
                    contract.SMSRemaining = contract.SMSRemaining + numberInName;
                    _crud.Save();

                }
                else if (service.Name.Contains("MIN"))
                {
                    contract.MinutesRemaining = contract.MinutesRemaining + numberInName;
                    _crud.Save();

                }
                else if (service.Name.Contains("GB"))
                {
                    contract.GBRemaining = contract.GBRemaining + numberInName;
                    _crud.Save();

                }
                _crud.Save();
                var newWriteOff = new WriteOffs
                {
                    ContractId_FK = contractId,
                    DateWriteOff = DateTime.Today,
                    Sum = service.Cost
                };

                // Добавляем новую запись в контекст базы данных
                _crud.CreateWriteOffs(newWriteOff);
                _crud.Save();

                var events = new EventsContract
                {
                    ContractId_FK = contract.ContractId,
                    Date = DateTime.Today,
                    CategoryEventId_FK = 3
                };

                // Добавляем новую запись в контекст базы данных
                _crud.CreateEvents(events);
                // Добавляем запись в контекст базы данных
                _crud.CreateServiceConnected(connectedService);
                _crud.Save();

                // Возвращаем успешный результат
                return Ok($"Услуга {service.Name} успешно подключена к контракту под номером {contractId}");
            }
            catch (Exception ex)
            {
                // Если произошла ошибка, логируем ее и возвращаем ошибку 500 Internal Server Error
                _logger.LogError(ex, $"An error occurred while connecting service {serviceId} to contract {contractId}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while connecting service to contract");
            }
        }

        // Метод для отключения услуги от контракта
        [HttpPost("{contractId}/disconnect-service/{serviceId}")]
        public async Task<IActionResult> DisconnectServiceFromContract(int contractId, int serviceId)
        {
            try
            {
                // Получаем список подключенных услуг для контракта
                var connectedServices = await GetConnectedServices(contractId);
                // Получаем контракт
                var contract = _crud.GetContract(contractId);

                // Проверяем, что контракт существует
                if (contract == null)
                {
                    return NotFound($"Контракт с номером {contractId} не найден");
                }

                // Проверяем, что статус контракта "Активный"
                if (contract.Status != "Активный")
                {
                    return BadRequest($"Невозможно отключить услугу. Статус контракта должен быть 'Активный'");
                }

                // Проверяем, есть ли запись о подключенной услуге для данного контракта
                var connectedService = connectedServices.FirstOrDefault(cs => cs.ServiceId == serviceId);
                if (connectedService == null)
                {
                    // Если такая услуга не подключена к контракту, возвращаем сообщение
                    return BadRequest($"Услуга с номером {serviceId} не подключена к контракту под номером {contractId}");
                }

              
                // Удаляем запись о подключении услуги от контракта
                _crud.DeleteServicesConnected(contractId, connectedService.ServiceId);
                _crud.Save();

                var events = new EventsContract
                {
                    ContractId_FK = contract.ContractId,
                    Date = DateTime.Today,
                    CategoryEventId_FK = 4
                };

                // Добавляем новую запись в контекст базы данных
                _crud.CreateEvents(events);
                // Возвращаем успешный результат
                return Ok($"Услуга {connectedService.ServiceId} успешно отключена от контракта под номером {contractId}");
            }
            catch (Exception ex)
            {
                // Если произошла ошибка, логируем ее и возвращаем ошибку 500 Internal Server Error
                _logger.LogError(ex, $"An error occurred while disconnecting service {serviceId} from contract {contractId}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while disconnecting service from contract");
            }
        }


        // Метод для получения всех записей о списаниях для контракта с определенным id
        [HttpGet("{id}/write-offs")]
        public IActionResult GetWriteOffsForContract(int id)
        {
            try
            {
                // Получаем все записи о списаниях для указанного контракта
                var writeOffs = _crud.GetWriteOffsForContract(id);

                if (writeOffs == null || !writeOffs.Any())
                {
                    // Если записи о списаниях не найдены, возвращаем пустой результат
                    return NoContent();
                }

                // Если записи о списаниях найдены, возвращаем их
                return Ok(writeOffs);
            }
            catch (Exception ex)
            {
                // Если произошла ошибка, логируем ее и возвращаем ошибку 500 Internal Server Error
                _logger.LogError(ex, $"An error occurred while getting write-offs for Contract with id {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while getting write-offs for Contract with id {id}");
            }
        }

        public class Events
        {
            public DateTime Date { get; set; }

            public string Name { get; set; }
         
        }



        // Метод для получения всех записей о списаниях для контракта с определенным id
        [HttpGet("{contractId}/events")]
        public IActionResult GetEventsForContract(int contractId)
        {
            try
            {
                
                

                //// Now, filter the write-offs based on the contract ID
                //var events = _crud.GetAllEvents()
                //    .Where(events => events.ContractId_FK == contractId)
                //    .ToList(); // 


                var events = from sc in _crud.GetAllEvents()
                                        where sc.ContractId_FK == contractId
                             join s in _crud.GetAllCategoryEvents() on sc.CategoryEventId_FK equals s.CategoryEventId
                                        select new Events
                                        {
                                            
                                            Name = s.Name,
                                          Date = sc.Date
                                        };


                // Если записи о списаниях найдены, возвращаем их
                return Ok(events);
            }
            catch (Exception ex)
            {
                // Если произошла ошибка, логируем ее и возвращаем ошибку 500 Internal Server Error
                _logger.LogError(ex, $"An error occurred while getting write-offs for Contract with id {contractId}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while getting write-offs for Contract with id {contractId}");
            }
        }











        // Метод для получения всех записей о зачислениях для контракта с определенным id
        [HttpGet("{id}/depositings")]
        public IActionResult GetDepositingsForContract(int id)
        {
            try
            {
                // Получаем все записи о списаниях для указанного контракта
                var depositings = _crud.GetDepositingsForContract(id);

                if (depositings == null || !depositings.Any())
                {
                    // Если записи о списаниях не найдены, возвращаем пустой результат
                    return NoContent();
                }

                // Если записи о списаниях найдены, возвращаем их
                return Ok(depositings);
            }
            catch (Exception ex)
            {
                // Если произошла ошибка, логируем ее и возвращаем ошибку 500 Internal Server Error
                _logger.LogError(ex, $"An error occurred while getting depositings for Contract with id {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while getting depositings for Contract with id {id}");
            }
        }













        [HttpGet("{id}/Was-monthly-payment")]
        public async Task<IActionResult> WasMonthlyPayment(int id)
        {
            try
            {
                var contract = _crud.GetContract(id);

                // Проверяем, существуют ли записи списаний для данного контракта за текущий месяц
                var writeOffsForCurrentMonth = _crud.GetWriteOffsForContract(id)
                    .Where(w => w.DateWriteOff.Month == DateTime.Today.Month && w.DateWriteOff.Year == DateTime.Today.Year && w.DateWriteOff.Day == contract.DateConnectionTariff.Day)
                    .ToList();

                // Если записи списаний за текущий месяц отсутствуют
                if (!writeOffsForCurrentMonth.Any())
                {
                    var wasMonthlyPayment = new DateTime(DateTime.Today.Year, DateTime.Today.Month, contract.DateConnectionTariff.Day);
                    return Ok(new { wasMonthlyPayment });
                }
                else
                {
                    var wasMonthlyPayment = new DateTime(DateTime.Today.Year, DateTime.Today.Month + 1, contract.DateConnectionTariff.Day);
                    return Ok(new { wasMonthlyPayment });
                }
            }
            catch (Exception ex)
            {
                // Если произошла ошибка, логируем ее и возвращаем ошибку 500 Internal Server Error
                _logger.LogError(ex, $"An error occurred while getting as-monthly-payment for Contract with id {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while getting as-monthly-payment for Contract with id {id}");
            }
        }













        // Метод для получения общей ежемесячной выплаты для Contractа с заданным id
        [HttpGet("{id}/total-payment")]
        public async Task<IActionResult> GetTotalPayment(int id)
        {
            try
            {

                // Получаем Contract с заданным id из Unit of Work
                var contract = _crud.GetContract(id);
                if (contract.Status == "Активный")
                {
                    if (contract == null)
                    {
                        // Если Contract не найден, возвращаем ошибку 404 Not Found
                        return NotFound();
                    }

                    var tariff = _crud.GetTariff(contract.TariffId);
                    // Получаем абонентскую плату для тарифа контракта
                    var tariffCost = tariff.Cost;

                    // Получаем список подключенных услуг для контракта асинхронно
                    var connectedServices = await GetConnectedServices(id);

                    // Вычисляем общую стоимость всех подключенных услуг
                    var totalServicesCost = connectedServices.Sum(service => service.Cost);

                    // Суммируем абонентскую плату тарифа и стоимость всех подключенных услуг
                    var totalPayment = tariffCost + totalServicesCost;

                    // Возвращаем общую ежемесячную выплату
                    return Ok(new { TotalPayment = totalPayment });
                }
                else
                {
                    var totalPayment = 0;
                    // Возвращаем общую ежемесячную выплату
                    return Ok(new { TotalPayment = totalPayment });
                }
            }
            catch (Exception ex)
            {
                // Если произошла ошибка, логируем ее и возвращаем ошибку 500 Internal Server Error
                _logger.LogError(ex, "Ошибка при получении общей ежемесячной выплаты для Contractа");
                return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка при получении общей ежемесячной выплаты для Contractа");
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



        [HttpGet("contracts/{contractId}/available-tariffs")]
        public async Task<ActionResult<IEnumerable<Tariff>>> GetAvailableTariffs(int contractId)
        {
            try
            {
                // Получаем контракт по его идентификатору
                var contract = _crud.GetContract(contractId);

                // Проверяем, что контракт существует
                if (contract == null)
                {
                    return NotFound("Contract not found");
                }

                // Получаем текущий тариф по контракту
                var currentTariff = _crud.GetTariff(contract.TariffId);

                // Проверяем, что текущий тариф существует
                if (currentTariff == null)
                {
                    return NotFound("Current tariff not found");
                }

                // Получаем баланс клиента
                var clientBalance = _crud.GetClient(contract.ClientId_FK).Balance;

                // Получаем все тарифы, кроме текущего и тех, чья стоимость превышает баланс клиента
                var availableTariffs = _crud.GetAllTariff().Where(t => t.TariffId != currentTariff.TariffId && t.Cost <= clientBalance);

                return Ok(availableTariffs);
            }
            catch (Exception ex)
            {
                // Логируем ошибку и возвращаем ошибку сервера
                _logger.LogError(ex, $"An error occurred while getting available tariffs for ContractId {contractId}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while getting available tariffs for ContractId {contractId}");
            }
        }



        [HttpGet("contracts/{contractId}/related-tariff")]
        public async Task<ActionResult<Tariff>> GetRelatedTariff(int contractId)
        {
            try
            {
                // Получаем контракт по его идентификатору
                var contract = _crud.GetContract(contractId);

                // Проверяем, что контракт существует
                if (contract == null)
                {
                    return NotFound("Contract not found");
                }

                // Получаем информацию о тарифе, связанном с контрактом
                var tariff = _crud.GetTariff(contract.TariffId);

                // Проверяем, что тариф существует
                if (tariff == null)
                {
                    return NotFound("Tariff not found");
                }

                return tariff;
            }
            catch (Exception ex)
            {
                // Логируем ошибку и возвращаем ошибку сервера
                _logger.LogError(ex, $"An error occurred while getting related tariff for ContractId {contractId}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while getting related tariff for ContractId {contractId}");
            }
        }








        // Контроллер для обновления Contractа с определенным id
        [HttpPut("updateStatus{id}")]

        public async Task<IActionResult> UpdateContractStatus(int id, OperatorMO_ASPNET.DAL.Models.Contract Contract)
        {
            try
            {

                // Проверяем, что id Contractа совпадает с переданным в параметрах
                if (id != Contract.ContractId)
                {
                    return BadRequest();
                }
                _crud.UpdateContract(Contract);

         
           
                // Логируем информацию об изменении Contractа
                _logger.LogInformation("Изменен Contract с id " + Contract.ContractId);
                try
                {

                    var events = new EventsContract
                    {
                        ContractId_FK = Contract.ContractId,
                        Date = DateTime.Today,
                        CategoryEventId_FK = 5
                    };

                    // Добавляем новую запись в контекст базы данных
                    _crud.CreateEvents(events);
                    // Сохраняем изменения в базе данных
                    _crud.Save();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Если Contract уже был изменен другим пользователем, возвращаем ошибку
                    if (!ContractExists(id))
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
                _logger.LogError(ex, $"An error occurred while updating Contract with id {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating Contract with id {id}");
            }
        }



        [HttpPut("{id}/checkAndClose")]
        public async Task<IActionResult> CheckAndCloseContract(int id)
        {
            try
            {
                var contract = _crud.GetContract(id);

                if (contract == null)
                {
                    return NotFound("Contract not found");
                }

                // Получаем последние записи из depositings и writeOffs для контракта
                var lastDepositing = _crud.GetDepositingsForContract(id).OrderByDescending(d => d.Date).FirstOrDefault();
                var lastWriteOff = _crud.GetWriteOffsForContract(id).OrderByDescending(w => w.DateWriteOff).FirstOrDefault();

                DateTime? lastActivityDate = null;

                // Определяем последнюю активность
                if (lastDepositing != null && lastWriteOff != null)
                {
                    lastActivityDate = lastDepositing.Date > lastWriteOff.DateWriteOff ? lastDepositing.Date : lastWriteOff.DateWriteOff;
                }
                else if (lastDepositing != null)
                {
                    lastActivityDate = lastDepositing.Date;
                }
                else if (lastWriteOff != null)
                {
                    lastActivityDate = lastWriteOff.DateWriteOff;
                }

                // Проверяем, если статус контракта "Активный" и прошло более полугода с последней активности
                if (contract.Status == "Активный" && lastActivityDate.HasValue && (DateTime.Now - lastActivityDate.Value).TotalDays > 180)
                {
                    // Обновляем статус контракта на "Закрытый"
                    contract.Status = "Закрытый";

                    var events = new EventsContract
                    {
                        ContractId_FK = contract.ContractId,
                        Date = DateTime.Today,
                        CategoryEventId_FK = 5
                    };

                    // Добавляем новую запись в контекст базы данных
                    _crud.CreateEvents(events);
                    _crud.UpdateContract(contract);
                    _crud.Save();

                    _logger.LogInformation($"Contract with id {id} has been closed due to inactivity for more than 6 months.");

                    return Ok(new { message = $"Contract with id {id} has been closed due to inactivity for more than 6 months." });

                }

                else if (contract.Status == "Закрытый")
                {
                    // Проверяем, если статус контракта "Закрытый" и если depositing и writeOffs их даты меньше чем полгода,
                    // то меняем статус на "Активный"
                    if ((lastDepositing != null && (DateTime.Now - lastDepositing.Date).TotalDays <= 180) ||
                        (lastWriteOff != null && (DateTime.Now - lastWriteOff.DateWriteOff).TotalDays <= 180))
                    {
                        contract.Status = "Активный";

                        var events = new EventsContract
                        {
                            ContractId_FK = contract.ContractId,
                            Date = DateTime.Today,
                            CategoryEventId_FK = 6
                        };

                        // Добавляем новую запись в контекст базы данных
                        _crud.CreateEvents(events);
                        _crud.UpdateContract(contract);
                        _crud.Save();

                        _logger.LogInformation($"Contract with id {id} status changed back to 'Активный' as activity detected within 6 months.");

                        return Ok(new { message = $"Contract with id {id} status changed back to 'Активный' as activity detected within 6 months." });
                    }
                }

                return Ok(new { message = "Contract is still active." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while checking and closing Contract with id {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while checking and closing Contract with id {id}");
            }
        }




    }
}