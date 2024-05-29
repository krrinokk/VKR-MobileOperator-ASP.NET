using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using OperatorMO_ASPNET.DAL;
using OperatorMO_ASPNET.DAL.Models;
using OperatorMO_ASPNET.ViewModels;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using System.Text;
using System.Security.Cryptography;

namespace OperatorMO_ASPNET.Controllers
{
    [Route("api/[controller]")]
    
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly OperatorContext _context;

        public AccountController(OperatorContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = _context.User.FirstOrDefault(u => u.Login == model.Login);

            if (existingUser == null)
            {
                return NotFound(new { message = "Пользователь с таким логином не найден" });
            }

            if (!VerifyPassword(existingUser.Password, model.Password))
            {
                return Unauthorized(new { message = "Неверный пароль" });
            }

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, existingUser.UserId.ToString()),
        new Claim(ClaimTypes.Name, existingUser.Login)
    };

            // Добавление информации о роли пользователя
            string role = existingUser.RoleId_FK == 1 ? "Оператор" : "Администратор";
            claims.Add(new Claim(ClaimTypes.Role, role));

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties();

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return Ok(new { message = "Аутентификация успешна", userId = existingUser.UserId, userName = existingUser.Name, role });
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { message = "Выход выполнен успешно" });
        }
        [HttpGet("check-auth")]
        public IActionResult CheckAuthentication()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var existingUser = _context.User.FirstOrDefault(u => u.UserId == int.Parse(userId));
                if (existingUser != null)
                {
                    var userName = existingUser.Name;
                    return Ok(new { message = "Пользователь аутентифицирован", userName });
                }
                else
                {
                    return NotFound(new { message = "Пользователь не найден" });
                }
            }
            else
            {
                // Возвращаем подробности об ошибке, указывая, что пользователь не аутентифицирован
                return Unauthorized(new { error = "Пользователь не аутентифицирован", details = "Доступ к защищенным ресурсам требует аутентификации" });
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

        private bool VerifyPassword(string hashedPassword, string password)
        {
            string hashedInputPassword = HashPassword(password);

            // Сравниваем хэш-значения
            return hashedPassword == hashedInputPassword;

        }


    }
}