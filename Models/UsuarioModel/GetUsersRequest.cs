using System.ComponentModel.DataAnnotations;


namespace AppShowDoMilhao.Models.UsuarioModel
{
    public class GetUsersRequest
    {
        [Required]
        public string Method { get; set; }

        [Required]
        public int UsuarioId { get; set; }
    }
}
