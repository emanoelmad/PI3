using System;
using System.ComponentModel.DataAnnotations;

namespace AppShowDoMilhao.Models.UsuarioModel
{
    public class UpdateUserByIdRequest
    {
        [Required]
        public int UsuarioId { get; set; }

        [StringLength(50)]
        public string NomeCompleto { get; set; }

        [StringLength(50)]
        public string Email { get; set; }

        [StringLength(100)]
        public string Avatar { get; set; }
    }
}
