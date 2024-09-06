using System.ComponentModel.DataAnnotations;

namespace AppShowDoMilhao.Models.PerguntaModel
{
    public class DenunciaPergunta
    {
        [Key]
        public int IdDenuncia { get; set; }
        public int IdPergunta { get; set; }
        public int IdUsuario { get; set; }
        public DateTime DataDenuncia { get; set; }
        public DateTime DataDelecao { get; set; }
    }
}
