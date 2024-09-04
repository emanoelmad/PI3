using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppShowDoMilhao.Models.UsuarioModel
{
    public class Usuario
    {
        [Key]
        [Column("IdUsuario")]
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("Nome")]
        public string Nome { get; set; }

        [Required]
        [StringLength(50)]
        [Column("Nickname")]
        public string Nickname { get; set; }

        [Required]
        [StringLength(100)]
        [Column("Email")]
        public string Email { get; set; }

        [StringLength(255)]
        [Column("Avatar")]
        public string? Avatar { get; set; }

        [Column("Numero_PartidasJogadas")]
        public int NumeroPartidasJogadas { get; set; } = 0;

        [Column("Numero_TotalPerguntas")]
        public int NumeroTotalPerguntas { get; set; } = 0;

        [Column("PremiacaoTotal")]
        public decimal PremiacaoTotal { get; set; } = 0.00m;

        [Column("Quant_Utilizacao_Ajuda")]
        public int QuantidadeUtilizacaoAjuda { get; set; } = 0;

        [Column("Numero_Derrotas_Erro")]
        public int NumeroDerrotasErro { get; set; } = 0;

        [Column("Numero_Derrotas_Parada")]
        public int NumeroDerrotasParada { get; set; } = 0;

        [Required]
        [Column("PasswordHash")]
        public byte[] PasswordHash { get; set; } 

        [Required]
        [Column("PasswordSalt")]
        public byte[] PasswordSalt { get; set; } 

        [Required]
        [Column("DataCriacao")]
        public DateTime DataCriacao { get; set; }

        [Column("DataDelecao")]
        public DateTime? DataDelecao { get; set; }
    }
}
