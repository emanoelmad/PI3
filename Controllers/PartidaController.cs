using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using AppShowDoMilhao.Data;
using AppShowDoMilhao.Models.PartidaModel;
using AppShowDoMilhao.Models.PerguntaModel;
using AppShowDoMilhao.Models;

namespace AppShowDoMilhao.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PartidaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PartidaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Iniciar Nova Partida
        [HttpPost("iniciar")]
        public async Task<IActionResult> IniciarPartida()
        {
            try
            {

                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null)
                {
                    return Unauthorized(new { Success = false, Message = "Usuário não está logado!" });
                }

                var novaPartida = new Partida
                {
                    IdUsuario = usuarioId.Value,
                    DataPartida = DateTime.UtcNow,
                    Status = "Em Andamento"
                };

                _context.Partidas.Add(novaPartida);
                await _context.SaveChangesAsync();

                return Ok(new { Success = true, Message = "Partida iniciada com sucesso!", PartidaId = novaPartida.IdPartida });
            }
            catch (Exception ex)
            {
                // Retorna um erro genérico para o cliente
                return StatusCode(500, new { Success = false, Message = "Ocorreu um erro ao iniciar a partida. Tente novamente mais tarde." });
            }
        }

        // 2. Obter Pergunta Atual
        [HttpGet("perguntaAtual/{partidaId}")]
        public async Task<IActionResult> ObterPerguntaAtual(int partidaId)
        {
            var partida = await _context.Partidas.FindAsync(partidaId);
            if (partida == null || partida.Status != "Em Andamento")
            {
                return NotFound(new { Success = false, Message = "Partida não encontrada ou já finalizada!" });
            }

            var pergunta = await _context.Perguntas
                .Where(p => p.DataDelecao == null)
                .OrderBy(p => Guid.NewGuid()) // Obter pergunta aleatória
                .FirstOrDefaultAsync();

            if (pergunta == null)
            {
                return NotFound(new { Success = false, Message = "Nenhuma pergunta disponível no momento!" });
            }

            var perguntaResponse = new PerguntaResponse
            {
                IdPergunta = pergunta.IdPergunta,
                Enunciado = pergunta.Enunciado,
                AlternativaA = pergunta.Alternativa_A,
                AlternativaB = pergunta.Alternativa_B,
                AlternativaC = pergunta.Alternativa_C,
                AlternativaD = pergunta.Alternativa_D,
            };

            return Ok(new { Success = true, Data = perguntaResponse });
        }

        // 3. Responder Pergunta
        [HttpPost("responder")]
        public async Task<IActionResult> ResponderPergunta([FromBody] RespostaRequest request)
        {
            var partida = await _context.Partidas.FindAsync(request.PartidaId);

            if (partida == null)
            {
                return NotFound(new { Success = false, Message = "Partida não encontrada!" });
            }

            if (partida.Status != "Em Andamento")
            {
                return BadRequest(new { Success = false, Message = "Partida já foi finalizada!" });
            }

            var pergunta = await _context.Perguntas.FindAsync(request.PerguntaId);
            if (pergunta == null)
            {
                return NotFound(new { Success = false, Message = "Pergunta não encontrada!" });
            }

            // Variáveis de controle
            int perguntasTotais = 5;
            decimal premioFinal = 1000000m;
            decimal premioPorPergunta = premioFinal / perguntasTotais;

            var respostaCorreta = pergunta.RespostaCorreta;
            var respostaCerta = respostaCorreta == request.RespostaEscolhida;

            if (respostaCerta)
            {
                partida.PremioObtido += premioPorPergunta;
                partida.NumRespostasCertas += 1; 
            }

            partida.NumPerguntasRespondidas += 1;

            bool partidaFinalizada = partida.NumPerguntasRespondidas >= perguntasTotais;

            if (partidaFinalizada)
            {
                // Verifica se todas as respostas foram corretas para aplicar o prêmio final
                if (partida.NumRespostasCertas == perguntasTotais)
                {
                    partida.Status = "Vitória";
                }
                else
                {
                    partida.Status = "Finalizada";
                }
            }

            _context.Partidas.Update(partida);
            await _context.SaveChangesAsync();

            return Ok(new { Success = true, Message = "Resposta processada com sucesso!", NovoPremio = partida.PremioObtido, PartidaFinalizada = partidaFinalizada });
        }


        // 4. Usar Ajuda
        [HttpPost("usarAjuda")]
        public async Task<IActionResult> UsarAjuda([FromBody] AjudaRequest request)
        {
            var partida = await _context.Partidas.FindAsync(request.PartidaId);
            if (partida == null || partida.Status != "Em Andamento")
            {
                return NotFound(new { Success = false, Message = "Partida não encontrada ou já finalizada!" });
            }

            var pergunta = await _context.Perguntas.FindAsync(request.PerguntaId);
            if (pergunta == null)
            {
                return NotFound(new { Success = false, Message = "Pergunta não encontrada!" });
            }

            // Lógica para eliminar duas alternativas erradas
            var alternativas = new[] { pergunta.Alternativa_A, pergunta.Alternativa_B, pergunta.Alternativa_C, pergunta.Alternativa_D };
            var alternativasErradas = alternativas.Where(a => a != pergunta.RespostaCorreta).OrderBy(a => Guid.NewGuid()).Take(2).ToList();

            return Ok(new
            {
                Success = true,
                AlternativasRestantes = alternativasErradas.Append(pergunta.RespostaCorreta).OrderBy(a => Guid.NewGuid())
            });
        }

        // 5. Finalizar Partida
        [HttpPost("finalizar")]
        public async Task<IActionResult> FinalizarPartida(int partidaId)
        {
            var partida = await _context.Partidas.FindAsync(partidaId);
            if (partida == null || partida.Status != "Em Andamento")
            {
                return NotFound(new { Success = false, Message = "Partida não encontrada ou já finalizada!" });
            }

            partida.Status = "Finalizada";

            await _context.SaveChangesAsync();

            return Ok(new { Success = true, Message = "Partida finalizada com sucesso!", PremioObtido = partida.PremioObtido });
        }

        // 6. Obter Histórico de Partidas
        [HttpGet("historico")]
        public async Task<IActionResult> ObterHistoricoDePartidas()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                return Unauthorized(new { Success = false, Message = "Usuário não está logado!" });
            }

            var historico = await _context.Partidas
                .Where(p => p.IdUsuario == usuarioId.Value && p.DataDelecao == null)
                .Select(p => new
                {
                    p.IdPartida,
                    p.DataPartida,
                    p.PremioObtido,
                    p.Status
                })
                .ToListAsync();

            return Ok(new { Success = true, Data = historico });
        }
    }
}
