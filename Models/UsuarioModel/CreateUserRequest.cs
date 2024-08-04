using System;
using System.ComponentModel.DataAnnotations;

namespace AppShowDoMilhao.Models.UsuarioModel
{
    public class CreateUserRequest
    {
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
    }
}
