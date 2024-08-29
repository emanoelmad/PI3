using System.ComponentModel.DataAnnotations;

namespace AppShowDoMilhao.Models.UsuarioModel
{
    public class GetUserByIdRequest
    {
        [Required]
        public int UsuarioId { get; set; }
    }
}
