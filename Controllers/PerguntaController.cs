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

                // Verifica a aprovação existente
                var aprovacao = await _context.PerguntaAprovacoes
                    .FirstOrDefaultAsync(a => a.IdPergunta == request.IdPergunta && a.IdUsuario == usuarioId.Value);

                if (aprovacao == null)
                {
                    return NotFound(new { Success = false, Message = "Aprovação não encontrada!" });
                }

                // Atualiza a aprovação
                aprovacao.Aprovado = request.Aprovado;
                aprovacao.DataAprovacao = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Verifica quantas aprovações a pergunta já possui
                var totalAprovacoes = await _context.PerguntaAprovacoes
                    .CountAsync(a => a.IdPergunta == request.IdPergunta && a.Aprovado == true);

                if (totalAprovacoes >= 5)
                {
                    // Marca a pergunta como aprovada
                    var pergunta = await _context.Perguntas.FindAsync(request.IdPergunta);
                    if (pergunta != null)
                    {
                        pergunta.Status = "Aprovada";
                        await _context.SaveChangesAsync();

                        // Atualiza o número total de perguntas do usuário
                        var usuarioQueCriouPergunta = await _context.Usuarios
                            .FirstOrDefaultAsync(u => u.UsuarioId == pergunta.IdUsuario);
                        if (usuarioQueCriouPergunta != null)
                        {
                            usuarioQueCriouPergunta.NumTotalPerguntasRespondidas = (usuarioQueCriouPergunta.NumTotalPerguntasRespondidas) + 1;
                            _context.Usuarios.Update(usuarioQueCriouPergunta);
                            await _context.SaveChangesAsync();
                        }

                        return Ok(new { Success = true, Message = "Pergunta aprovada!" });
                    }

                    return NotFound(new { Success = false, Message = "Pergunta não encontrada!" });
                }

                return Ok(new { Success = true, Message = "Aprovação registrada!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Erro ao aprovar a pergunta.", Details = ex.Message });
            }
        }

        // 6. Lista de Perguntas Adicionadas (Aceitas e Rejeitadas)
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

                // Busca as perguntas que não foram deletadas e que possuem status "Aprovada" ou "Rejeitada"
                var perguntas = await _context.Perguntas
                    .Where(p => p.DataDelecao == null && (p.Status == "Aprovada" || p.Status == "Rejeitada"))
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



        // 7. Obter Perguntas para Edição.
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
                    AlternativaD = pergunta.Alternativa_D,
                    RespostaCorreta = pergunta.RespostaCorreta
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

        [HttpPut("editar/{idPergunta}")]
        public async Task<IActionResult> EditarPergunta(int idPergunta, [FromBody] PerguntaUpdateRequest request)
        {
            try
            {
                // Verifica se o usuário está logado
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null)
                {
                    return Unauthorized(new { Success = false, Message = "Usuário não está logado!" });
                }

                // Busca a pergunta pelo ID, assegurando que ela pertence ao usuário e está com status "Rejeitada" ou "Pendente"
                var pergunta = await _context.Perguntas
                    .FirstOrDefaultAsync(p => p.IdPergunta == idPergunta && p.IdUsuario == usuarioId.Value && (p.Status == "Rejeitada" || p.Status == "Pendente"));

                if (pergunta == null)
                {
                    return NotFound(new { Success = false, Message = "Pergunta não encontrada ou não pode ser editada!" });
                }

                // Atualiza os campos editáveis da pergunta
                pergunta.Enunciado = request.Enunciado ?? pergunta.Enunciado;
                pergunta.Alternativa_A = request.AlternativaA ?? pergunta.Alternativa_A;
                pergunta.Alternativa_B = request.AlternativaB ?? pergunta.Alternativa_B;
                pergunta.Alternativa_C = request.AlternativaC ?? pergunta.Alternativa_C;
                pergunta.Alternativa_D = request.AlternativaD ?? pergunta.Alternativa_D;
                pergunta.RespostaCorreta = request.RespostaCorreta ?? pergunta.RespostaCorreta;
                pergunta.DataAtualizacao = DateTime.UtcNow;

                // Redefine o status para "Pendente" para reavaliar a pergunta após a edição
                pergunta.Status = "Pendente";

                _context.Perguntas.Update(pergunta);
                await _context.SaveChangesAsync();

                // Remove as aprovações anteriores associadas a essa pergunta
                var aprovacoesExistentes = _context.PerguntaAprovacoes
                    .Where(pa => pa.IdPergunta == idPergunta);
                _context.PerguntaAprovacoes.RemoveRange(aprovacoesExistentes);

                // Seleciona cinco usuários aleatórios para aprovar a pergunta editada
                var usuarios = _context.Usuarios.OrderBy(u => Guid.NewGuid()).Take(5).ToList();

                foreach (var usuario in usuarios)
                {
                    var aprovacao = new PerguntaAprovacao
                    {
                        IdPergunta = pergunta.IdPergunta,
                        IdUsuario = usuario.UsuarioId,
                        Aprovado = false, 
                        DataAprovacao = null 
                    };
                    _context.PerguntaAprovacoes.Add(aprovacao);
                }

                await _context.SaveChangesAsync();

                return Ok(new { Success = true, Message = "Pergunta editada com sucesso! Aguardando nova aprovação." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Erro ao editar a pergunta.", Details = ex.Message });
            }
        }

        [HttpPost("denunciar/{idPergunta}")]
        public async Task<IActionResult> DenunciarPergunta(int idPergunta)
        {
            try
            {
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null)
                {
                    return Unauthorized(new { Success = false, Message = "Usuário não está logado!" });
                }

                // Verifica se o usuário já denunciou a pergunta
                var denunciaExistente = await _context.DenunciaPerguntas
                    .FirstOrDefaultAsync(d => d.IdPergunta == idPergunta && d.IdUsuario == usuarioId.Value);

                if (denunciaExistente != null)
                {
                    return BadRequest(new { Success = false, Message = "Você já denunciou esta pergunta." });
                }

                // Adiciona a denúncia
                var denuncia = new DenunciaPergunta
                {
                    IdPergunta = idPergunta,
                    IdUsuario = usuarioId.Value,
                    DataDenuncia = DateTime.UtcNow
                };

                _context.DenunciaPerguntas.Add(denuncia);
                await _context.SaveChangesAsync();

                // Verifica quantas denúncias a pergunta já possui
                var totalDenuncias = await _context.DenunciaPerguntas
                    .CountAsync(d => d.IdPergunta == idPergunta);

                // Se houverem duas ou mais denúncias, alterar o status da pergunta
                if (totalDenuncias >= 2)
                {
                    var pergunta = await _context.Perguntas.FindAsync(idPergunta);
                    pergunta.Status = "Em Revisao";
                    pergunta.DataAtualizacao = DateTime.UtcNow;
                    _context.Perguntas.Update(pergunta);
                    await _context.SaveChangesAsync();

                    // Seleciona cinco usuários aleatórios para revisar a pergunta
                    var usuariosRevisores = _context.Usuarios.OrderBy(u => Guid.NewGuid()).Take(5).ToList();
                    foreach (var usuario in usuariosRevisores)
                    {
                        var aprovacao = new PerguntaAprovacao
                        {
                            IdPergunta = idPergunta,
                            IdUsuario = usuario.UsuarioId,
                            Aprovado = false, // Revisão pendente
                            DataAprovacao = null
                        };
                        _context.PerguntaAprovacoes.Add(aprovacao);
                    }

                    await _context.SaveChangesAsync();

                    return Ok(new { Success = true, Message = "Pergunta enviada para revisão após denúncias!" });
                }

                return Ok(new { Success = true, Message = "Denúncia registrada!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Erro ao denunciar a pergunta.", Details = ex.Message });
            }
        }

        // 8. Obter perguntas para Revisão
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

                // Busca as perguntas que estão no status "Em Revisão" e que não foram deletadas
                var perguntas = await _context.Perguntas
                    .Where(p => p.Status == "Em Revisao" && p.DataDelecao == null)
                    .Select(p => new
                    {
                        p.IdPergunta,
                        p.Enunciado,
                        p.Alternativa_A,
                        p.Alternativa_B,
                        p.Alternativa_C,
                        p.Alternativa_D,
                        p.RespostaCorreta,
                        p.Status                       
                    })
                    .ToListAsync();

                return Ok(new { Success = true, Data = perguntas });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Ocorreu um erro ao consultar as perguntas para revisão.", Details = ex.Message });
            }
        }


        [HttpPost("revisar")]
        public async Task<IActionResult> RevisarPergunta([FromBody] AprovarPerguntaRequest request)
        {
            try
            {
                // Verifica se o usuário está logado
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null)
                {
                    return Unauthorized(new { Success = false, Message = "Usuário não está logado!" });
                }

                // Verifica se o usuário já revisou a pergunta
                var aprovacao = await _context.PerguntaAprovacoes
                    .FirstOrDefaultAsync(a => a.IdPergunta == request.IdPergunta && a.IdUsuario == usuarioId.Value);

                if (aprovacao == null)
                {
                    return NotFound(new { Success = false, Message = "Revisão não encontrada!" });
                }

                // Atualiza a revisão
                aprovacao.Aprovado = request.Aprovado;
                aprovacao.DataAprovacao = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Busca o usuário para atualizar suas estatísticas
                var usuario = await _context.Usuarios.FindAsync(usuarioId.Value);
                if (usuario == null)
                {
                    return NotFound(new { Success = false, Message = "Usuário não encontrado!" });
                }

                // Atualiza as estatísticas de acordo com a aprovação ou rejeição
                if (request.Aprovado)
                {
                    usuario.NumTotalPerguntasAceita++; // Incrementa o número de perguntas aprovadas
                }
                else
                {
                    usuario.NumTotalPerguntasRejeitadas++; // Incrementa o número de perguntas rejeitadas
                }

                // Salva as mudanças no usuário
                await _context.SaveChangesAsync();

                // Verifica quantas aprovações e rejeições a pergunta já possui
                var totalAprovacoes = await _context.PerguntaAprovacoes
                    .CountAsync(a => a.IdPergunta == request.IdPergunta && a.Aprovado == true);

                var totalRejeicoes = await _context.PerguntaAprovacoes
                    .CountAsync(a => a.IdPergunta == request.IdPergunta && a.Aprovado == false);

                // Busca a pergunta para alterar seu status conforme as aprovações/rejeições
                var pergunta = await _context.Perguntas.FindAsync(request.IdPergunta);

                if (totalAprovacoes >= 5)
                {
                    pergunta.Status = "Aprovada";
                }
                else if (totalRejeicoes >= 3)
                {
                    pergunta.Status = "Rejeitada";
                }

                // Salva as mudanças na pergunta
                await _context.SaveChangesAsync();

                return Ok(new { Success = true, Message = "Revisão registrada!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Erro ao revisar a pergunta.", Details = ex.Message });
            }
        }



    }
}
