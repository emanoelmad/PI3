using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using AppShowDoMilhao.Data;
using AppShowDoMilhao.Models.PartidaModel;
using AppShowDoMilhao.Models.PerguntaModel;
using AppShowDoMilhao.Models;
using AppShowDoMilhao.Services;

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
            try
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

                // Cria uma lista das alternativas
                var alternativas = new List<string>
                {
                    pergunta.Alternativa_A,
                    pergunta.Alternativa_B,
                    pergunta.Alternativa_C,
                    pergunta.Alternativa_D
                };

                // Embaralha as alternativas
                alternativas.Shuffle();

                // Cria a resposta
                var perguntaResponse = new PerguntaResponse
                {
                    IdPergunta = pergunta.IdPergunta,
                    Enunciado = pergunta.Enunciado,
                    AlternativaA = alternativas[0],
                    AlternativaB = alternativas[1],
                    AlternativaC = alternativas[2],
                    AlternativaD = alternativas[3]
                };

                return Ok(new { Success = true, Data = perguntaResponse });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Ocorreu um erro ao obter a pergunta.", Details = ex.Message });
            }
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

            // Lógica de prêmios por pergunta
            var premios = new[]
            {
                new { Pergunta = 1, Acertar = 1000m, Parar = 0m, Errar = 0m },
                new { Pergunta = 2, Acertar = 5000m, Parar = 1000m, Errar = 500m },
                new { Pergunta = 3, Acertar = 50000m, Parar = 5000m, Errar = 2500m },
                new { Pergunta = 4, Acertar = 100000m, Parar = 50000m, Errar = 25000m },
                new { Pergunta = 5, Acertar = 300000m, Parar = 100000m, Errar = 50000m },
                new { Pergunta = 6, Acertar = 500000m, Parar = 300000m, Errar = 150000m },
                new { Pergunta = 7, Acertar = 1000000m, Parar = 500000m, Errar = 0m }
            };

            // Encontra o prêmio da pergunta atual
            var premioAtual = premios.FirstOrDefault(p => p.Pergunta == partida.NumPerguntasRespondidas + 1);
            if (premioAtual == null)
            {
                return BadRequest(new { Success = false, Message = "Número da pergunta inválido!" });
            }

            // Verifica se a resposta está correta
            var respostaCorreta = pergunta.RespostaCorreta == request.RespostaEscolhida;

            // Lógica de resposta correta, parar ou errar
            if (request.DecidiuParar)
            {
                partida.PremioObtido = premioAtual.Parar;
                partida.Status = "Parada";
                _context.Partidas.Update(partida);
                await _context.SaveChangesAsync();
                return Ok(new { Success = true, Message = "Você decidiu parar!", PremioObtido = partida.PremioObtido });
            }
            else if (respostaCorreta)
            {
                partida.PremioObtido = premioAtual.Acertar;
                partida.NumRespostasCertas += 1;
            }
            else
            {
                partida.PremioObtido = premioAtual.Errar;
                partida.Status = "Finalizada";
                _context.Partidas.Update(partida);
                await _context.SaveChangesAsync();
                return Ok(new { Success = true, Message = "Você errou a resposta!", PremioObtido = partida.PremioObtido });
            }

            partida.NumPerguntasRespondidas += 1;

            // Verifica se a partida está finalizada
            bool partidaFinalizada = partida.NumPerguntasRespondidas >= 7;
            if (partidaFinalizada)
            {
                partida.Status = "Vitória";
            }

            _context.Partidas.Update(partida);
            await _context.SaveChangesAsync();

            return Ok(new { Success = true, Message = "Resposta processada com sucesso!", NovoPremio = partida.PremioObtido, PartidaFinalizada = partidaFinalizada });
        }



        // 4. Usar Ajuda
        [HttpPost("usarAjuda")]
        public async Task<IActionResult> UsarAjuda([FromBody] AjudaRequest request)
        {
            try
            {
                // Verifica se o usuário está logado
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null)
                {
                    return Unauthorized(new { Success = false, Message = "Usuário não está logado!" });
                }

                // Verifica se a partida está em andamento
                var partida = await _context.Partidas.FindAsync(request.PartidaId);
                if (partida == null || partida.Status != "Em Andamento")
                {
                    return NotFound(new { Success = false, Message = "Partida não encontrada ou já finalizada!" });
                }

                // Verifica se a pergunta existe
                var pergunta = await _context.Perguntas.FindAsync(request.PerguntaId);
                if (pergunta == null)
                {
                    return NotFound(new { Success = false, Message = "Pergunta não encontrada!" });
                }

                // Lógica para eliminar duas alternativas erradas
                var alternativas = new[] { pergunta.Alternativa_A, pergunta.Alternativa_B, pergunta.Alternativa_C, pergunta.Alternativa_D };
                var alternativasErradas = alternativas.Where(a => a != pergunta.RespostaCorreta).OrderBy(a => Guid.NewGuid()).Take(2).ToList();

                // Atualiza o campo Quant_Utilizacao_Ajuda do usuário
                var usuario = await _context.Usuarios.FindAsync(usuarioId);
                if (usuario != null)
                {
                    usuario.QuantidadeUtilizacaoAjuda += 1;
                    _context.Usuarios.Update(usuario);
                    await _context.SaveChangesAsync();
                }

                // Retorna as alternativas restantes, embaralhadas
                return Ok(new
                {
                    Success = true,
                    AlternativasRestantes = alternativasErradas.Append(pergunta.RespostaCorreta).OrderBy(a => Guid.NewGuid())
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Erro ao usar a ajuda.", Details = ex.Message });
            }
        }


        // 5. Finalizar Partida
        [HttpPost("finalizar")]
        public async Task<IActionResult> FinalizarPartida(int partidaId)
        {
            // Busca a partida pelo ID
            var partida = await _context.Partidas.FindAsync(partidaId);
            if (partida == null || partida.Status != "Em Andamento")
            {
                return NotFound(new { Success = false, Message = "Partida não encontrada ou já finalizada!" });
            }

            // Finaliza a partida alterando o status
            partida.Status = "Finalizada";

            // Busca o usuário associado à partida
            var usuario = await _context.Usuarios.FindAsync(partida.IdUsuario); // Supondo que existe um campo UsuarioId na tabela de Partidas
            if (usuario == null)
            {
                return NotFound(new { Success = false, Message = "Usuário associado à partida não encontrado!" });
            }

            // Incrementa o número de partidas jogadas pelo usuário
            usuario.NumeroPartidasJogadas++;

            // Salva as alterações no banco de dados
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Success = true,
                Message = "Partida finalizada com sucesso!",
                PremioObtido = partida.PremioObtido
            });
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
