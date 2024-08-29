using System.ComponentModel.DataAnnotations;

namespace AppShowDoMilhao.Models.UsuarioModel
{
    public class CreateUserRequest
    {
        [Required(ErrorMessage = "O nome completo é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome completo deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O nickname é obrigatório.")]
        [StringLength(50, ErrorMessage = "O nickname deve ter no máximo 50 caracteres.")]
        public string Nickname { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O e-mail fornecido não é válido.")]
        [StringLength(100, ErrorMessage = "O e-mail deve ter no máximo 100 caracteres.")]
        public string Email { get; set; }

        [StringLength(255, ErrorMessage = "A URL do avatar deve ter no máximo 255 caracteres.")]
        public string? Avatar { get; set; } // O avatar é opcional

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres.")]
        public string Senha { get; set; }
    }
}
