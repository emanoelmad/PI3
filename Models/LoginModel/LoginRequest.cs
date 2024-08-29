using System.ComponentModel.DataAnnotations;

namespace AppShowDoMilhao.Models.LoginModel
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "O campo Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O formato do email é inválido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O campo Senha é obrigatório.")]
        public string Senha { get; set; }
    }
}
