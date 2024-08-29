using AppShowDoMilhao.Models;
using Microsoft.EntityFrameworkCore;
using AppShowDoMilhao.Models.UsuarioModel;
using Microsoft.AspNetCore.Identity; // Adicionado para PasswordHasher
using Microsoft.AspNetCore.Mvc;
using AppShowDoMilhao.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AppShowDoMilhao.Services;
using Microsoft.AspNetCore.Authorization;


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

        [HttpGet("get-user")]
        public async Task<IActionResult> GetUserById([FromBody] GetUserByIdRequest request)
        {
            try
            {
                // Validação do login
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null)
                {
                    return Unauthorized(new { Success = false, Message = "Usuário não está logado!" });
                }

                // Busca o usuário pelo ID fornecido
                var usuario = await _context.Usuarios
                    .Where(u => u.UsuarioId == request.UsuarioId && u.DataDelecao == null) // Verifica se o usuário existe e não foi deletado
                    .Select(u => new UserResponse
                    {
                        UsuarioId = u.UsuarioId,
                        Nome = u.Nome,
                        Nickname = u.Nickname,
                        Email = u.Email,
                        Avatar = u.Avatar,
                        NumeroPartidasJogadas = u.NumeroPartidasJogadas,
                        NumeroTotalPerguntas = u.NumeroTotalPerguntas,
                        PremiacaoTotal = u.PremiacaoTotal,
                        QuantidadeUtilizacaoAjuda = u.QuantidadeUtilizacaoAjuda,
                        NumeroDerrotasErro = u.NumeroDerrotasErro,
                        NumeroDerrotasParada = u.NumeroDerrotasParada,
                        DataCriacao = u.DataCriacao
                    })
                    .FirstOrDefaultAsync();

                if (usuario == null)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Usuário não encontrado ou foi deletado!"
                    });
                }

                // Retorna os dados do usuário
                return Ok(new
                {
                    Success = true,
                    Data = usuario
                });
            }
            catch (Exception ex)
            {
                // Tratamento de erros genéricos
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Ocorreu um erro ao processar sua solicitação.",
                    Details = ex.Message // Em produção, você pode querer remover ou logar os detalhes do erro ao invés de retorná-los.
                });
            }
        }


        [HttpGet("get-users")]
        public async Task<IActionResult> GetUsers([FromBody] GetUsersRequest request)
        {
            try
            {
                // Validação do login
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null)
                {
                    return Unauthorized(new { Success = false, Message = "Usuário não está logado!" });
                }

                // Consulta base de usuários
                var query = _context.Usuarios.Where(u => u.DataDelecao == null).AsQueryable();

                // Aplicação dos filtros, se fornecidos
                if (!string.IsNullOrEmpty(request.Nome))
                {
                    query = query.Where(u => u.Nome.Contains(request.Nome));
                }

                if (!string.IsNullOrEmpty(request.Nickname))
                {
                    query = query.Where(u => u.Nickname.Contains(request.Nickname));
                }

                if (!string.IsNullOrEmpty(request.Email))
                {
                    query = query.Where(u => u.Email.Contains(request.Email));
                }

                // Execução da consulta e mapeamento para UserResponse
                var usuarios = await query.Select(u => new UserResponse
                {
                    UsuarioId = u.UsuarioId,
                    Nome = u.Nome,
                    Nickname = u.Nickname,
                    Email = u.Email,
                    Avatar = u.Avatar,
                    NumeroPartidasJogadas = u.NumeroPartidasJogadas,
                    NumeroTotalPerguntas = u.NumeroTotalPerguntas,
                    PremiacaoTotal = u.PremiacaoTotal,
                    QuantidadeUtilizacaoAjuda = u.QuantidadeUtilizacaoAjuda,
                    NumeroDerrotasErro = u.NumeroDerrotasErro,
                    NumeroDerrotasParada = u.NumeroDerrotasParada,
                    DataCriacao = u.DataCriacao
                }).ToListAsync();

                // Verifica se a lista de usuários está vazia
                if (!usuarios.Any())
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Nenhum usuário encontrado!"
                    });
                }

                // Retorna a lista de usuários
                return Ok(new
                {
                    Success = true,
                    Data = usuarios
                });
            }
            catch (Exception ex)
            {
                // Tratamento de erros genéricos
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Ocorreu um erro ao processar sua solicitação.",
                    Details = ex.Message // Em produção, você pode querer remover ou logar os detalhes do erro ao invés de retorná-los.
                });
            }
        }

        // Permite que este método seja acessado sem autenticação
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verifica se o email já está em uso
            if (await _context.Usuarios.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Email já está em uso!"
                });
            }

            // Gerar salt e hash da senha
            var salt = PasswordService.GenerateSalt();
            var hashedPassword = PasswordService.HashPassword(request.Senha, salt);

            // Criar o novo usuário
            var newUser = new UsuarioModel
            {
                Nome = request.Nome,
                Nickname = request.Nickname,
                Email = request.Email,
                Avatar = request.Avatar,
                PasswordSalt = salt,
                PasswordHash = hashedPassword,
                DataCriacao = DateTime.UtcNow // Definindo a data de criação
                                              // DataDelecao é deixado como null inicialmente
            };

            // Adicionar o usuário ao banco de dados
            _context.Usuarios.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Success = true,
                UsuarioId = newUser.UsuarioId,
                Message = "Usuário registrado com sucesso!"
            });
        }

        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUserById([FromBody] UpdateUserByIdRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Success = false, Message = "Dados inválidos." });
            }

            // Verifica se o usuário está logado
            var loggedInUserId = HttpContext.Session.GetInt32("UsuarioId");
            if (loggedInUserId == null)
            {
                return Unauthorized(new { Success = false, Message = "Usuário não está logado." });
            }

            // Verifica se o usuário está tentando atualizar seu próprio perfil
            if (loggedInUserId.Value != request.UsuarioId)
            {
                return Forbid(); // Não é necessário fornecer uma mensagem aqui. O `Forbid` é um status 403.
            }

            try
            {
                var usuario = await _context.Usuarios.FindAsync(request.UsuarioId);
                if (usuario == null)
                {
                    return NotFound(new { Success = false, Message = "Usuário não encontrado." });
                }

                // Atualiza os campos fornecidos no request
                if (!string.IsNullOrEmpty(request.Nome))
                {
                    usuario.Nome = request.Nome;
                }

                if (!string.IsNullOrEmpty(request.Nickname))
                {
                    usuario.Nickname = request.Nickname;
                }

                if (!string.IsNullOrEmpty(request.Email))
                {
                    usuario.Email = request.Email;
                }

                if (!string.IsNullOrEmpty(request.Avatar))
                {
                    usuario.Avatar = request.Avatar;
                }

                // Atualiza a senha, se fornecida
                if (!string.IsNullOrEmpty(request.Senha))
                {
                    var salt = PasswordService.GenerateSalt();
                    var hashedPassword = PasswordService.HashPassword(request.Senha, salt);
                    usuario.PasswordHash = hashedPassword;
                    usuario.PasswordSalt = salt;
                }

                // Atualiza os outros campos, se fornecidos
                if (request.NumeroPartidasJogadas.HasValue)
                {
                    usuario.NumeroPartidasJogadas = request.NumeroPartidasJogadas.Value;
                }

                if (request.NumeroTotalPerguntas.HasValue)
                {
                    usuario.NumeroTotalPerguntas = request.NumeroTotalPerguntas.Value;
                }

                if (request.PremiacaoTotal.HasValue)
                {
                    usuario.PremiacaoTotal = request.PremiacaoTotal.Value;
                }

                if (request.QuantidadeUtilizacaoAjuda.HasValue)
                {
                    usuario.QuantidadeUtilizacaoAjuda = request.QuantidadeUtilizacaoAjuda.Value;
                }

                if (request.NumeroDerrotasErro.HasValue)
                {
                    usuario.NumeroDerrotasErro = request.NumeroDerrotasErro.Value;
                }

                if (request.NumeroDerrotasParada.HasValue)
                {
                    usuario.NumeroDerrotasParada = request.NumeroDerrotasParada.Value;
                }

                // Salva as mudanças no banco de dados
                _context.Usuarios.Update(usuario);
                await _context.SaveChangesAsync();

                return Ok(new { Success = true, Message = "Usuário atualizado com sucesso." });
            }
            catch (Exception ex)
            {
                // Tratamento de erros genéricos
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Ocorreu um erro ao processar sua solicitação.",
                    Details = ex.Message // Em produção, considere remover ou logar os detalhes do erro ao invés de retorná-los.
                });
            }
        }



        [HttpDelete("delete-user")]
        public async Task<IActionResult> DeleteUserById([FromBody] DeleteUserByIdRequest request)
        {
            try
            {
                // Validação do login
                var loggedInUserId = HttpContext.Session.GetInt32("UsuarioId");
                if (loggedInUserId == null)
                {
                    return Unauthorized(new { Success = false, Message = "Usuário não está logado!" });
                }

                // Verifica se o usuário a ser excluído existe
                var usuario = await _context.Usuarios.FindAsync(request.UsuarioId);
                if (usuario == null)
                {
                    return NotFound(new { Success = false, Message = "Usuário não encontrado!" });
                }

                // Verifica se o usuário logado tem permissão para excluir este usuário
                if (loggedInUserId.Value != request.UsuarioId)
                {
                    return Forbid();
                }

                // Marca o usuário como excluído
                usuario.DataDelecao = DateTime.UtcNow; // Atualiza a data de exclusão
                _context.Usuarios.Update(usuario);
                await _context.SaveChangesAsync();

                return Ok(new { Success = true, Message = "Usuário marcado como excluído com sucesso!" });
            }
            catch (Exception ex)
            {
                // Tratamento de erros genéricos
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Ocorreu um erro ao processar sua solicitação.",
                    Details = ex.Message // Em produção, você pode querer remover ou logar os detalhes do erro ao invés de retorná-los.
                });
            }
        }


    }
}

