using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using AppShowDoMilhao.Data;
using AppShowDoMilhao.Models.LoginModel;
using AppShowDoMilhao.Services;
using System;


namespace AppShowDoMilhao.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Verificar se o usuário existe com base no email
            var usuario = await _context.Usuarios
                                        .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (usuario == null)
            {
                return Unauthorized(new { Success = false, Message = "Email ou senha incorretos" });
            }

            // Gerar o hash da senha inserida utilizando o salt armazenado
            var hashedPassword = PasswordService.HashPassword(request.Senha, usuario.PasswordSalt);

            // Comparar o hash da senha inserida com o hash armazenado
            if (!hashedPassword.SequenceEqual(usuario.PasswordHash))
            {
                return Unauthorized(new { Success = false, Message = "Email ou senha incorretos" });
            }

            // Se a autenticação for bem-sucedida, armazene o estado de login na sessão
            HttpContext.Session.SetInt32("UsuarioId", usuario.UsuarioId);

            return Ok(new { Success = true, Message = "Login bem-sucedido", Id = usuario.UsuarioId });
        }


        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UsuarioId");

            return Ok(new { Success = true, Message = "Logout realizado com sucesso" });
        }

        [HttpGet("check-session")]
        public IActionResult CheckSession()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                return Unauthorized(new { Success = false, Message = "Usuário não está logado" });
            }

            return Ok(new { Success = true, UsuarioId = usuarioId });
        }

    }
}
