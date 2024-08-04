using AppShowDoMilhao.Models;
using Microsoft.EntityFrameworkCore;
using AppShowDoMilhao.Models.UsuarioModel;
using Microsoft.AspNetCore.Identity; // Adicionado para PasswordHasher
using Microsoft.AspNetCore.Mvc;
using AppShowDoMilhao.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;


namespace AppShowDoMilhao.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<UsuarioModel> _passwordHasher; // Adicionado para PasswordHasher

        public UsuarioController(ApplicationDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<UsuarioModel>(); // Inicializa PasswordHasher
        }

        [HttpPost("get-user")]
        public async Task<IActionResult> GetUserById([FromBody] GetUserByIdRequest request)
        {
            if (request.Method != "GetUserById")
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Método inválido, revise sua nomenclatura!"
                });
            }

            var usuario = await _context.Usuarios.FindAsync(request.UsuarioId);
            if (usuario == null)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = "Usuário não encontrado pelo Id fornecido!"
                });
            }

            var usuarioResponse = new UsuarioResponse
            {
                UsuarioId = usuario.UsuarioId,
                NomeCompleto = usuario.NomeCompleto,
                Email = usuario.Email,
                DataCriacao = usuario.DataCriacao
            };

            return Ok(new
            {
                Success = true,
                Usuario = usuarioResponse
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {

            if (ModelState.IsValid)
            {
                var usuario = new UsuarioModel
                {
                    NomeCompleto = request.NomeCompleto,
                    Email = request.Email,
                    Avatar = request.Avatar,
                    Senha = _passwordHasher.HashPassword(new UsuarioModel(), request.Senha),
                    DataCriacao = DateTime.UtcNow // Adiciona a data de criação atual
                };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Success = true,
                    UsuarioId = usuario.UsuarioId,
                    Message = "Usuário registrado com sucesso!"
                });
            }
            return BadRequest(ModelState);
        }

        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUserById([FromBody] UpdateUserByIdRequest request)
        {
            var usuario = await _context.Usuarios.FindAsync(request.UsuarioId);
            if (usuario == null)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = "Usuário não encontrado pelo Id informado!"
                });
            }

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(request.NomeCompleto))
                {
                    usuario.NomeCompleto = request.NomeCompleto;
                }
                if (!string.IsNullOrEmpty(request.Email))
                {
                    usuario.Email = request.Email;
                }
                if (!string.IsNullOrEmpty(request.Avatar))
                {
                    usuario.Avatar = request.Avatar;
                }

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Usuarios.Any(e => e.UsuarioId == request.UsuarioId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Ok(new
                {
                    Success = true,
                    UsuarioId = usuario.UsuarioId,
                    Message = "Usuário atualizado com sucesso!"
                });
            }
            return BadRequest(ModelState);
        }


        [HttpDelete("delete-user")]
        public async Task<IActionResult> DeleteUserById([FromBody] DeleteUserByIdRequest request)
        {
            if (request.Method != "DeleteUserById")
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Método inválido, revise sua nomenclatura!"
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuario = await _context.Usuarios.FindAsync(request.UsuarioId);
            if (usuario == null)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = "Usuário não encontrado pelo Id informado!"
                });
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                Success = true,
                UsuarioId = usuario.UsuarioId,
                Message = "Usuário deletado com sucesso!"
            });
        }

    }
}
