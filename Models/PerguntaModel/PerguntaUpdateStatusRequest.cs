using System.ComponentModel.DataAnnotations;

namespace AppShowDoMilhao.Models
{
    public class PerguntaUpdateStatusRequest
    {
        [Required]
        public int IdPergunta { get; set; }

        [Required]
        [StringLength(20)]
        public string NovoStatus { get; set; }
    }
}
