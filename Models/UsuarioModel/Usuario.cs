using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppShowDoMilhao.Models.UsuarioModel
{
    public class Usuario
    {
        [Key]
        [Column("id_usuario")]
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("nome")]
        public string Nome { get; set; }

        [Required]
        [StringLength(50)]
        [Column("nickname")]
        public string Nickname { get; set; }

        [Required]
        [StringLength(100)]
        [Column("Email")]
        public string Email { get; set; }

        [StringLength(255)]
        [Column("avatar")]
        public string? Avatar { get; set; }

        [Column("numero_partidas_jogadas")]
        public int NumeroPartidasJogadas { get; set; } = 0;

        [Column("numero_total_perguntas")]
        public int NumeroTotalPerguntas { get; set; } = 0;

        [Column("premiacao_total")]
        public decimal PremiacaoTotal { get; set; } = 0.00m;

        [Column("quantidade_utilizacao_ajuda")]
        public int QuantidadeUtilizacaoAjuda { get; set; } = 0;

        [Column("numero_derrotas_erro")]
        public int NumeroDerrotasErro { get; set; } = 0;

        [Column("numero_derrotas_parada")]
        public int NumeroDerrotasParada { get; set; } = 0;

        [Required]
        [Column("PasswordHash")]
        public byte[] PasswordHash { get; set; } 

        [Required]
        [Column("PasswordSalt")]
        public byte[] PasswordSalt { get; set; } 

        [Required]
        [Column("data_criacao")]
        public DateTime DataCriacao { get; set; }

        [Column("data_delecao")]
        public DateTime? DataDelecao { get; set; }
    }
}
