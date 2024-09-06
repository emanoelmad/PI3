using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppShowDoMilhao.Models
{
    public class Pergunta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdPergunta { get; set; }

        [Required]
        public int IdUsuario { get; set; }

        [Required]
        public string Enunciado { get; set; }

        [Required]
        [StringLength(255)]
        public string Alternativa_A { get; set; }

        [Required]
        [StringLength(255)]
        public string Alternativa_B { get; set; }

        [Required]
        [StringLength(255)]
        public string Alternativa_C { get; set; }

        [Required]
        [StringLength(255)]
        public string Alternativa_D { get; set; }

        [Required]
        [StringLength(255)]
        public string RespostaCorreta { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "pendente";

        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        public DateTime? DataAtualizacao { get; set; }

        public DateTime? DataDelecao { get; set; }

    }
}
