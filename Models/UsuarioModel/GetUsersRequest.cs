using System;
using System.ComponentModel.DataAnnotations;

namespace AppShowDoMilhao.Models.UsuarioModel
{
    public class GetUsersRequest
    {
        // Parâmetros de filtro
        [StringLength(100)]
        public string? Nome { get; set; }

        [StringLength(50)]
        public string? Nickname { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

    }
}
