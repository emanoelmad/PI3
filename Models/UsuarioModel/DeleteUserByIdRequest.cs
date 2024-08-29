using System;
using System.ComponentModel.DataAnnotations;

namespace AppShowDoMilhao.Models.UsuarioModel
{
    public class DeleteUserByIdRequest
    {
        [Required]
        public int UsuarioId { get; set; }
    }
}
