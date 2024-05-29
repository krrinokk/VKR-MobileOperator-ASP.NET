using Microsoft.AspNetCore.Mvc;
using OperatorMO_ASPNET.DAL.Repository;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OperatorMO_ASPNET.DAL.Models;
using BLL.Interfaces;
using System.Globalization;

namespace OperatorMO_ASPNET.Controllers
{
    // Указываем маршрут для данного контроллера
    [Route("api/[controller]")]
    // Указываем, что данный класс является контроллером API
    [ApiController]
    public class ChatController : ControllerBase
    {
        // Интерфейс для работы с базой данных
        private readonly IDbCrud _crud;
        // Логгер для записи информации о работе приложения
        private readonly ILogger _logger;

        // Конструктор с параметрами
        public ChatController(IDbCrud newIDbCrud, ILogger<ChatController> logger)
        {
            _crud = newIDbCrud;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChatDto>>> GetAllMessages()
        {
            _logger.LogInformation("You have moved to ChatController, to the GetAllChat() method");
            var chats = _crud.GetAllMessages(); // предполагается, что этот метод возвращает список объектов Chat
            var chatDtos = new List<ChatDto>();

            foreach (var chat in chats)
            {
                var user = _crud.GetUser(chat.UserId_FK); // метод для получения информации о пользователе по его идентификатору
                var role = user.RoleId_FK == 1 ? "Оператор" : "Администратор"; // проверяем роль пользователя

                chatDtos.Add(new ChatDto
                {
                    Message = chat.Message,
                    Date = chat.Date,
                    UserName = user.Name,
                    UserId = user.UserId,
                    MessageId=chat.MessageId,
                    Role = role
                });
            }

            return Ok(chatDtos);
        }
    

    public class ChatDto
    {
            public int MessageId { get; set; }
            public string Message { get; set; }
        public DateTime Date { get; set; }
        public string UserName { get; set; }

            public int UserId { get; set; }
            public string Role { get; set; }
    }

    // Метод для получения Chatа по идентификатору
    [HttpGet("{id}")]
        public async Task<ActionResult<OperatorMO_ASPNET.DAL.Models.Chat>> GetMessage(int id)
        {
            try
            {
                // Получаем Chat из базы данных по идентификатору
                var Chat = _crud.GetMessage(id);
                // Если Chat не найден, возвращаем ошибку
                if (Chat == null)
                {
                    _logger.LogInformation($"Message with id {id} not found");
                    return NotFound();
                }
                // Если Chat найден, возвращаем его
                _logger.LogInformation($"Message with id {id} found: {Chat}");
                return Chat;
            }
            catch (Exception ex)
            {
                // Записываем информацию об ошибке в лог
                _logger.LogError(ex, $"An error occurred while getting message with id {id}");
                // Возвращаем ошибку сервера
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while getting Chat with id {id}");
            }
        }


        // Этот метод удаляет Client с заданным id
        [HttpDelete("{messageId}")]

        public async Task<IActionResult> DeleteMessage(int messageId)
        {
            try
            {
                // Получаем Client с заданным id из Unit of Work
                var message = _crud.GetMessage(messageId);
                if (message == null)
                {
                    // Если Client не найден, возвращаем ошибку 404 Not Found
                    return NotFound();
                }
                // Удаляем Client из Unit of Work
                _crud.DeleteMessage(messageId);
                _crud.Save();
                _logger.LogInformation("Удален message с messageId " + message.MessageId);
                return NoContent();
            }
            catch (Exception ex)
            {
                // Если произошла ошибка, логируем ее и возвращаем ошибку 500 Internal Server Error
                _logger.LogError(ex, "Ошибка при удалении message");
                return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка при удалении message");
            }
        }


        // Контроллер для обновления Clientа с определенным id
        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateMessage(int id, OperatorMO_ASPNET.DAL.Models.Chat message)
        {
            try
            {
                // Проверяем, что id Clientа совпадает с переданным в параметрах
                if (id != message.MessageId)
                {
                    return BadRequest();
                }
                // Вызываем метод для обновления Clientа в базе данных
                _crud.UpdateMessage(message);
                // Логируем информацию об изменении Clientа
                _logger.LogInformation("Изменен message с id " + message.MessageId);
                try
                {
                    // Сохраняем изменения в базе данных
                    _crud.Save();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Если Client уже был изменен другим пользователем, возвращаем ошибку
                    if (!ChatExists(id))
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
                _logger.LogError(ex, $"An error occurred while updating message with id {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating message with id {id}");
            }
        }

        // Контроллер для создания нового Message
        [HttpPost]

        public IActionResult CreateMessage(OperatorMO_ASPNET.DAL.Models.Chat message)
        {
            try
            {
                // Проверяем валидность модели
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }




                message.Date = DateTime.Now;



                // Создаем новый Client в базе данных
                _crud.CreateMessage(message);
                // Сохраняем изменения в базе данных
                _crud.Save();

                // Возвращаем успешный результат с информацией о созданном Clientе
                return CreatedAtAction("GetMessage", new { id = message.MessageId }, message);
            }
            catch (Exception ex)
            {
                // Логируем ошибку и возвращаем ошибку сервера
                _logger.LogError(ex, "An error occurred while creating a new message");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating a new message");
            }
        }

        // Этот метод проверяет, существует ли Chat с заданным id
        private bool ChatExists(int id)
        {
            //return _context.CategoryTables.Any(e => e.CategoryId == id);
            return _crud.GetMessage(id) != null;
        }

     
    }
}