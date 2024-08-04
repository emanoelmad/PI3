using System;
using System.ComponentModel.DataAnnotations;

namespace AppShowDoMilhao.Models.UsuarioModel
{
    public class DeleteUserByIdRequest
    {
        [Required]
        public string Method { get; set; }

        [Required]
        public int UsuarioId { get; set; }
    }
}
