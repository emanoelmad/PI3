using System;
using System.ComponentModel.DataAnnotations;

namespace AppShowDoMilhao.Models.UsuarioModel
{
    public class UpdateUserByIdRequest
    {
        [Required]
        public int UsuarioId { get; set; }

        [StringLength(100)]
        public string? Nome { get; set; }

        [StringLength(50)]
        public string? Nickname { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(255)]
        public string? Avatar { get; set; }

        [StringLength(100, MinimumLength = 6)]
        public string? Senha { get; set; }

        // Outras propriedades que podem ser atualizadas
        public int? NumeroPartidasJogadas { get; set; }
        public int? NumTotalPerguntasRespondidas { get; set; }
        public decimal? PremiacaoTotal { get; set; }
        public int? QuantidadeUtilizacaoAjuda { get; set; }
        public int? NumeroDerrotasErro { get; set; }
        public int? NumeroDerrotasParada { get; set; }

    }
}
