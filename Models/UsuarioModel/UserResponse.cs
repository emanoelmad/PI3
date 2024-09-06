using System;

namespace AppShowDoMilhao.Models.UsuarioModel
{
    public class UserResponse
    {
        public int UsuarioId { get; set; }

        public string Nome { get; set; }

        public string Nickname { get; set; }

        public string Email { get; set; }

        public string? Avatar { get; set; }

        public int NumeroPartidasJogadas { get; set; }

        public int NumTotalPerguntasRespondidas { get; set; } // Renomeado para refletir a model

        public int NumTotalPerguntasRejeitadas { get; set; } // Adicionado

        public int NumTotalPerguntasAceita { get; set; } // Adicionado

        public decimal PremiacaoTotal { get; set; }

        public int QuantidadeUtilizacaoAjuda { get; set; }

        public int NumeroDerrotasErro { get; set; }

        public int NumeroDerrotasParada { get; set; }

        public DateTime DataCriacao { get; set; }
    }
}
