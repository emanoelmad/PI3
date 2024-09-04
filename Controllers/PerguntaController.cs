using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using AppShowDoMilhao.Models;
using AppShowDoMilhao.Data;
using AppShowDoMilhao.Models.PerguntaModel;

namespace AppShowDoMilhao.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PerguntaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PerguntaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 5. Adição de Pergunta
        [HttpPost("add")]
        public async Task<IActionResult> AddPergunta([FromBody] PerguntaRequest request)
        {
            try
            {
                // Verifica se o usuário está logado
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null)
                {
                    return Unauthorized(new { Success = false, Message = "Usuário não está logado!" });
                }

                // Cria uma nova pergunta
                var pergunta = new Pergunta
                {
                    IdUsuario = usuarioId.Value,
                    Enunciado = request.Enunciado,
                    Alternativa_A = request.AlternativaA,
                    Alternativa_B = request.AlternativaB,
                    Alternativa_C = request.AlternativaC,
                    Alternativa_D = request.AlternativaD,
                    RespostaCorreta = request.RespostaCorreta,
                    Status = "pendente",
                    DataCriacao = DateTime.UtcNow
                };

                _context.Perguntas.Add(pergunta);
                await _context.SaveChangesAsync();

                // Seleciona cinco usuários aleatórios
                var usuarios = _context.Usuarios.OrderBy(u => Guid.NewGuid()).Take(5).ToList();

                foreach (var usuario in usuarios)
                {
                    var aprovacao = new PerguntaAprovacao
                    {
                        IdPergunta = pergunta.IdPergunta,
                        IdUsuario = usuario.UsuarioId,  // Corrigido para usar usuario.IdUsuario
                        Aprovado = null // Inicialmente indefinido
                    };
                    _context.PerguntaAprovacoes.Add(aprovacao);
                }

                await _context.SaveChangesAsync();

                return Ok(new { Success = true, Message = "Pergunta adicionada e enviada para aprovação!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Erro ao adicionar a pergunta.", Details = ex.Message });
            }
        }

        [HttpPost("aprovar")]
        public async Task<IActionResult> AprovarPergunta([FromBody] AprovarPerguntaRequest request)
        {
            try
            {
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null)
                {
                    return Unauthorized(new { Success = false, Message = "Usuário não está logado!" });
                }

                var aprovacao = await _context.PerguntaAprovacoes
                    .FirstOrDefaultAsync(a => a.IdPergunta == request.IdPergunta && a.IdUsuario == usuarioId.Value);

                if (aprovacao == null)
                {
                    return NotFound(new { Success = false, Message = "Aprovação não encontrada!" });
                }

                aprovacao.Aprovado = request.Aprovado;
                aprovacao.DataAprovacao = DateTime.UtcNow; // Atualiza a data de aprovação
                await _context.SaveChangesAsync();

                // Verifica quantas aprovações a pergunta já possui
                var totalAprovacoes = await _context.PerguntaAprovacoes
                    .CountAsync(a => a.IdPergunta == request.IdPergunta && a.Aprovado == true);

                if (totalAprovacoes >= 5)
                {
                    var pergunta = await _context.Perguntas.FindAsync(request.IdPergunta);
                    pergunta.Status = "Aprovada";
                    await _context.SaveChangesAsync();

                    return Ok(new { Success = true, Message = "Pergunta aprovada!" });
                }

                return Ok(new { Success = true, Message = "Aprovação registrada!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Erro ao aprovar a pergunta.", Details = ex.Message });
            }
        }

        // 6. Lista de Perguntas Adicionadas (Aceitas e Não Aceitas)
        [HttpGet("list")]
        public async Task<IActionResult> GetPerguntas()
        {
            try
            {
                // Verifica se o usuário está logado
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null)
                {
                    return Unauthorized(new { Success = false, Message = "Usuário não está logado!" });
                }

                // Busca as perguntas filtrando para não mostrar as deletadas
                var perguntas = await _context.Perguntas
                    .Where(p => p.DataDelecao == null)
                    .Select(p => new
                    {
                        p.IdPergunta,
                        p.IdUsuario,
                        p.Enunciado,
                        p.Alternativa_A,
                        p.Alternativa_B,
                        p.Alternativa_C,
                        p.Alternativa_D,
                        p.RespostaCorreta,
                        p.Status,
                        p.DataCriacao
                    })
                    .ToListAsync();

                return Ok(new { Success = true, Data = perguntas });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Ocorreu um erro ao consultar as perguntas.", Details = ex.Message });
            }
        }


        // 7. Edição de Perguntas
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> GetPerguntaForEdit(int id)
        {
            try
            {
                // Verifica se o usuário está logado
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null)
                {
                    return Unauthorized(new { Success = false, Message = "Usuário não está logado!" });
                }

                // Busca a pergunta pelo ID, garantindo que não foi deletada
                var pergunta = await _context.Perguntas
                    .FirstOrDefaultAsync(p => p.IdPergunta == id && p.DataDelecao == null);

                if (pergunta == null)
                {
                    return NotFound(new { Success = false, Message = "Pergunta não encontrada!" });
                }

                // Verifica se o usuário logado é o proprietário da pergunta (opcional, dependendo do caso de uso)
                if (pergunta.IdUsuario != usuarioId)
                {
                    return Forbid();
                }

                // Mapeia a entidade Pergunta para PerguntaResponse
                var perguntaResponse = new PerguntaResponse
                {
                    IdPergunta = pergunta.IdPergunta,
                    Enunciado = pergunta.Enunciado,
                    AlternativaA = pergunta.Alternativa_A,
                    AlternativaB = pergunta.Alternativa_B,
                    AlternativaC = pergunta.Alternativa_C,
                    AlternativaD = pergunta.Alternativa_D
                };

                return Ok(new { Success = true, Data = perguntaResponse });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Ocorreu um erro ao consultar a pergunta para edição.",
                    Details = ex.Message
                });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdatePergunta([FromBody] PerguntaUpdateRequest request)
        {
            try
            {
                // Verifica se o usuário está logado
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null)
                {
                    return Unauthorized(new { Success = false, Message = "Usuário não está logado!" });
                }

                var pergunta = await _context.Perguntas.FindAsync(request.IdPergunta);
                if (pergunta == null)
                {
                    return NotFound(new { Success = false, Message = "Pergunta não encontrada!" });
                }

                // Atualiza os campos da pergunta somente se forem fornecidos na request
                if (!string.IsNullOrEmpty(request.Enunciado))
                {
                    pergunta.Enunciado = request.Enunciado;
                }
                if (!string.IsNullOrEmpty(request.AlternativaA))
                {
                    pergunta.Alternativa_A = request.AlternativaA;
                }
                if (!string.IsNullOrEmpty(request.AlternativaB))
                {
                    pergunta.Alternativa_B = request.AlternativaB;
                }
                if (!string.IsNullOrEmpty(request.AlternativaC))
                {
                    pergunta.Alternativa_C = request.AlternativaC;
                }
                if (!string.IsNullOrEmpty(request.AlternativaD))
                {
                    pergunta.Alternativa_D = request.AlternativaD;
                }
                if (!string.IsNullOrEmpty(request.RespostaCorreta))
                {
                    pergunta.RespostaCorreta = request.RespostaCorreta;
                }

                _context.Perguntas.Update(pergunta);
                await _context.SaveChangesAsync();

                return Ok(new { Success = true, Message = "Pergunta atualizada com sucesso!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Ocorreu um erro ao atualizar a pergunta.", Details = ex.Message });
            }
        }



        // 8. Revisão de Pergunta
        [HttpGet("review")]
        public async Task<IActionResult> GetPerguntasParaRevisao()
        {
            try
            {
                // Verifica se o usuário está logado
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null)
                {
                    return Unauthorized(new { Success = false, Message = "Usuário não está logado!" });
                }

                var perguntas = await _context.Perguntas
                    .Where(p => p.Status == "denunciada" && p.DataDelecao == null)
                    .ToListAsync();

                return Ok(new { Success = true, Data = perguntas });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Ocorreu um erro ao consultar as perguntas para revisão.", Details = ex.Message });
            }
        }

        [HttpPut("review/update")]
        public async Task<IActionResult> UpdatePerguntaStatus([FromBody] PerguntaUpdateStatusRequest request)
        {
            try
            {
                // Verifica se o usuário está logado
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null)
                {
                    return Unauthorized(new { Success = false, Message = "Usuário não está logado!" });
                }

                var pergunta = await _context.Perguntas.FindAsync(request.IdPergunta);
                if (pergunta == null)
                {
                    return NotFound(new { Success = false, Message = "Pergunta não encontrada!" });
                }

                // Atualiza o status da pergunta
                pergunta.Status = request.NovoStatus;

                _context.Perguntas.Update(pergunta);
                await _context.SaveChangesAsync();

                return Ok(new { Success = true, Message = "Status da pergunta atualizado com sucesso!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Ocorreu um erro ao atualizar o status da pergunta.", Details = ex.Message });
            }
        }

    }
}
