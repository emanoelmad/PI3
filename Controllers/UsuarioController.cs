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
using System.Security.Claims;


namespace AppShowDoMilhao.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<Usuario> _passwordHasher; // Adicionado para PasswordHasher

        public UsuarioController(ApplicationDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<Usuario>(); // Inicializa PasswordHasher
        }

        [HttpGet("get-user/{usuarioId}")]
        public async Task<IActionResult> GetUserById(int usuarioId)
        {
            try
            {
                // Valida��o do login
                var sessionUsuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (sessionUsuarioId == null)
                {
                    return Unauthorized(new { Success = false, Message = "Usu�rio n�o est� logado!" });
                }

                // Busca o usu�rio pelo ID fornecido
                var usuario = await _context.Usuarios
                    .Where(u => u.UsuarioId == usuarioId && u.DataDelecao == null) // Verifica se o usu�rio existe e n�o foi deletado
                    .Select(u => new UserResponse
                    {
                        UsuarioId = u.UsuarioId,
                        Nome = u.Nome,
                        Nickname = u.Nickname,
                        Email = u.Email,
                        Avatar = u.Avatar,
                        NumeroPartidasJogadas = u.NumeroPartidasJogadas,
                        NumTotalPerguntasRespondidas = u.NumTotalPerguntasRespondidas,
                        NumTotalPerguntasRejeitadas = u.NumTotalPerguntasRejeitadas,
                        NumTotalPerguntasAceita = u.NumTotalPerguntasAceita,
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
                        Message = "Usu�rio n�o encontrado ou foi deletado!"
                    });
                }

                // Retorna os dados do usu�rio
                return Ok(new
                {
                    Success = true,
                    Data = usuario
                });
            }
            catch (Exception ex)
            {
                // Tratamento de erros gen�ricos
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Ocorreu um erro ao processar sua solicita��o.",
                    Details = ex.Message // Em produ��o, voc� pode querer remover ou logar os detalhes do erro ao inv�s de retorn�-los.
                });
            }
        }



        [HttpGet("get-users")]
        public async Task<IActionResult> GetUsers([FromBody] GetUsersRequest request)
        {
            try
            {
                // Valida��o do login
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null)
                {
                    return Unauthorized(new { Success = false, Message = "Usu�rio n�o est� logado!" });
                }

                // Consulta base de usu�rios
                var query = _context.Usuarios.Where(u => u.DataDelecao == null).AsQueryable();

                // Aplica��o dos filtros, se fornecidos
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

                // Execu��o da consulta e mapeamento para UserResponse
                var usuariosList = await query.Select(u => new UserResponse
                {
                    UsuarioId = u.UsuarioId,
                    Nome = u.Nome,
                    Nickname = u.Nickname,
                    Email = u.Email,
                    Avatar = u.Avatar,
                    NumeroPartidasJogadas = u.NumeroPartidasJogadas,
                    NumTotalPerguntasRespondidas = u.NumTotalPerguntasRespondidas,
                    NumTotalPerguntasRejeitadas = u.NumTotalPerguntasRejeitadas,
                    NumTotalPerguntasAceita = u.NumTotalPerguntasAceita,
                    PremiacaoTotal = u.PremiacaoTotal,
                    QuantidadeUtilizacaoAjuda = u.QuantidadeUtilizacaoAjuda,
                    NumeroDerrotasErro = u.NumeroDerrotasErro,
                    NumeroDerrotasParada = u.NumeroDerrotasParada,
                    DataCriacao = u.DataCriacao
                }).ToListAsync();

                // Verifica se a lista de usu�rios est� vazia
                if (!usuariosList.Any())
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Nenhum usu�rio encontrado!"
                    });
                }

                // Usar uma fila para organizar o processamento dos usu�rios
                var filaUsuarios = new Queue<UserResponse>(usuariosList);

                // Processamento da fila
                var usuariosProcessados = new List<UserResponse>();
                while (filaUsuarios.Count > 0)
                {
                    var usuario = filaUsuarios.Dequeue(); // Retira o usu�rio da fila
                    usuariosProcessados.Add(usuario); // Adiciona o usu�rio processado � lista
                }

                // Retorna a lista de usu�rios processados
                return Ok(new
                {
                    Success = true,
                    Data = usuariosProcessados
                });
            }
            catch (Exception ex)
            {
                // Tratamento de erros gen�ricos
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Ocorreu um erro ao processar sua solicita��o.",
                    Details = ex.Message
                });
            }
        }


        // Permite que este m�todo seja acessado sem autentica��o
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verifica se o email j� est� em uso
            if (await _context.Usuarios.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Email j� est� em uso!"
                });
            }

            // Gerar salt e hash da senha
            var salt = PasswordService.GenerateSalt();
            var hashedPassword = PasswordService.HashPassword(request.Senha, salt);

            // Criar o novo usu�rio
            var newUser = new Usuario
            {
                Nome = request.Nome,
                Nickname = request.Nickname,
                Email = request.Email,
                Avatar = request.Avatar,
                PasswordSalt = salt,
                PasswordHash = hashedPassword,
                DataCriacao = DateTime.UtcNow 
                                              
            };

            // Adicionar o usu�rio ao banco de dados
            _context.Usuarios.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Success = true,
                UsuarioId = newUser.UsuarioId,
                Message = "Usu�rio registrado com sucesso!"
            });
        }

        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUserById([FromBody] UpdateUserByIdRequest request)
        {
            // Obt�m o ID do usu�rio logado pela sess�o ou pelo User.Identity
            if (request.Id == null)
            {
                return Unauthorized(new { Success = false, Message = "Usu�rio n�o autenticado." });
            }

            // Converte o ID obtido em n�mero (int)
            if (!await _context.Usuarios.AnyAsync(u => u.UsuarioId == request.Id))
            {
                return BadRequest(new { Success = false, Message = "ID do usu�rio inv�lido." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { Success = false, Message = "Dados inv�lidos." });
            }

            try
            {
                var usuario = await _context.Usuarios.FindAsync(request.Id);
                if (usuario == null)
                {
                    return NotFound(new { Success = false, Message = "Usu�rio n�o encontrado." });
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

                if (request.NumTotalPerguntasRespondidas.HasValue)
                {
                    usuario.NumTotalPerguntasRespondidas = request.NumTotalPerguntasRespondidas.Value;
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

                // Salva as mudan�as no banco de dados
                _context.Usuarios.Update(usuario);
                await _context.SaveChangesAsync();

                return Ok(new { Success = true, Message = "Usu�rio atualizado com sucesso." });
            }
            catch (Exception ex)
            {
                // Tratamento de erros gen�ricos
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Ocorreu um erro ao processar sua solicita��o.",
                    Details = ex.Message // Em produ��o, considere remover ou logar os detalhes do erro ao inv�s de retorn�-los.
                });
            }
        }



        [HttpDelete("delete-user")]
        public async Task<IActionResult> DeleteUserById([FromBody] DeleteUserByIdRequest request)
        {
            try
            {
                // Valida��o do login
                var loggedInUserId = HttpContext.Session.GetInt32("UsuarioId");
                if (loggedInUserId == null)
                {
                    return Unauthorized(new { Success = false, Message = "Usu�rio n�o est� logado!" });
                }

                // Verifica se o usu�rio a ser exclu�do existe
                var usuario = await _context.Usuarios.FindAsync(request.UsuarioId);
                if (usuario == null)
                {
                    return NotFound(new { Success = false, Message = "Usu�rio n�o encontrado!" });
                }

                // Verifica se o usu�rio logado tem permiss�o para excluir este usu�rio
                if (loggedInUserId.Value != request.UsuarioId)
                {
                    return Forbid();
                }

                // Marca o usu�rio como exclu�do
                usuario.DataDelecao = DateTime.UtcNow; // Atualiza a data de exclus�o
                _context.Usuarios.Update(usuario);
                await _context.SaveChangesAsync();

                return Ok(new { Success = true, Message = "Usu�rio marcado como exclu�do com sucesso!" });
            }
            catch (Exception ex)
            {
                // Tratamento de erros gen�ricos
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Ocorreu um erro ao processar sua solicita��o.",
                    Details = ex.Message // Em produ��o, voc� pode querer remover ou logar os detalhes do erro ao inv�s de retorn�-los.
                });
            }
        }


    }
}

