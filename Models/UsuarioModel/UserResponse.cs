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

        public int NumeroTotalPerguntas { get; set; }

        public decimal PremiacaoTotal { get; set; }

        public int QuantidadeUtilizacaoAjuda { get; set; }

        public int NumeroDerrotasErro { get; set; }

        public int NumeroDerrotasParada { get; set; }

        public DateTime DataCriacao { get; set; }

    }
}
