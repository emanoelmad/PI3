using System;
using System.ComponentModel.DataAnnotations;

namespace AppShowDoMilhao.Models.UsuarioModel
{
    public class UsuarioModel
    {
        [Key]
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(50)]
        public string NomeCompleto { get; set; }

        [Required]
        [StringLength(50)]
        public string Email { get; set; }

        [StringLength(100)]
        public string Avatar { get; set; }

        [Required]
        [StringLength(100)]
        public string Senha { get; set; }

        [Required]
        public DateTime DataCriacao { get; set; }

        public DateTime? DataDelecao { get; set; }
    }
}
